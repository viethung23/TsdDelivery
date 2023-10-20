namespace TsdDelivery.Application.Interface.V1;

public interface IClaimsService
{
    public Guid GetCurrentUserId { get; }
    public string? IpAddress { get; }
    public string? Role { get; }
}
