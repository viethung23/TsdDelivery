using TsdDelivery.Application.Models.Reservation.DTO;

namespace TsdDelivery.Application.Models.Reservation.Response;

public class CalculatePriceResponse
{
    public decimal Distance { get; set; }
    public decimal TotalAmount { get; set; }
}