using Microsoft.EntityFrameworkCore;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;
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

    public Task<List<User?>> GetUserByPhoneNumber(string phoneNumber)
    {
        return _dbSet.Where(u => u.PhoneNumber.Equals(phoneNumber)).ToListAsync();
    }

    public Task<User?> GetUserByPhoneNumberAndRoleId(string phoneNumeber, Guid roleId)
    {
        //return _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber.Equals(phoneNumeber) && u.RoleId.Equals(roleId));
        return _dbSet.Where(x => x.PhoneNumber.Equals(phoneNumeber) && x.RoleId.Equals(roleId)).Include(x => x.Vehicles)
            .FirstOrDefaultAsync();
    }

    public Task<User> GetDriverDetail(Guid driverId)
    {
       return _dbSet.Where(x => x.Id == driverId)
            .Include(x => x.Wallet)
            .Include(x => x.Vehicles)
                .ThenInclude(x => x.VehicleType).FirstAsync();
    }

    public Task<int> GetUserCountByRole(Guid roleId)
    {
        return _dbSet.Where(x => x.RoleId == roleId).CountAsync();
    }
}
