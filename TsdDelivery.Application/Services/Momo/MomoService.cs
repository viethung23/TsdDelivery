using System.IdentityModel.Tokens.Jwt;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;
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
    private readonly ICurrentTime _currentTime;
    private readonly IConnectionMultiplexer _redisConnection;
    public MomoService(IUnitOfWork unitOfWork,AppConfiguration appConfiguration
        ,IHangFireRepository hangFireRepository,ICurrentTime currentTime
        ,IConnectionMultiplexer redisConnection)
    {
        _unitOfWork = unitOfWork;
        _configuration = appConfiguration;
        _hangFireRepository = hangFireRepository;
        _currentTime = currentTime;
        _redisConnection = redisConnection;
    }
    
    public async Task<(bool, string?, string?)> CreateMomoPayment(CreateReservationRequest request, Reservation entity)
    {
        var momoOneTimePayRequest = new MomoOneTimePaymentRequest(_configuration.MomoConfig.PartnerCode,
            _currentTime.GetCurrentTime().Ticks.ToString() + entity.Code+"tsd" ?? string.Empty, (long)request.TotalPrice!,
            entity.Id!.ToString() ?? string.Empty,
            "Thanh toán đặt xe TSD" ?? string.Empty, _configuration.MomoConfig.ReturnUrl,_configuration.MomoConfig.IpnUrl, "captureWallet",
            string.Empty);
        momoOneTimePayRequest.MakeSignature(_configuration.MomoConfig.AccessKey,
            _configuration.MomoConfig.SecretKey);
         
        return await momoOneTimePayRequest.GetLink(_configuration.MomoConfig.PaymentUrl);
    }
    
    /*public async Task<OperationResult<string>> ProcessMomoPaymentReturn(MomoOneTimePaymentResultRequest request)
    {
        var result = new OperationResult<string>();
        const string methodName = "AutoCancelReservationWhenOverAllowPaymentTime";
        try
        {
            var isValidSignature = request.IsValidSignature(_configuration.MomoConfig.AccessKey, _configuration.MomoConfig.SecretKey);
            if (isValidSignature)
            {
                var id = Guid.Parse(request.orderId);
                var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(id);
                if (request.resultCode == 0)
                {
                    reservation.ReservationStatus = ReservationStatus.AwaitingDriver;
                    //await _unitOfWork.ReservationRepository.Update(reservation);     /// ddang 
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
                        ReservationId = reservation.Id,
                        TransactionType = TransactionType.Plus
                    };
                    // TO DO HERE
                    await _unitOfWork.TransactionRepository.AddAsync(transactionForAdmin);
                    await _unitOfWork.SaveChangeAsync();
                    
                    var cache = _redisConnection.GetDatabase();
                    var FeHost = cache.StringGet(request.orderId).ToString();
                    result.Payload = FeHost;
                    // goi Background Service Check status va refund sau 5p
                    var timeToCancel = DateTime.UtcNow.AddMinutes(55);
                    BackgroundJob.Schedule<IBackgroundService>(
                        x => x.AutoCancelAndRefundWhenOverAllowTimeAwaitingDriver(request.orderId!,request.transId!.ToString()), timeToCancel);
                    // delete job chờ thanh toán 
                    // ở đây gọi HangfireRepository để delete cái job check over allow payment time 
                    await _hangFireRepository.DeleteJob(id,methodName);
                    
                }
                else
                {
                    result.AddError(ErrorCode.ServerError, "Payment process failed");
                    return result;
                }
            }
            else
            {
                result.AddError(ErrorCode.ServerError, "Invalid signature in response");
                return result;
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
    }*/

    
}
