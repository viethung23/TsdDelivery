namespace TsdDelivery.Domain.Entities;

public class Service : BaseEntity
{
    public string ServiceName { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }

    public Guid? VehicleTypeId {  get; set; }
    public VehicleType? VehicleType { get; set; }
    public ICollection<ShippingRate?> shippingRates { get; set; }
    public ICollection<ReservationDetail?> reservationDetails { get; set; }
} 
