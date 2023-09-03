using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.User.Request;

namespace TsdDelivery.Api.Controllers;

public class UserController : BaseController
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    /// <summary>
    ///  Api for Admin
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsers();

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Register([FromBody] UserCreateUpdate request)
    {
        var response = await _userService.Register(request);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("SUCCESS");
    }
    
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _userService.Login(request);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var response = await _userService.DeleteUser(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Delete Success");
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UploadAvatar(Guid id, IFormFile blob)
    {
        var response = await _userService.UploadImage(id, blob);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var response = await _userService.GetUserById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}
