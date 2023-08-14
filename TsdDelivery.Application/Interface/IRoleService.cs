using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Role.Request;
using TsdDelivery.Application.Models.Role.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Interface;

public interface IRoleService
{
    public Task<OperationResult<List<RoleResponse>>> GetAllRoles();
    public Task<OperationResult<RoleResponse>> CreateRole(RoleCreateUpdate request);
    public Task<OperationResult<RoleResponse>> DeleteRole(Guid id);
}
