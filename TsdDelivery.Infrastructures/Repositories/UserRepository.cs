using Microsoft.EntityFrameworkCore;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context
        ,ICurrentTime currentTime
        ,IClaimsService claimsService) 
        : base(context,claimsService,currentTime)
    {
        // To do 
    }

    public Task<User?> GetUserByPhoneNumber(string phoneNumber)
    {
        return _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber.Equals(phoneNumber));
    }
}
