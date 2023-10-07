using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Models.Transaction.Response;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Description { get; set; }
    public TransactionType TransactionType { get; set; }
    public DateTime CreationDate { get; set; }
}