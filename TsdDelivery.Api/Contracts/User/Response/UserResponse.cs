namespace TsdDelivery.Api.Contracts.User.Response;

public record UserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreationDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModificationDate { get; set; }

}
