using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    public Task<Role?> GetRoleByRoleName(string roleName);
}
