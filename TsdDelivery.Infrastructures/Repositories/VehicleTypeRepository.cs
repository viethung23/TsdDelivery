using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class VehicleTypeRepository : GenericRepository<VehicleType>,IVehicleTypeReposiory
{
    public VehicleTypeRepository(AppDbContext appDbContext
        ,IClaimsService claimsService
        ,ICurrentTime currentTime) : base(appDbContext , claimsService , currentTime)
    {
    }

    //To do
}
