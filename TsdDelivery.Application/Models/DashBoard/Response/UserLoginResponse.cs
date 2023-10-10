namespace TsdDelivery.Application.Models.DashBoard.Response;

public class UserLoginResponse
{
    public long TotalUserLogin { get; set; }
    public List<UserLoginDayByDay> UserLoginDayByDays { get; set; }
}