using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Services.Momo.Request;
using TsdDelivery.Application.Services.Momo.Response;
using TsdDelivery.Domain.Entities;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Services;

public class BackgroundService : IBackgroundService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppConfiguration _configuration;
    public BackgroundService(IUnitOfWork unitOfWork,AppConfiguration appConfiguration)
    {
        _unitOfWork = unitOfWork;
        _configuration = appConfiguration;
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

    public async Task AutoCancelAndRefundWhenOverAllowTimeAwaitingDriver(string orderId,string transId)
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
                        PaymentMethod = "Thanh-toan-online",
                        Description = "Hoàn tiền từ đơn đặt có Mã: " + reservation.Id + " . Vì lý do quá thời gian chờ tài xế",
                        WalletId = admin.Wallet!.Id,
                        ReservationId = reservation.Id
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
            throw new Exception($"Error at BackgroundService.AutoCancelAndRefundWhenOverAllowTimeAwaitingDriver: Message {e.Message}");
        }
    }
}