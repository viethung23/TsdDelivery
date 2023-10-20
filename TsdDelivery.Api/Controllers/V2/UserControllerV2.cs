using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface.V1;

namespace TsdDelivery.Api.Controllers.V2;

public class UserControllerV2 : BaseControllerV2
{
    private readonly IUserService _userService;
    public UserControllerV2(IUserService userService)
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