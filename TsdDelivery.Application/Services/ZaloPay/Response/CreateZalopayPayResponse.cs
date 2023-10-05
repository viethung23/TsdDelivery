namespace TsdDelivery.Application.Services.ZaloPay.Response;

public class CreateZalopayPayResponse
{
    public int return_code { get; set; }
    public string return_message { get; set; } = string.Empty;
    public string order_url { get; set; } = string.Empty;
}