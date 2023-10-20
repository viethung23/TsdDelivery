using TsdDelivery.Application.Interface.V1;

namespace TsdDelivery.Application.Services.V1;

public class CurrentTime : ICurrentTime
{
    public DateTime GetCurrentTime() => DateTime.UtcNow.AddHours(7);
}
