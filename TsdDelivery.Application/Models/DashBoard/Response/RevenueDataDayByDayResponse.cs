namespace TsdDelivery.Application.Models.DashBoard.Response;

public class RevenueDataDayByDayResponse
{
    public DateTime Date { get; set; }
    public double TotalRevenueReceived { get; set; }
    public double TotalPayouts { get; set; }
    public double TotalExpensesForDriver { get; set; }
}