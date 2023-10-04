using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Repositories;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<Reservation> GetReservationDetail(Guid id);
    public Task<List<Reservation>> GetReservationHistoryForUser(Guid userId);
}