using TsdDelivery.Application.Models.Service.Response;

namespace TsdDelivery.Application.Models.VehicleType.Response;

public class VehicleTypeDetailResponse
{
    public Guid Id { get; set; }
    public string VehicleTypeName { get; set; }
    public string? VehicleTypeImage { get; set; }
    public string? Description { get; set; }
    public List<ServiceResponse?> Services { get; set; }
}