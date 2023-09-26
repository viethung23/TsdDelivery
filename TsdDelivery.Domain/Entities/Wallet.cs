namespace TsdDelivery.Domain.Entities;

public class Wallet : BaseEntity
{
    public decimal Balance { get; set; }
    public decimal Debt { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }
    
    public ICollection<Transaction>? Transactions { get; set; }
}