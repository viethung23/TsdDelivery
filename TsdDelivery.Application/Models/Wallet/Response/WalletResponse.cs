namespace TsdDelivery.Application.Models.Wallet.Response;

public class WalletResponse
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public decimal Debt { get; set; }
}