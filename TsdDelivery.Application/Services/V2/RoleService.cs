using MapsterMapper;
using TsdDelivery.Application.Interface.V2;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Role.Request;
using TsdDelivery.Application.Models.Role.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Services.V2;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<OperationResult<List<RoleResponse>>> GetAllRoles()
    {
        var result = new OperationResult<List<RoleResponse>>();
        var roles = await _unitOfWork.RoleRepository.GetAllAsync();
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

    public async Task<OperationResult<RoleResponse>> CreateRole(RoleCreateUpdate request)
    {
        var result = new OperationResult<RoleResponse>();
        try
        {
            var role = await _unitOfWork.RoleRepository.GetRoleByRoleName(request.RoleName);

            if (role is not null)
            {
                result.AddError(ErrorCode.BadRequest, string.Format("The Role Name [{0}] already exit", request.RoleName));
                return result;
            }

            var entity = new Role() { RoleName = request.RoleName };
            await _unitOfWork.RoleRepository.AddAsync(entity);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Can not save Role to Database");
            }
            result.Payload = _mapper.Map<RoleResponse>(entity);
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

    public async Task<OperationResult<RoleResponse>> GetRole(Guid id)
    {
        var result = new OperationResult<RoleResponse>();
        try
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            result.Payload = _mapper.Map<RoleResponse>(role);
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
}