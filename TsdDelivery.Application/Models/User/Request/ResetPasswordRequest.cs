namespace TsdDelivery.Application.Models.User.Request;

public class ResetPasswordRequest
{
    public string Code { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}