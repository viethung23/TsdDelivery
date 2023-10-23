using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface.V1;

namespace TsdDelivery.Api.Controllers.V2;

[Route("api/v{version:apiVersion}/roles")] 
[ApiController]
[ApiVersion("2.0")]
public class RoleController : BaseController
{
    
    public RoleController(IRoleService roleService)
    {
        
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        
        return Ok("HAHAHAHHAHA");
    }
}