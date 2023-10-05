using Hangfire;
using Microsoft.Extensions.Caching.Memory;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Reservation.Request;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Application.Services.Momo.Request;
using TsdDelivery.Domain.Entities;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Services.Momo;

public class MomoService : IMomoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppConfiguration _configuration;
    private readonly IHangFireRepository _hangFireRepository;
    private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    public MomoService(IUnitOfWork unitOfWork,AppConfiguration appConfiguration,IHangFireRepository hangFireRepository)
    {
        _unitOfWork = unitOfWork;
        _configuration = appConfiguration;
        _hangFireRepository = hangFireRepository;
    }
    
    public async Task<OperationResult<string>> ProcessMomoPaymentReturn(MomoOneTimePaymentResultRequest request)
    {
        var result = new OperationResult<string>();
        const string methodName = "AutoCancelReservationWhenOverAllowPaymentTime";
        var cacheKey = $"MomoPayment_{request.signature}";
        if(_cache.Get(cacheKey) != null) 
        {
            // Trả về kết quả từ cache
            result.Payload = "ban da thanh toan thanh cong";
            return result;
        }
        try
        {
            var isValidSignature = request.IsValidSignature(_configuration.MomoConfig.AccessKey, _configuration.MomoConfig.SecretKey);
            if (isValidSignature == true || isValidSignature == false)      // đaonj này đang k check chữ kí
            {
                var id = Guid.Parse(request.orderId);
                var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(id);
                if (request.resultCode == 0)
                {
                    reservation.ReservationStatus = ReservationStatus.AwaitingDriver;
                    await _unitOfWork.ReservationRepository.Update(reservation);
                    var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                    if (!isSuccess)
                    {
                        result.AddError(ErrorCode.ServerError,"Fail to update reservation status");
                    }
                    //todo
                    var include = new [] {"Wallet"};
                    // fix cứng Admin 
                    var admin = await _unitOfWork.UserRepository.GetSingleByCondition(x => x.Role.RoleName == "ADMIN", include);
                    admin.Wallet!.Balance += request.amount;
                    await _unitOfWork.SaveChangeAsync();

                    var transactionForAdmin = new Transaction()
                    {
                        Price = request.amount,
                        Status = TransactionStatus.success.ToString(),
                        PaymentMethod = "Thanh-toan-online",
                        Description = "Nhận tiền thanh toán từ đơn đặt có Mã: " + reservation.Id,
                        WalletId = admin.Wallet!.Id,
                        ReservationId = reservation.Id
                    };
                    // TO DO HERE
                    await _unitOfWork.TransactionRepository.AddAsync(transactionForAdmin);
                    await _unitOfWork.SaveChangeAsync();
                    
                    
                    
                    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(7));
                    
                    result.Payload = "Thanh toan thanh cong";
                    // goi Background Service Check status va refund sau 5p
                    var timeToCancel = DateTime.UtcNow.AddMinutes(5);
                    BackgroundJob.Schedule<IBackgroundService>(
                        x => x.AutoCancelAndRefundWhenOverAllowTimeAwaitingDriver(request.orderId!,request.transId!), timeToCancel);
                    // delete job chờ thanh toán 
                    // ở đây gọi HangfireRepository để delete cái job check over allow payment time 
                    //await _hangFireRepository.DeleteJob(id,methodName);
                }
                else
                {
                    result.Payload = "Payment process failed";
                }
            }
            else
            {
                result.Payload = "Invalid signature in response";
            }
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<(bool, string?, string?)> CreateMomoPayment(CreateReservationRequest request, Reservation entity)
    {
        var momoOneTimePayRequest = new MomoOneTimePaymentRequest(_configuration.MomoConfig.PartnerCode,
            DateTime.Now.Ticks.ToString() + entity.Id ?? string.Empty, (long)request.TotalPrice!,
            entity.Id!.ToString() ?? string.Empty,
            "Thanh toán đặt xe TSD" ?? string.Empty, _configuration.MomoConfig.ReturnUrl,
            _configuration.MomoConfig.IpnUrl, "captureWallet",
            string.Empty);
        momoOneTimePayRequest.MakeSignature(_configuration.MomoConfig.AccessKey,
            _configuration.MomoConfig.SecretKey);
         
        return momoOneTimePayRequest.GetLink(_configuration.MomoConfig.PaymentUrl);
    }
}
