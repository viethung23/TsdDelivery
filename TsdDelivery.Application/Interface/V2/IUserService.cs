using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Transaction.Response;
using TsdDelivery.Application.Models.User.DTO;
using TsdDelivery.Application.Models.User.Request;
using TsdDelivery.Application.Models.User.Response;
using TsdDelivery.Application.Models.Wallet.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Interface.V2;

public interface IUserService
{
    public Task<OperationResult<List<UserResponse>>> GetUsers();
    public Task<OperationResult<UserResponse>> Register(UserCreateUpdate request);
    public Task<OperationResult<UserLoginResponse>> Login(LoginRequest query);
    public Task<OperationResult<UserResponse>> DeleteUser(Guid id);
    public Task<OperationResult<UserResponse>> UploadImage(Guid id,IFormFile blob);
    public Task<OperationResult<UserResponse>> GetUser(Guid id);
    public Task<OperationResult<UserResponse>> ChangeStatusUser(Guid userId,StatusAccount statusAccount);
    //public Task<OperationResult<UserResponse>> ActiveUser(Guid userId);
    public Task<OperationResult<UserResponse>> ForgotPassword(string email);
    public Task<OperationResult<UserResponse>> ResetPassword(ResetPasswordRequest request);
    public Task<OperationResult<WalletResponse>> GetWalletByUser(Guid userId);
    public Task<OperationResult<List<TransactionResponse>>> GetTransactionByUser(Guid userId);
}