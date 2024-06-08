using System.Net.Http.Headers;
using Hangfire;
using Newtonsoft.Json;
using StackExchange.Redis;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Application.Services.Momo.Request;
using TsdDelivery.Application.Services.PayPal.Request;
using TsdDelivery.Domain.Entities;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppConfiguration _configuration;
    private readonly IHangFireRepository _hangFireRepository;
    private readonly ICurrentTime _currentTime;
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly IPayPalService _payPalService;

    public PaymentService(IUnitOfWork unitOfWork,AppConfiguration appConfiguration
        ,IHangFireRepository hangFireRepository,ICurrentTime currentTime
        ,IConnectionMultiplexer redisConnection,IPayPalService payPalService)
    {
        _unitOfWork = unitOfWork;
        _configuration = appConfiguration;
        _currentTime = currentTime;
        _redisConnection = redisConnection;
        _hangFireRepository = hangFireRepository;
        _payPalService = payPalService;
    }
    
    public async Task<OperationResult<string>> ProcessPaymentReturn(string paymentMethod, object paymentResultRequest)
    {
        const string methodName = "AutoCancelReservationWhenOverAllowPaymentTime";
        var result = new OperationResult<string>();
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                Reservation reservation = new Reservation(); // initial Reservation
                MomoOneTimePaymentResultRequest momoResult = new MomoOneTimePaymentResultRequest();
                PayPalPaymentResultRequest paypalResult = new PayPalPaymentResultRequest();
                string capture_id = "";
                
                switch (paymentMethod.ToUpper())
                {
                    case "MOMO":
                        momoResult = paymentResultRequest as MomoOneTimePaymentResultRequest;
                        reservation = await VerifyReturnContentMoMo(momoResult);
                        break;
                    
                    case "ZALOPAY":        // hàm này làm ma giáo 
                        var orderId = paymentResultRequest as string;
                        reservation = await VerifyReturnContentZalopay(orderId);
                        break;

                    case "PAYPAL": 
                        paypalResult = paymentResultRequest as PayPalPaymentResultRequest;
                        var kq = await VerifyReturnContentPaypal(paypalResult);
                        reservation = kq.Item1;
                        capture_id = kq.Item2;
                        break;
                }

                reservation.ReservationStatus = ReservationStatus.AwaitingDriver;
                //await _unitOfWork.ReservationRepository.Update(reservation);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess)
                {
                    result.AddError(ErrorCode.ServerError, "Fail to update reservation status");
                    return result;
                }

                // add money to Admin wallet
                var include = new[] { "Wallet" };
                // fix cứng Admin 
                var admin = await _unitOfWork.UserRepository.GetSingleByCondition(x => x.Role.RoleName == "ADMIN",
                    include);
                admin.Wallet!.Balance += reservation.TotallPrice;
                await _unitOfWork.SaveChangeAsync();

                var transactionForAdmin = new Transaction()
                {
                    Price = reservation.TotallPrice,
                    Status = TransactionStatus.success.ToString(),
                    PaymentMethod = paymentMethod,
                    Description = "Nhận tiền thanh toán từ đơn đặt có Mã: " + reservation.Id,
                    WalletId = admin.Wallet!.Id,
                    ReservationId = reservation.Id,
                    TransactionType = TransactionType.Plus
                };
                // TO DO HERE
                await _unitOfWork.TransactionRepository.AddAsync(transactionForAdmin);
                await _unitOfWork.SaveChangeAsync();

                await transaction.CommitAsync();

                //cahe and execute job
                var cache = _redisConnection.GetDatabase();
                var FeHost = cache.StringGet(reservation.Id.ToString()).ToString();
                result.Payload = FeHost;
                // goi Background Service Check status va refund sau 5p
                var timeToCancel = DateTime.UtcNow.AddMinutes(10);
                if (paymentMethod == "MOMO")
                {
                    BackgroundJob.Schedule<IBackgroundService>(
                        x => x.AutoCancelAndRefundMoMoWhenOverAllowTimeAwaitingDriver(paymentMethod,reservation.Id.ToString(),
                            momoResult.transId!.ToString()), timeToCancel);
                }
                if (paymentMethod == "PAYPAL")
                {
                    // ===============================================================================================================================
                    BackgroundJob.Schedule<IBackgroundService>(
                        x => x.AutoCancelAndRefundPayPalWhenOverAllowTimeAwaitingDriver(paymentMethod,reservation.Id.ToString(),
                            capture_id), timeToCancel);
                }
                if (paymentMethod == "ZALOPAY")
                {
                    BackgroundJob.Schedule<IBackgroundService>(
                        x => x.AutoSendEmailToAdminZaloPayWhenOverAllowTimeAwaitingDriver(paymentMethod,reservation.Id.ToString()), timeToCancel);
                }
                
                
                // delete job chờ thanh toán 
                // ở đây gọi HangfireRepository để delete cái job check over allow payment time 
                await _hangFireRepository.DeleteJob(reservation.Id, methodName);

            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
                await transaction.RollbackAsync();
            }
            return result;
        }
    }

    public async Task<OperationResult<string>> CancelPayPalPayment(string token)
    {
        var result = new OperationResult<string>();
        try
        {
            var accessToken = await _payPalService.GenerateAccessToken(_configuration.PaypalConfig.ClientId,
                _configuration.PaypalConfig.SecretKey, _configuration.PaypalConfig.PayPalUrl);
            using HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization",$"Bearer {AccessToken}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync($"{_configuration.PaypalConfig.PayPalUrl}/v2/checkout/orders/{token}");
            var content = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(content);
            Guid reference_id = data.purchase_units[0].reference_id;
            var re = await _unitOfWork.ReservationRepository.GetByIdAsync(reference_id);
            re.ReservationStatus = ReservationStatus.Cancelled;
            await _unitOfWork.SaveChangeAsync();
            
            //cahe and execute job
            var cache = _redisConnection.GetDatabase();
            var FeHost = cache.StringGet(reference_id.ToString()).ToString();
            result.Payload = FeHost;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    private async Task<Reservation> VerifyReturnContentMoMo(MomoOneTimePaymentResultRequest request)
    {
        var isValidSignature = request.IsValidSignature(_configuration.MomoConfig.AccessKey, _configuration.MomoConfig.SecretKey);
        if (!isValidSignature)
        {
            
        }
        var id = Guid.Parse(request.orderId);
        var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(id);
        return reservation;
    }

    private async Task<(Reservation?,string?)> VerifyReturnContentPaypal(PayPalPaymentResultRequest request)
    {
        var accessToken = await _payPalService.GenerateAccessToken(_configuration.PaypalConfig.ClientId,
            _configuration.PaypalConfig.SecretKey, _configuration.PaypalConfig.PayPalUrl);
        var isApproved = await request.IsApproved(_configuration.PaypalConfig.PayPalUrl, accessToken);
        if (!isApproved)
        {
            
        }
        var result = await request.CapturePaymentForOrder(_configuration.PaypalConfig.PayPalUrl, accessToken);
        var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(result.Item1);
        return (reservation,result.Item2);
    }

    private async Task<Reservation> VerifyReturnContentZalopay(string orderId)
    {
        var re = await _unitOfWork.ReservationRepository.GetByIdAsync(Guid.Parse(orderId));
        return re;
    }
}