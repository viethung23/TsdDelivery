namespace TsdDelivery.Application.Interface;

public interface IBackgroundService
{
    Task AutoCancelReservationWhenOverAllowPaymentTime(Guid reservationId);
    
}