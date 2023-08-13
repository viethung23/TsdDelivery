

namespace TsdDelivery.Application.Models.Users;

public record UserLoginQuery
{
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}
