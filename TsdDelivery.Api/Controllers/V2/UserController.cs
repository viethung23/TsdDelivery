using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface.V1;

namespace TsdDelivery.Api.Controllers.V2;

[Route("api/v{version:apiVersion}/[controller]/[action]")] 
[ApiController]
[ApiVersion("2.0")]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsers();

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}