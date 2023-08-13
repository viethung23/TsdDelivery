using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Users;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<List<User>>> GetAllUsers()
    {
        var result = new OperationResult<List<User>>();

        var user = await _unitOfWork.UserRepository.GetAllAsync();

        if(user is null)
        {
            result.AddError(ErrorCode.NotFound
                ,string.Format($"not found {0}", new string("hahaha")));
        }

        result.Payload = user;
        return result;
    }

    public async Task<OperationResult<User>> Register(UserCreateCommand command)
    {
        var result = new OperationResult<User>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByPhoneNumber(command.PhoneNumber);

            if (user is not null)
            {
                result.AddError(ErrorCode.ValidationError, string.Format("The Phone Number {0} is duplicate", command.PhoneNumber));
            }

            var entiy = new User
            {
                Email = command.Email,
                FullName = command.FullName,
                PhoneNumber = command.PhoneNumber,
                PasswordHash = command.Password,
            };

            await _unitOfWork.UserRepository.AddAsync(entiy);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;

            if (!isSuccess)
            {
                result.AddUnknownError("Can not save to database");
            }
        }
        catch(Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;

    }

    public async Task<OperationResult<UserLoginQueryResponse>> Login(UserLoginQuery query)
    {
        var result = new OperationResult<UserLoginQueryResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByPhoneNumber(query.PhoneNumber);
            if (user is null)
            {
                result.AddError(ErrorCode.IdentityUserDoesNotExist, string.Format("The Phone number {0} not exit", query.PhoneNumber));
            }
            if (user.PasswordHash != query.Password)
            {
                result.AddError(ErrorCode.IncorrectPassword, "IncorrectPassword");
            }

            // To do call token generator 

            var userLoginResponse = new UserLoginQueryResponse()
            {
                Id = user.Id,
                AvatarUrl = user.AvatarUrl,
                CreatedBy = user.CreatedBy,
                CreationDate = user.CreationDate,
                Email = user.Email,
                FullName = user.FullName,
                ModificationDate = user.ModificationDate,
                PhoneNumber = user.PhoneNumber,
                Token = "day la token tam thoi de test"
            };
            result.Payload = userLoginResponse;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }
}
