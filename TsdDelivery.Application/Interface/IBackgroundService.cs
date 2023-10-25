namespace TsdDelivery.Application.Interface;

public interface IBackgroundService
{
    Task AutoCancelReservationWhenOverAllowPaymentTime(Guid reservationId);
    Task AutoCancelAndRefundWhenOverAllowTimeAwaitingDriver(string orderId,string transId);
    Task AutoResetCacheUserLoginCount();
}