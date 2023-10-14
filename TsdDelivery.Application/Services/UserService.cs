using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Mail;
using TsdDelivery.Application.Models.User.Request;
using TsdDelivery.Application.Models.User.Response;
using TsdDelivery.Application.Utils;
using TsdDelivery.Domain.Entities;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTime _currentTime;
    private readonly IBlobStorageAzureService _blobStorageAzureService;
    private readonly AppConfiguration _appConfiguration;
    private readonly IClaimsService _claimsService;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly IMailService _mailService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public UserService(IUnitOfWork unitOfWork, ICurrentTime currentTime
        ,IBlobStorageAzureService blobStorageAzureService
        ,AppConfiguration appConfiguration,IClaimsService claimsService
        ,IMapper mapper,IConnectionMultiplexer redisConnection
        ,IMailService mailService,IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _currentTime = currentTime;
        _blobStorageAzureService = blobStorageAzureService;
        _appConfiguration = appConfiguration;
        _claimsService = claimsService;
        _mapper = mapper;
        _redisConnection = redisConnection;
        _mailService = mailService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResult<List<UserResponse>>> GetAllUsers()
    {
        var result = new OperationResult<List<UserResponse>>();
        try
        {
            // config property include 
            string[] includes = { "Role","Vehicles"}; 

            var users = await _unitOfWork.UserRepository.GetAllAsync(includes);
            
            result.Payload = _mapper.Map<List<UserResponse>>(users);
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
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetUserByPhoneNumber(command.PhoneNumber);
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(Guid.Parse(command.RoleId));
                var entity = new User();
                if (users.Count() > 0)
                {
                    if(users.Any(x =>x.RoleId.ToString() == command.RoleId))
                    {
                        result.AddError(ErrorCode.ValidationError, string.Format("Account already exists with phone : [{0}] and role : [{1}]", command.PhoneNumber,role.RoleName));
                        return result;
                    }
                    else
                    {
                        entity.Email = command.Email;
                        entity.FullName = command.FullName;
                        entity.PhoneNumber = command.PhoneNumber;
                        entity.PasswordHash = command.Password;
                        entity.Role = role;
                    }
                }
                else
                {
                    entity.Email = command.Email;
                    entity.FullName = command.FullName;
                    entity.PhoneNumber = command.PhoneNumber;
                    entity.PasswordHash = command.Password;
                    entity.Role = role;
                }
                if (role!.RoleName.ToUpper() == "DRIVER")
                {
                    entity.DriverStatus = DriverStatus.Available;
                }            
                var u = await _unitOfWork.UserRepository.AddAsync(entity);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess)
                {
                    result.AddUnknownError("Can not save to database");
                }
                if (u.Role.RoleName == "DRIVER")
                {
                    var wallet = new Wallet()
                    {
                        Balance = 0M,
                        Debt = 0M,
                        User = u
                    };
                    var w = await _unitOfWork.WalletRepository.AddAsync(wallet);
                    await _unitOfWork.SaveChangeAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                result.AddUnknownError(ex.Message);
                await transaction.RollbackAsync();
            }
            finally
            {
                _unitOfWork.Dispose();
            }
            return result;
        }
    }
    

    public async Task<OperationResult<UserLoginResponse>> Login(LoginRequest query)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            var roleId = Guid.Parse(query.RoleId);
            var user = await _unitOfWork.UserRepository.GetUserByPhoneNumberAndRoleId(query.PhoneNumber, roleId);
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
            if (user is null)
            {
                result.AddError(ErrorCode.IdentityUserDoesNotExist, string.Format("The Phone number [{0}] not exit With role [{1}]", query.PhoneNumber,role.RoleName));
                return result;
            }
            if (user.IsDeleted == true)
            {
                result.AddError(ErrorCode.ServerError, "Your account has been disabled");
                return result;
            }
            if (user.PasswordHash != query.Password)
            {
                result.AddError(ErrorCode.IncorrectPassword, "IncorrectPassword");
                return result;
            }
            if (role!.RoleName == "DRIVER" && user.Vehicles.Count == 0)
            {
                result.AddError(ErrorCode.ServerError, "Your account has not registered a vehicle with the system");
                return result;
            }
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
                RoleName = user.Role.RoleName,
                Token = token
            };
            result.Payload = userLoginResponse;
            
            // add user vao redis cache (tính những người có tk nào đã đăng nhập)
            /*IDatabase redisDb = _redisConnection.GetDatabase();
            string key = "user_logins_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd");
            redisDb.SetAdd(key, user.Id.ToString());*/
            
            /*IDatabase redisDb = _redisConnection.GetDatabase();
            string loginCountKey = "login_count_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd");
            redisDb.StringIncrement(loginCountKey);*/
            
            // add user vao bang userLogin
            var userLogin = new UserLogin() { UserId = user.Id };
            await _unitOfWork.UserLoginRepository.AddAsync(userLogin);
            await _unitOfWork.SaveChangeAsync();
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
            var user = await _unitOfWork.UserRepository.GetSingleByCondition(u => u.Id == id, new[] { "Role" });
            var imageUrl = await _blobStorageAzureService.SaveImageAsync(blob);
            user.AvatarUrl = imageUrl;
            await _unitOfWork.UserRepository.Update(user);

            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Can not save Role to Database");
            }

            var userResponse = new UserResponse()
            {
                Id = user.Id,
                AvatarUrl = imageUrl,
                CreatedBy = user.CreatedBy,
                CreationDate = user.CreationDate,
                Email = user.Email,
                FullName = user.FullName,
                ModificationDate = user.ModificationDate,
                PhoneNumber = user.PhoneNumber,
                RoleName = user.Role.RoleName
            };

            result.Payload = userResponse;
        }
        catch (InvalidOperationException e)
        {
            result.AddUnknownError($"Not Found by ID: [{id}]");
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        return result;

    }

    public async Task<OperationResult<UserResponse>> GetUserById(Guid id)
    {
        var result = new OperationResult<UserResponse>();
        try
        {
            //var user = await _unitOfWork.UserRepository.GetSingleByCondition(x => x.Id == id, new[] {"Role"} );
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            var userResponse = new UserResponse
            {
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                CreatedBy= user.CreatedBy,
                CreationDate = user.CreationDate,
                Email = user.Email,
                Id = user.Id,
                ModificationDate = user.ModificationDate,
                PhoneNumber = user.PhoneNumber,
                RoleName = "",                    //user.Role == null ? user.Role.RoleName : ""
                IsDelete = user.IsDeleted
            };

            result.Payload = userResponse;
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

    public async Task<OperationResult<UserResponse>> DisableUser(Guid userId)
    {
        var result = new OperationResult<UserResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            user!.IsDeleted = true;
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, $"Can not disable User with id: {userId}");
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

    public async Task<OperationResult<UserResponse>> ActiveUser(Guid userId)
    {
        var result = new OperationResult<UserResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user.IsDeleted == false)
            {
                result.AddError(ErrorCode.ServerError,"This user is in active state");
                return result;
            }
            user!.IsDeleted = false;
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, $"Can not Active User with id: {userId}");
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

    public async Task<OperationResult<UserResponse>> ForgotPassword(string email)
    {
        var result = new OperationResult<UserResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetSingleByCondition(x => x.Email == email);
            var requestContext = _httpContextAccessor?.HttpContext?.Request;
            var clientHost = requestContext?.Headers["X-Client-Host"].ToString();
            
            IDatabase cache = _redisConnection.GetDatabase();
            var key = StringUtils.RandomString();
            cache.StringSet(key, user.Id.ToString(), TimeSpan.FromMinutes(20));
            string body = await _mailService.GetEmailTemplateForgotPassword("forgotPassword",clientHost,key, user);
            MailData mailData = new MailData(
                new List<string> { email },
                "WELCOME TO TSD PROJECT", 
                body
            );
            bool sendResult = await _mailService.SendAsync(mailData, new CancellationToken());
            if (!sendResult)
            {
                result.AddError(ErrorCode.ServerError,"Send Email fail");
                return result;
            }
        }
        catch (InvalidOperationException)
        {
            result.AddUnknownError($"Not found email:[{email}]");
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

    public async Task<OperationResult<UserResponse>> ResetPassword(ResetPasswordRequest request)
    {
        var result = new OperationResult<UserResponse>();
        try
        {
            if (request.Password != request.ConfirmPassword)
            {
                result.AddError(ErrorCode.ServerError, "password and confirmPassword not match");
                return result;
            }
            IDatabase cache = _redisConnection.GetDatabase();
            var id = cache.StringGet(request.Code);
            if (!id.HasValue)
            {
                result.AddError(ErrorCode.ServerError, "The code does not exist or has expired");
                return result;
            }
            var user = await _unitOfWork.UserRepository.GetByIdAsync(Guid.Parse(id.ToString()));
            user.PasswordHash = request.Password;
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (isSuccess)
            {
                cache.KeyDelete(request.Code);
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