using System.ComponentModel.DataAnnotations;
using TsdDelivery.Application.Models.ShippingRate.DTO;

namespace TsdDelivery.Application.Models.Service.Request;

public class CreateServiceRequest
{
    [Required]
    public Guid VehicleTypeId { get; set; }
    
    [Required]
    public string ServiceName { get; set; }
    
    [Required]
    [Range(0,5000000)]
    public decimal Price { get; set; }
    
    public string? Description { get; set; }
    public List<ShippingRateDTO?> ShippingRateDtos { get; set; } = new List<ShippingRateDTO?>();
}