using TsdDelivery.Application.Models.Reservation.DTO;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Models.Reservation.Response;

public class ReservationResponse
{
    public string SendLocation { get; set; }
    public string ReciveLocation { get; set; }
    public string RecipientName { get; set; }
    public string RecipientPhone { get; set;}
    public decimal Distance { get; set; }
    public DateTime PickUpDateTime { get; set; }
    public GoodsDto GoodsDto { get; set; }
    public decimal TotallPrice { get; set; }
    public ReservationStatus ReservationStatus { get; set; }
}