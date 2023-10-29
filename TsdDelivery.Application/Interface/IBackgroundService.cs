namespace TsdDelivery.Application.Interface;

public interface IBackgroundService
{
    Task AutoCancelReservationWhenOverAllowPaymentTime(Guid reservationId);
    Task AutoCancelAndRefundMoMoWhenOverAllowTimeAwaitingDriver(string paymentMethod,string orderId,string transId);
    Task AutoCancelAndRefundPayPalWhenOverAllowTimeAwaitingDriver(string paymentMethod,string orderId,string captureId);
    Task AutoResetCacheUserLoginCount();
}