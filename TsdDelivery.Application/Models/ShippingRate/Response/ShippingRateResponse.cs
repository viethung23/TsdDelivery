namespace TsdDelivery.Application.Models.ShippingRate.Response;

public class ShippingRateResponse
{
    public Guid Id { get; set; }
    public int KmFrom { get; set; }
    public int KmTo { get; set;}
    public decimal Price { get; set; }
}