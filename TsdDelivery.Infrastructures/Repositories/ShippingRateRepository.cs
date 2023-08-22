using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class ShippingRateRepository : GenericRepository<ShippingRate>,IShippingRateRepository
{
    public ShippingRateRepository(AppDbContext dbContext
        ,ICurrentTime currentTime
        ,IClaimsService claimsService) 
        : base(dbContext,claimsService,currentTime)
    {
    }
    
    // To do
}