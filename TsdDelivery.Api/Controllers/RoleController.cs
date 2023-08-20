using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.Role.Request;

namespace TsdDelivery.Api.Controllers;


public class RoleController : BaseController
{
    private readonly IRoleService _roleService;
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var response = await _roleService.GetAllRoles();

        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok(response.Payload);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateRole(RoleCreateUpdate request)
    {
        var response = await _roleService.CreateRole(request);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("Success");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var response = await _roleService.DeleteRole(id);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("Delete Success");
    }
}
