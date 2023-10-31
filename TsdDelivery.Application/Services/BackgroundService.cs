using StackExchange.Redis;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.Mail;
using TsdDelivery.Application.Services.Momo.Request;
using TsdDelivery.Application.Services.Momo.Response;
using TsdDelivery.Application.Services.PayPal.Request;
using TsdDelivery.Domain.Entities;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Services;

public class BackgroundService : IBackgroundService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppConfiguration _configuration;
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly ICurrentTime _currentTime;
    private readonly IPayPalService _payPalService;
    private readonly IMailService _mailService;
    
    public BackgroundService(IUnitOfWork unitOfWork,AppConfiguration appConfiguration
        ,IConnectionMultiplexer connectionMultiplexer
        ,ICurrentTime currentTime,IPayPalService payPalService,
        IMailService mailService)
    {
        _unitOfWork = unitOfWork;
        _configuration = appConfiguration;
        _redisConnection = connectionMultiplexer;
        _currentTime = currentTime;
        _payPalService = payPalService;
        _mailService = mailService;
    }
    
    public async Task AutoCancelReservationWhenOverAllowPaymentTime(Guid reservationId)
    {
        try
        {
            var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(reservationId);
            if (reservation.ReservationStatus == ReservationStatus.AwaitingPayment)
            {
                reservation.ReservationStatus = ReservationStatus.Cancelled;
                await _unitOfWork.SaveChangeAsync();
            }
            
        }
        catch (Exception e)
        {
            throw new Exception($"Error at BackgroundService.AutoCancelReservationWhenOverAllowPaymentTime: Message {e.Message}");
        }
    }

    public async Task AutoCancelAndRefundMoMoWhenOverAllowTimeAwaitingDriver(string paymentMethod,string orderId,string transId)
    {
        try
        {
            var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(Guid.Parse(orderId));
            
            // neu status == AwaitingDriver
            if (reservation!.ReservationStatus == ReservationStatus.AwaitingDriver)
            {
                //
                var orderid = Guid.NewGuid().ToString();
                var sPrice = reservation.TotallPrice.ToString();
                var momoRefundRequest = new MomoRefundRequest(_configuration.MomoConfig.PartnerCode,
                    "momorefund"+orderid, long.Parse(sPrice.Substring(0, sPrice.Length - 3))
                    , orderid,long.Parse(transId));
                momoRefundRequest.MakeSignature(_configuration.MomoConfig.AccessKey,_configuration.MomoConfig.SecretKey);
                (bool createMomoLinkResult, string? createMessage, MomoRefundResponse? data) =  // gui request hoan tien den Momo
                    momoRefundRequest.Refund(_configuration.MomoConfig.RefundUrl);
                
                // neu hoan tien thanh cong
                if (createMomoLinkResult)
                {
                    var include = new [] {"Wallet"};
                    // fix cứng Admin 
                    var admin = await _unitOfWork.UserRepository.GetSingleByCondition(x => x.Role.RoleName == "ADMIN", include);
                    admin.Wallet!.Balance -= data!.amount;
                    await _unitOfWork.SaveChangeAsync();
                    
                    var transactionForAdmin = new Transaction()
                    {
                        Price = data.amount,
                        Status = TransactionStatus.success.ToString(),
                        PaymentMethod = paymentMethod,
                        Description = "Hoàn tiền từ đơn đặt có Mã: " + reservation.Id + " . Vì lý do quá thời gian chờ tài xế",
                        WalletId = admin.Wallet!.Id,
                        ReservationId = reservation.Id,
                        TransactionType = TransactionType.Minus
                    };
                    await _unitOfWork.TransactionRepository.AddAsync(transactionForAdmin);
                    await _unitOfWork.SaveChangeAsync();
                }
                else
                {
                    throw new Exception(createMessage);
                }
                
                reservation.ReservationStatus = ReservationStatus.Cancelled;
                await _unitOfWork.SaveChangeAsync();
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Error at BackgroundService.AutoCancelAndRefundMoMoWhenOverAllowTimeAwaitingDriver: Message {e.Message}");
        }
    }

    public async Task AutoCancelAndRefundPayPalWhenOverAllowTimeAwaitingDriver(string paymentMethod,string orderId, string captureId)
    {
        try
        {
            var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(Guid.Parse(orderId));
            if (reservation!.ReservationStatus == ReservationStatus.AwaitingDriver)
            {
                var paypalRefundRequest =
                    new PayPalRefundRequest(
                        $"Hoàn tiền cho đơn đặt xe : {orderId}"); // will be refund defaut all money payer payed
                var accessToken = await _payPalService.GenerateAccessToken(_configuration.PaypalConfig.ClientId,
                    _configuration.PaypalConfig.SecretKey, _configuration.PaypalConfig.PayPalUrl);
                var isRefund =
                    await paypalRefundRequest.Refund(_configuration.PaypalConfig.PayPalUrl, captureId, accessToken);
                if (isRefund)
                {
                    var include = new[] { "Wallet" };
                    // fix cứng Admin 
                    var admin = await _unitOfWork.UserRepository.GetSingleByCondition(x => x.Role.RoleName == "ADMIN",
                        include);
                    admin.Wallet!.Balance -= reservation!.TotallPrice;
                    await _unitOfWork.SaveChangeAsync();

                    var transactionForAdmin = new Transaction()
                    {
                        Price = reservation!.TotallPrice,
                        Status = TransactionStatus.success.ToString(),
                        PaymentMethod = paymentMethod,
                        Description = "Hoàn tiền từ đơn đặt có Mã: " + reservation.Id +
                                      " . Vì lý do quá thời gian chờ tài xế",
                        WalletId = admin.Wallet!.Id,
                        ReservationId = reservation.Id,
                        TransactionType = TransactionType.Minus
                    };
                    await _unitOfWork.TransactionRepository.AddAsync(transactionForAdmin);
                    reservation.ReservationStatus = ReservationStatus.Cancelled;
                    await _unitOfWork.SaveChangeAsync();
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Error at BackgroundService.AutoCancelAndRefundPayPalWhenOverAllowTimeAwaitingDriver: Message {e.Message}");
        }
    }

    public async Task AutoSendEmailToAdminZaloPayWhenOverAllowTimeAwaitingDriver(string paymentMethod, string orderId)
    {
        try
        {
            var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(Guid.Parse(orderId));
            if (reservation!.ReservationStatus == ReservationStatus.AwaitingDriver)
            {
                var mailData = new MailData(new List<string>() { "viethungdev23@gmail.com" },
                    "Thông báo đơn đặt xe của khách hàng hết hiệu lực",
                    $"Đơn đặt xe có mã : {orderId}, giá tiền : {reservation.TotallPrice}, của khách hàng : {reservation.UserId} đã hết hiệu lực và cần được hoàn lại tiền!!");
                var isSended = await _mailService.SendAsync(mailData, new CancellationToken());
                if (isSended)
                {
                    var include = new[] { "Wallet" };
                    // fix cứng Admin 
                    var admin = await _unitOfWork.UserRepository.GetSingleByCondition(x => x.Role.RoleName == "ADMIN",
                        include);
                    admin.Wallet!.Balance -= reservation!.TotallPrice;
                    await _unitOfWork.SaveChangeAsync();

                    var transactionForAdmin = new Transaction()
                    {
                        Price = reservation!.TotallPrice,
                        Status = TransactionStatus.success.ToString(),
                        PaymentMethod = paymentMethod,
                        Description = "Hoàn tiền từ đơn đặt có Mã: " + reservation.Id +
                                      " . Vì lý do quá thời gian chờ tài xế",
                        WalletId = admin.Wallet!.Id,
                        ReservationId = reservation.Id,
                        TransactionType = TransactionType.Minus
                    };
                    await _unitOfWork.TransactionRepository.AddAsync(transactionForAdmin);
                    reservation.ReservationStatus = ReservationStatus.Cancelled;
                    await _unitOfWork.SaveChangeAsync();
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Error at BackgroundService.AutoSendEmailToAdminZaloPayWhenOverAllowTimeAwaitingDriver: Message {e.Message}");
        }
    }

    public async Task AutoResetCacheUserLoginCount()
    {
        IDatabase redisDb = _redisConnection.GetDatabase();
        string loginCountKey = "login_count_" + _currentTime.GetCurrentTime().ToString("yyyyMMdd");
        redisDb.StringSet(loginCountKey, 0);
    }
}