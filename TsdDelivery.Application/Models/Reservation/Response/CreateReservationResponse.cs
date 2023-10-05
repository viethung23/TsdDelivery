namespace TsdDelivery.Application.Models.Reservation.Response;

public class CreateReservationResponse
{
    public Guid Id { get; set; }
    public string PaymentUrl { get; set; }
    public string? deeplink { get; set; }
}