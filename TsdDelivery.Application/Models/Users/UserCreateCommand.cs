namespace TsdDelivery.Application.Models.Users;

public record UserCreateCommand 
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}
