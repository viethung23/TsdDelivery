namespace TsdDelivery.Application.Interface;

public interface IClaimsService
{
    public Guid GetCurrentUserId { get; }
    public string? IpAddress { get; }
    public string? Role { get; }
}
