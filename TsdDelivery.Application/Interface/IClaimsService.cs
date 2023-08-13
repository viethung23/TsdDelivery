namespace TsdDelivery.Application.Interface;

public interface IClaimsService
{
    public Guid GetCurrentUserId { get; }
}
