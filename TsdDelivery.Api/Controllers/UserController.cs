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
    [Authorize(Policy = "RequireAdminRole")]
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /Driver
    ///     {
    ///        "phoneNumber": "0987656765",
    ///        "password": "123",
    ///        "roleId": "1cb47d53-a12c-4fd6-a4cc-08dba3d1f4f1"
    ///     }
    ///
    ///     POST /User
    ///     {
    ///        "phoneNumber": "2222222222",
    ///        "password": "123",
    ///        "roleId": "0170ca46-f56b-4575-a4cb-08dba3d1f4f1"
    ///     }
    ///
    /// </remarks> 
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

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> DisableUser(Guid userId)
    {
        var response = await _userService.DisableUser(userId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok($"Disable Success User with id = {userId}");
    }
    
    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> ActiveUser(Guid userId)
    {
        var response = await _userService.ActiveUser(userId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok($"Active Success User with id = {userId}");
    }
    
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var response = await _userService.ForgotPassword(email);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Success");
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var response = await _userService.ResetPassword(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Success");
    }
}
