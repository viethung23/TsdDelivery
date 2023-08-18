namespace TsdDelivery.Domain.Entities;

public class ShippingRate : BaseEntity
{
    public int KmFrom { get; set; }
    public int KmTo { get; set;}
    public decimal Price { get; set; }

    public Guid? ServiceId { get; set; }
    public Service? Service { get; set; }
}
