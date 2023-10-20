using Microsoft.EntityFrameworkCore;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;
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
    public Task<Reservation> GetReservationDetail(Guid id)
    {
        var reservation = _dbSet.Where(x => x.Id == id)
            .Include(x => x.User)
            .Include(x => x.Driver)
            .Include(x => x.reservationDetails)
            .ThenInclude(x => x.Service)
            .ThenInclude(x => x.VehicleType).FirstAsync();

        return reservation;
    }

    public Task<List<Reservation>> GetReservationHistoryForUser(Guid userId)
    {
        var reservation = _dbSet.Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreationDate)
            .Include(x => x.reservationDetails)
            .ThenInclude(x => x.Service)
            .ThenInclude(x => x.VehicleType).ToListAsync();
        return reservation;
    }

    public Task<List<Reservation>> GetReservationHistoryForDriver(Guid driverId)
    {
        var reservation = _dbSet.Where(x => x.DriverId == driverId)
            .OrderByDescending(x => x.CreationDate)
            .Include(x => x.reservationDetails)
            .ThenInclude(x => x.Service)
            .ThenInclude(x => x.VehicleType).ToListAsync();
        return reservation;
    }
}