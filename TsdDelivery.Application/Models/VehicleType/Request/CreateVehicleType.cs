using System.ComponentModel.DataAnnotations;

namespace TsdDelivery.Application.Models.VehicleType.Request;

public class CreateVehicleType
{
    [Required]
    public string VehicleTypeName { get; set; }

    public string? Description { get; set; }
}
