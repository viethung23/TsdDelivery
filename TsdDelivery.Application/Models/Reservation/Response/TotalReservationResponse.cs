namespace TsdDelivery.Application.Models.Reservation.Response;

public class TotalReservationResponse
{
    public int TotalReservations { get; set; }
    public List<TotalReservationDayByDayResponse> TotalReservationDayByDayResponses { get; set; }
}

public class TotalReservationDayByDayResponse
{
    public DateTime Date { get; set; }
    public int TotalReservations { get; set; }
}