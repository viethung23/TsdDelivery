using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.User.Request;
using TsdDelivery.Application.Models.User.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Interface;

public interface IUserService
{
    public Task<OperationResult<List<UserResponse>>> GetAllUsers();
    public Task<OperationResult<User>> Register(UserCreateUpdate request);
    public Task<OperationResult<UserLoginResponse>> Login(LoginRequest query);
    public Task<OperationResult<UserResponse>> DeleteUser(Guid id);
    public Task<OperationResult<UserResponse>> UploadImage(Guid id,IFormFile blob);
    public Task<OperationResult<UserResponse>> GetUserById(Guid id);
    public Task<OperationResult<UserResponse>> DisableUser(Guid userId);
    public Task<OperationResult<UserResponse>> ActiveUser(Guid userId);
    public Task<OperationResult<UserResponse>> ForgotPassword(string email);
    public Task<OperationResult<UserResponse>> ResetPassword(ResetPasswordRequest request);
}
