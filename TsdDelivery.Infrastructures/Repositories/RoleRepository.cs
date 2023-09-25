using Microsoft.EntityFrameworkCore;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context
        , ICurrentTime currentTime
        , IClaimsService claimsService)
        : base(context, claimsService, currentTime)
    {
        
    }

    public Task<Role?> GetRoleByRoleName(string roleName)
    {
        return  _dbSet.FirstOrDefaultAsync(r => r.RoleName == roleName);
    }
}
