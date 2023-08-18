using Microsoft.EntityFrameworkCore;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    /*private readonly AppDbContext _Context;
    public UserRoleRepository(AppDbContext appDbContext)
    {
        _Context = appDbContext;
    }

    public async Task AddRoleForUser(User User, Role Role)
    {
        await _Context.UserRoles.AddAsync(new UserRole() { User = User,Role = Role }) ;
    }

    public async Task<List<Role?>> GetRoleByUser(Guid UserId)
    {
        return await _Context.UserRoles.Where(ur => ur.UserId == UserId).Select(x => x.Role).ToListAsync();
    }

    public async Task<List<User?>> GetUsersByRole(Guid RoleId)
    {
        return await _Context.UserRoles.Where(ur => ur.RoleId == RoleId).Select(x => x.User).ToListAsync();
    }*/
}
