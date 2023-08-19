namespace TsdDelivery.Application.Models.User.Request;

public class LoginRequest
{
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string RoleId { get; set; }
}
