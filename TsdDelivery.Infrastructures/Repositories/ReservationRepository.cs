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
}