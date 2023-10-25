using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface.V2;
using TsdDelivery.Application.Models.Role.Request;

namespace TsdDelivery.Api.Controllers.V2;

[Route("api/v{version:apiVersion}/roles")] 
[ApiController]
[ApiVersion("2.0")]
public class RoleController : BaseController
{
    private readonly IRoleService _roleService;
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var response = await _roleService.GetAllRoles();

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var response = await _roleService.GetRole(id);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPost]
    //[ValidateModel]
    //[Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> CreateRole(RoleCreateUpdate request)
    {
        var response = await _roleService.CreateRole(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : CreatedAtAction(nameof(GetRole), new {id = response.Payload?.Id},response.Payload);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var response = await _roleService.DeleteRole(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
    }
}