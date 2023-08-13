using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using TsdDelivery.Api.Contracts.User.Request;
using TsdDelivery.Api.Contracts.User.Response;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.Users;

namespace TsdDelivery.Api.Controllers;

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

        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        var users = response.Payload.ToList();
        var listUserResponse = new List<UserResponse>();
        foreach(var user in users)
        {
            var userResponse = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                CreatedBy = user.CreatedBy,
                CreationDate = user.CreationDate,
                FullName = user.FullName,
                ModificationDate = user.ModificationDate,
                PhoneNumber = user.PhoneNumber
            };
            listUserResponse.Add(userResponse);
        }
        

        return Ok(listUserResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserCreateUpdate request)
    {
        var userCommand = new UserCreateCommand
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber
        };
        var response = await _userService.Register(userCommand);

        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("SUCCESS");
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var loginQuery = new UserLoginQuery()
        {
            PhoneNumber = request.PhoneNumber,
            Password = request.Password
        };
        var response = await _userService.Login(loginQuery);

        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }


        var userLoginResponse = new UserLoginResponse()
        {
            Id = response.Payload.Id,
            AvatarUrl = response.Payload.AvatarUrl,
            CreatedBy = response.Payload.CreatedBy,
            CreationDate = response.Payload.CreationDate,
            Email = response.Payload.Email,
            FullName = response.Payload.FullName,
            ModificationDate = response.Payload.ModificationDate,
            PhoneNumber = response.Payload.PhoneNumber,
            Token = response.Payload.Token
        };
        return Ok(userLoginResponse);
    }
}
