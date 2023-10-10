namespace TsdDelivery.Domain.Entities;

public class UserLogin : BaseEntity
{
    public Guid? UserId { get; set; }
    
    public User? loggedInUser { get; set; }
}