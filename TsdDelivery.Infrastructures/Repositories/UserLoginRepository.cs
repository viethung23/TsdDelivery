using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class UserLoginRepository : GenericRepository<UserLogin>,IUserLoginRepository
{
    public UserLoginRepository(AppDbContext context
        ,ICurrentTime currentTime
        ,IClaimsService claimsService) 
        : base(context,claimsService,currentTime)
    {
        // To do 
    }
}