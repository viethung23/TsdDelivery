namespace TsdDelivery.Application.Models.Vehicle.DTO;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string NameVehicle { get; set; }
    public string? Description { get; set; }
    public string VehicleLoad { get; set; }
    public string LicensePlate { get; set; }
    public string? ImageVehicle { get; set; }
}