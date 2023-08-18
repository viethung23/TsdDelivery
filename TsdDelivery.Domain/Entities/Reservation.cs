using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Domain.Entities;

public class Reservation : BaseEntity
{
    public string SendLocation { get; set; }
    public string ReciveLocation { get; set; }
    public string RecipientName { get; set; }
    public string RecipientPhone { get; set;}
    public DateTime PickUpDateTime { get; set; }
    public Goods Goods { get; set; }
    public decimal TotallPrice { get; set; }
    public ReservationStatus ReservationStatus { get; set; }

    public Guid? UserId { get; set; }
    public Guid? DriverId { get; set; }

    public User? User { get; set; }
    public User? Driver { get; set; }

    public ICollection<ReservationDetail?> reservationDetails { get; set; }
}
