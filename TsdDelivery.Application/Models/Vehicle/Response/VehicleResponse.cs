using TsdDelivery.Application.Models.VehicleType.DTO;

namespace TsdDelivery.Application.Models.Vehicle.Response;

public class VehicleResponse
{
    public Guid Id { get; set; }
    public string NameVehicle { get; set; }
    public string? Description { get; set; }
    public string VehicleLoad { get; set; }
    public string LicensePlate { get; set; }
    public string? ImageVehicle { get; set; }
    public VehicleTypeDto VehicleTypeDto { get; set; }
}
