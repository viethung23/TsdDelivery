namespace TsdDelivery.Application.Models.User.Response;

public class UserLoginResponse
/*(
     Guid Id ,
     string FullName ,
     string Email ,
     string PhoneNumber ,
     string? AvatarUrl ,
     DateTime CreationDate ,
     Guid? CreatedBy ,
     DateTime? ModificationDate ,
     string Token 
);*/
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreationDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModificationDate { get; set; }
    public string? Token { get; set; }
}
