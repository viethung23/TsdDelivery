using System.ComponentModel.DataAnnotations;

namespace TsdDelivery.Application.Models.Coordinates;

public class CurrentCoordinates
{
    [Range(-90, 90, ErrorMessage = "Invalid latitude value")]
    public double Latitude { get; set; } = 0.0D;

    [Range(-180, 180, ErrorMessage = "Invalid longitude value")]
    public double Longitude { get; set; } = 0.0D;
}