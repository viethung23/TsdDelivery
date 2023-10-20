using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class ServiceRepository : GenericRepository<Service>,IServiceRepository
{
    public ServiceRepository(AppDbContext dbContext
        ,ICurrentTime currentTime
        ,IClaimsService claimsService) 
        : base(dbContext,claimsService,currentTime)
    {
    }
    
    // to do 
}