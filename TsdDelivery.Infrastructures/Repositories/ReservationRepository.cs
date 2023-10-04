using Microsoft.EntityFrameworkCore;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class ReservationRepository : GenericRepository<Reservation>,IReservationRepository
{
    public ReservationRepository(AppDbContext context
        ,IClaimsService claimsService
        ,ICurrentTime currentTime) 
        : base(context,claimsService,currentTime)
    {
    }
    // to do
    public async Task<Reservation> GetReservationDetail(Guid id)
    {
        var reservation = _dbSet.Where(x => x.Id == id)
            .Include(x => x.Driver)
            .Include(x => x.reservationDetails)
            .ThenInclude(x => x.Service)
            .ThenInclude(x => x.VehicleType).First();

        return await Task.FromResult(reservation);
    }

    public Task<List<Reservation>> GetReservationHistoryForUser(Guid userId)
    {
        var reservation = _dbSet.Where(x => x.UserId == userId)
            .Include(x => x.reservationDetails)
            .ThenInclude(x => x.Service)
            .ThenInclude(x => x.VehicleType).ToListAsync();
        return reservation;
    }
}