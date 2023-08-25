using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class ReservationDetailRepository : GenericRepository<ReservationDetail>,IReservationDetailRepository
{
    public ReservationDetailRepository(AppDbContext dbContext
        ,IClaimsService claimsService
        ,ICurrentTime currentTime) : base(dbContext,claimsService,currentTime)
    {
    }
    // to do
}