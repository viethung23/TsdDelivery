namespace TsdDelivery.Api.Contracts.User.Request;

public record LoginRequest
{
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}
