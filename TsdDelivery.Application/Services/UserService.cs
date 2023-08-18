using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.User.Request;
using TsdDelivery.Application.Models.User.Response;
using TsdDelivery.Application.Repositories;
using TsdDelivery.Application.Utils;
using TsdDelivery.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        _fileContainer = blobServiceClient.GetBlobContainerClient("images");

    }

    public async Task<OperationResult<List<UserResponse>>> GetAllUsers()
    {
        var result = new OperationResult<List<UserResponse>>();
        try
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();

            if (users is null)
            {
                result.AddError(ErrorCode.NotFound
                    , string.Format($"not found {0}", new string("hahaha")));
                return result;
            }

            var listUserResponse = new List<UserResponse>();
            foreach (var user in users)
            {
                //var roles = await _unitOfWork.UserRoleRepository.GetRoleByUser(user.Id);

                var userResponse = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    AvatarUrl = user.AvatarUrl,
                    CreatedBy = user.CreatedBy,
                    CreationDate = user.CreationDate,
                    FullName = user.FullName,
                    ModificationDate = user.ModificationDate,
                    PhoneNumber = user.PhoneNumber,
                    //Roles = roles.Select(r => r.RoleName).ToList()
                    Roles = new List<string?> { "haha","haha"}
                    
                };
                listUserResponse.Add(userResponse);
            }
            result.Payload = listUserResponse;
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

            var u = await _unitOfWork.UserRepository.AddAsync(entiy);
            var roleUSER = await _unitOfWork.RoleRepository.GetRoleByRoleName("USER");
            //var userRole = _unitOfWork.UserRoleRepository.AddRoleForUser(u, roleUSER);
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
            //var roles = (await _unitOfWork.UserRoleRepository.GetRoleByUser(user.Id)).Select(r => r.RoleName).ToList();
        /// =========================================================================================================================== here 
            /*if(roles.Any(x => x.Equals("DRIVER")))
            {
                result.AddError(ErrorCode.ValidationError, "Your account is not registered as a driver!");
                return result;
            }*/
            // To do call token generator 
            //string? token = user.GenerateJsonWebToken(_appConfiguration.JwtSettings, _currentTime.GetCurrentTime(),roles);
            string? token = user.GenerateJsonWebToken(_appConfiguration.JwtSettings, _currentTime.GetCurrentTime(),new List<string?> { "haha"});


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
                await client.UploadAsync(data, httpHeaders: new BlobHttpHeaders { ContentType = "image/png" }, conditions: null);
            }

            user.AvatarUrl = client.Uri.AbsoluteUri;
            await _unitOfWork.UserRepository.Update(user);

            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Can not save Role to Database");
            }
            //var roles = await _unitOfWork.UserRoleRepository.GetRoleByUser(user.Id);
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
                //Roles = roles.Select(r => r.RoleName).ToList()
                Roles = new List<string?> { "haha"}
            };

            result.Payload = userResponse;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        return result;

    }

    public async Task<OperationResult<UserResponse>> RegisterDriver(Guid idDriver)
    {
        var result = new OperationResult<UserResponse>();
        try
        {
            /*var roles = await _unitOfWork.UserRoleRepository.GetRoleByUser(idDriver);
            if(roles.Any(r => r.RoleName.Equals("DRIVER") || r.RoleName.Equals("ADMIN")))
            {
                result.AddError(ErrorCode.ValidationError, "you are DRIVER already!");
                return result;
            }*/


            var role = await _unitOfWork.RoleRepository.GetRoleByRoleName("DRIVER");
            var user = await _unitOfWork.UserRepository.GetByIdAsync(idDriver);
            //await _unitOfWork.UserRoleRepository.AddRoleForUser(user, role);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Can not save Role to Database");
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
}



/* The example to implement 
 
public class FooService
{
    private readonly IUnitOfWork _unitOfWork;
    public FooService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void Bar()
    {
        using (var transaction = _unitOfWork.BeginTransaction())
        {
            try
            {
                _unitOfWork.Users.Add(new UserModel("dummy username"));
                _unitOfWork.SaveChanges();
                _unitOfWork.Addresses.Add(new AddressModel("dummy address"));
                _unitOfWork.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }
}
 
 */