using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.User.Request;
using TsdDelivery.Application.Models.User.Response;
using TsdDelivery.Application.Utils;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTime _currentTime;
    private readonly AppConfiguration _appConfiguration;
    private readonly BlobContainerClient _fileContainer;
    public UserService(IUnitOfWork unitOfWork, ICurrentTime currentTime, AppConfiguration appConfiguration)
    {
        _unitOfWork = unitOfWork;
        _currentTime = currentTime;
        _appConfiguration = appConfiguration;

        var credential = new StorageSharedKeyCredential(_appConfiguration.FileService.StorageAccount, _appConfiguration.FileService.Key);
        var blobUri = $"https://{_appConfiguration.FileService.StorageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri),credential);
        _fileContainer = blobServiceClient.GetBlobContainerClient("files");

    }

    public async Task<OperationResult<List<User>>> GetAllUsers()
    {
        var result = new OperationResult<List<User>>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetAllAsync();

            if (user is null)
            {
                result.AddError(ErrorCode.NotFound
                    , string.Format($"not found {0}", new string("hahaha")));
                return result;
            }
            result.Payload = user;
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

    public async Task<OperationResult<User>> Register(UserCreateUpdate command)
    {
        var result = new OperationResult<User>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByPhoneNumber(command.PhoneNumber);

            if (user is not null)
            {
                result.AddError(ErrorCode.ValidationError, string.Format("The Phone Number {0} is duplicate", command.PhoneNumber));
                return result;
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

    public async Task<OperationResult<UserLoginResponse>> Login(LoginRequest query)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByPhoneNumber(query.PhoneNumber);
            if (user is null)
            {
                result.AddError(ErrorCode.IdentityUserDoesNotExist, string.Format("The Phone number {0} not exit", query.PhoneNumber));
                return result;
            }
            if (user.PasswordHash != query.Password)
            {
                result.AddError(ErrorCode.IncorrectPassword, "IncorrectPassword");
                return result;
            }

            // To do call token generator 
            string? token = user.GenerateJsonWebToken(_appConfiguration.JwtSettings, _currentTime.GetCurrentTime());


            var userLoginResponse = new UserLoginResponse()
            {
                Id = user.Id,
                AvatarUrl = user.AvatarUrl,
                CreatedBy = user.CreatedBy,
                CreationDate = user.CreationDate,
                Email = user.Email,
                FullName = user.FullName,
                ModificationDate = user.ModificationDate,
                PhoneNumber = user.PhoneNumber,
                Token = token
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

    public async Task<OperationResult<UserResponse>> DeleteUser(Guid id)
    {
        var result = new OperationResult<UserResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            await _unitOfWork.UserRepository.Delete(user);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Can not save Role to Database");
            }
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

    public async Task<OperationResult<UserResponse>> UploadImage(Guid id, IFormFile blob)
    {
        var result = new OperationResult<UserResponse>();

        try
        {

            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            

            BlobClient client = _fileContainer.GetBlobClient(blob.FileName);

            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            var userResponse = new UserResponse()
            {
                Id = user.Id,
                AvatarUrl = client.Uri.AbsoluteUri,
                CreatedBy = user.CreatedBy,
                CreationDate = user.CreationDate,
                Email = user.Email,
                FullName = user.FullName,
                ModificationDate = user.ModificationDate,
                PhoneNumber = user.PhoneNumber,
            };

            result.Payload = userResponse;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        return result;

    }
}
