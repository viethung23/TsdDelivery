using TsdDelivery.Application.Interface;

namespace TsdDelivery.Application.Services;

public class CurrentTime : ICurrentTime
{
    public DateTime GetCurrentTime() => DateTime.UtcNow;
}
