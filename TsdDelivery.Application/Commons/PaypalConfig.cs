namespace TsdDelivery.Application.Commons;

public class PaypalConfig
{
    public string ClientId { get; set; }
    public string SecretKey { get; set; }
    public string PayPalUrl { get; set; }
    public string ReturnUrl { get; set; }
    public string CancelUrl { get; set; }
}