namespace TsdDelivery.Domain.Entities.Enums;

public enum ReservationStatus
{
    AwaitingPayment,
    AwaitingDriver,
    Cancelled,
    Completed,
    OnTheWayToPickupPoint,
    InDelivery
}
