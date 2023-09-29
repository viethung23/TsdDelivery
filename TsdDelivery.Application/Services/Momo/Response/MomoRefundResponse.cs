namespace TsdDelivery.Application.Services.Momo.Response;

public class MomoRefundResponse
{
    public string partnerCode { get; set; } = string.Empty;
    public string orderId { get; set; } = string.Empty;
    public string requestId { get; set; } = string.Empty;
    public long amount { get; set; }
    public long transId { get; set; } 
    public string resultCode { get; set; } 
    public string message { get; set; } = string.Empty;
    public long responseTime { get; set; } 
}