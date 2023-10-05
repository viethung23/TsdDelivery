namespace TsdDelivery.Application.Models.Reservation.Response;

public class ReservationHistoryResponse
{
    public Guid Id { get; set; }
    public string SendLocation { get; set; }
    public string ReciveLocation { get; set; }
    public string ReservationStatus { get; set; }
    public string GoodsName { get; set; }
    public string VehicleType { get; set; }
    public DateTime CreationDate { get; set; }
}