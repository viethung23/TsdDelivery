using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class VehicleRepository : GenericRepository<Vehicle>,IVehicleRepository
{
    public VehicleRepository(AppDbContext appDbContext
        ,ICurrentTime currentTime
        ,IClaimsService claimsService) 
        : base(appDbContext,claimsService, currentTime)
    {
    }

    // to do
}
