using System.ComponentModel.DataAnnotations;

namespace TsdDelivery.Application.Models.Vehicle.request;

public class CreateVehicle
{
    [Required]
    public string NameVehicle { get; set; }

    public string? Description { get; set; }
    
    [Required]
    public string VehicleLoad { get; set; }

    [Required]
    public string LicensePlate { get; set; }

    [Required]
    public string VehicleTypeId { get; set; }

    [Required]
    public string UserId { get; set; }
}
