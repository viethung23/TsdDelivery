namespace TsdDelivery.Application.Repositories;

public interface IHangFireRepository
{
    Task<string> DeleteJob(Guid reservationId, string? methodName);
    Task<string> DeleteJob(Guid reservationId);
}