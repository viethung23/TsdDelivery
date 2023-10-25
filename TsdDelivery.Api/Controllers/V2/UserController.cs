using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface.V2;
using TsdDelivery.Application.Models.User.DTO;
using TsdDelivery.Application.Models.User.Request;


namespace TsdDelivery.Api.Controllers.V2;

[Route("api/v{version:apiVersion}/users")] 
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
    //[Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetUsers()
    {
        var response = await _userService.GetUsers();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var response = await _userService.GetUser(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("{userId}/wallets")]
    [Authorize]
    public async Task<IActionResult> GetWalletByUser(Guid userId)
    {
        var response = await _userService.GetWalletByUser(userId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("{userId}/wallets/transactions")]
    //[Authorize]
    public async Task<IActionResult> GetTransactionByUser(Guid userId)
    {
        var response = await _userService.GetTransactionByUser(userId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Register([FromBody] UserCreateUpdate request)
    {
        var response = await _userService.Register(request);

        return response.IsError ? HandleErrorResponse(response.Errors) : CreatedAtAction(nameof(GetUser), new {id = response.Payload?.Id},response.Payload);
    }
    
    [HttpPost]
    [Route("login")]
    [ValidateModel]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _userService.Login(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var response = await _userService.DeleteUser(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
    }
    
    [HttpPatch("{userId}/status")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> ChangeStatusUser(Guid userId,[FromBody] StatusAccount status)
    {
        var response = await _userService.ChangeStatusUser(userId,status);
        return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UploadAvatar(Guid id, IFormFile blob)
    {
        var response = await _userService.UploadImage(id, blob);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var response = await _userService.ForgotPassword(email);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Success");
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var response = await _userService.ResetPassword(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Success");
    }
}