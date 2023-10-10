namespace TsdDelivery.Application.Models.DashBoard.Response;

public class UserLoginDayByDay
{
    public DateTime Date { get; set; }
    public long TotalUserLogin { get; set; }
}