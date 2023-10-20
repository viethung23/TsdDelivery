namespace TsdDelivery.Application.Interface.V1;

public interface IBackgroundService
{
    Task AutoCancelReservationWhenOverAllowPaymentTime(Guid reservationId);
    Task AutoCancelAndRefundWhenOverAllowTimeAwaitingDriver(string orderId,string transId);
    Task AutoResetCacheUserLoginCount();
}