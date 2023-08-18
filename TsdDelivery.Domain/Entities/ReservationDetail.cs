namespace TsdDelivery.Domain.Entities;

public class ReservationDetail
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid ServiceId { get; set; }

    public Reservation Reservation { get; set; }
    public Service Service { get; set; }
}
