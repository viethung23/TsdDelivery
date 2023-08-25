using System.ComponentModel.DataAnnotations;

namespace TsdDelivery.Application.Models.ShippingRate.Request;

public class CreateShippingRateRequest
{
    [Required]
    [Range(0,100)]
    public int KmFrom { get; set; }
    
    [Required]
    [Range(1,10000)]
    public int KmTo { get; set;}
    
    [Required]
    [Range(0,5000000)]
    public decimal Price { get; set; }
    
    [Required]
    public Guid ServiceId { get; set; }
}