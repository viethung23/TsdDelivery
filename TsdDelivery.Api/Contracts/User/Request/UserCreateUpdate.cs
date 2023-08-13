namespace TsdDelivery.Api.Contracts.User.Request;

public record UserCreateUpdate
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}
