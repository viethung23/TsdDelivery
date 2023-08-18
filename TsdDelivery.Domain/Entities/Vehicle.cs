namespace TsdDelivery.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string NameVehicle { get; set; }
    public string? Description { get; set; }
    public string VehicleLoad { get; set; }
    public string LicensePlate { get; set; }
    public string? ImageVehicle { get; set; }

    public Guid? UserId { get; set; }
    public Guid? VehicleTypeId { get; set; }

    public User? User { get; set; }
    public VehicleType? VehicleType { get; set; }
}
