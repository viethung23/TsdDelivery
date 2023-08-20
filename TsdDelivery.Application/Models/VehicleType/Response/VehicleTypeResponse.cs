namespace TsdDelivery.Application.Models.VehicleType.Response;

public class VehicleTypeResponse
{
    public Guid Id { get; set; }
    public string VehicleTypeName { get; set; }
    public string? VehicleTypeImage { get; set; }
    public string? Description { get; set; }
}
