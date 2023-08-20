using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Role.Request;
using TsdDelivery.Application.Models.Role.Response;
using TsdDelivery.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TsdDelivery.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<RoleResponse>> CreateRole(RoleCreateUpdate request)
    {
        var result = new OperationResult<RoleResponse>();
        try
        {
            var role = await _unitOfWork.RoleRepository.GetRoleByRoleName(request.RoleName);

            if (role is not null)
            {
                result.AddError(ErrorCode.ValidationError, string.Format("The Role Name [{0}] already exit", request.RoleName));
                return result;
            }

            var entity = new Role() { RoleName = request.RoleName };
            await _unitOfWork.RoleRepository.AddAsync(entity);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Can not save Role to Database");
            }
            // here not set data to Payload
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<RoleResponse>> DeleteRole(Guid id)
    {
        var result = new OperationResult<RoleResponse>();
        try
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            await _unitOfWork.RoleRepository.Delete(role);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Can not save Role to Database");
            }
        }
        catch(Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<List<RoleResponse>>> GetAllRoles()
    {
        var result = new OperationResult<List<RoleResponse>>();

        var roles = await _unitOfWork.RoleRepository.GetAllAsync();
        if (roles is null)
        {
            result.AddError(ErrorCode.NotFound
                , "Not found any Roles");
        }
        var list = new List<RoleResponse>();
        foreach(var i in roles)
        {
            var roleResponse = new RoleResponse
            {
                Id = i.Id,
                CreatedBy = i.CreatedBy,
                CreationDate = i.CreationDate,
                ModificationDate = i.ModificationDate,
                RoleName = i.RoleName,
            };
            list.Add(roleResponse);
        }
        result.Payload = list;
        return result;

    }
}
