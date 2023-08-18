namespace TsdDelivery.Domain.Entities;

public class VehicleType : BaseEntity
{
    public string VehicleTypeName { get; set; }
    public string? VehicleTypeImage { get; set; }
    public string? Description { get; set; }

    public ICollection<Vehicle?> Vehicles { get; set; }
    public ICollection<Service?> services { get; set; }
}
