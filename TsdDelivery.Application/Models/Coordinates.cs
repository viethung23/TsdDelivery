using System.ComponentModel.DataAnnotations;

namespace TsdDelivery.Application.Models;

public class Coordinates
{
    [Range(-90, 90, ErrorMessage = "Invalid latitude value")]
    public double? Latitude { get; set; }
    
    [Range(-180, 180, ErrorMessage = "Invalid longitude value")]
    public double? Longitude { get; set; }
}