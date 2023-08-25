using System.ComponentModel.DataAnnotations;

namespace TsdDelivery.Application.Models.Reservation.Request;

public class CalculatePriceRequest
{
    [Required]
    public decimal Distance { get; set; }
    [Required]
    public List<Guid> ServiceIds { get; set; }
}