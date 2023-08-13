using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Users;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Interface;

public interface IUserService
{
    public Task<OperationResult<List<User>>> GetAllUsers();
    public Task<OperationResult<User>> Register(UserCreateCommand command);
    public Task<OperationResult<UserLoginQueryResponse>> Login(UserLoginQuery query);
}
