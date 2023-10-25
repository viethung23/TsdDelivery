using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Role.Request;
using TsdDelivery.Application.Models.Role.Response;

namespace TsdDelivery.Application.Interface.V2;

public interface IRoleService
{
    public Task<OperationResult<List<RoleResponse>>> GetAllRoles();
    public Task<OperationResult<RoleResponse>> CreateRole(RoleCreateUpdate request);
    public Task<OperationResult<RoleResponse>> DeleteRole(Guid id);
    public Task<OperationResult<RoleResponse>> GetRole(Guid id);
}