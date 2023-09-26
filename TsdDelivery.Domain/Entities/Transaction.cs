namespace TsdDelivery.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedDate { get; set; }
    
    public Guid? WalletId { get; set; }
    public Wallet? Wallet { get; set; }
    public Guid? ReservationId { get; set; }
    public Reservation Reservation { get; set; }
}