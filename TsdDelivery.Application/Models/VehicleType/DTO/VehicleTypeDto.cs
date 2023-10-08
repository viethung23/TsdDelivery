namespace TsdDelivery.Application.Models.VehicleType.DTO;

public class VehicleTypeDto
{
    public Guid Id { get; set; }
    public string VehicleTypeName { get; set; }
    public string? VehicleTypeImage { get; set; }
    public string? Description { get; set; }
}