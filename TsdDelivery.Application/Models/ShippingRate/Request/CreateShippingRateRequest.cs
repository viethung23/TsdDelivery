namespace TsdDelivery.Application.Models.ShippingRate.Request;

public class CreateShippingRateRequest
{
    public int KmFrom { get; set; }
    public int KmTo { get; set;}
    public decimal Price { get; set; }
    public Guid ServiceId { get; set; }
}