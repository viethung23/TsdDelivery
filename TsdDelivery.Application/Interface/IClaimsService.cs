namespace TsdDelivery.Application.Interface;

public interface IClaimsService
{
    public Guid GetCurrentUserId { get; }
    public string Host { get; }
    public string IpAddress { get; }
}
