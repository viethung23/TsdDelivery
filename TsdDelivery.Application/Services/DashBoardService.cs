using StackExchange.Redis;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.DashBoard.Response;
using TsdDelivery.Domain.Entities;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Services;

public class DashBoardService : IDashBoardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly ICurrentTime _currentTime;
    public DashBoardService(IUnitOfWork unitOfWork,IConnectionMultiplexer connectionMultiplexer,ICurrentTime currentTime)
    {
        _unitOfWork = unitOfWork;
        _redisConnection = connectionMultiplexer;
        _currentTime = currentTime;
    }
    public async Task<OperationResult<UserCountResult>> GetCountPercentUser()
    {
        var result = new OperationResult<UserCountResult>();
        Guid adminRoleId = new Guid("eebc82eb-8893-43a7-a4ca-08dba3d1f4f1");
        Guid userRoleId = new Guid("0170ca46-f56b-4575-a4cb-08dba3d1f4f1");
        Guid driverRoleId = new Guid("1cb47d53-a12c-4fd6-a4cc-08dba3d1f4f1");
        var total = await _unitOfWork.UserRepository.Count();
        var userCount = await _unitOfWork.UserRepository.GetUserCountByRole(userRoleId);
        var driverCount = await _unitOfWork.UserRepository.GetUserCountByRole(driverRoleId);
        var adminCount = await _unitOfWork.UserRepository.GetUserCountByRole(adminRoleId);
        
        var userPercent = (double)userCount / total * 100;
        var driverPercent = (double)driverCount / total * 100;
        var adminPercent = (double)adminCount / total * 100;
        var userCountResult = new UserCountResult()
        {
            AdminPercent = adminPercent,
            AdminCount = adminCount,
            UserPercent = userPercent,
            DriverCount = driverCount,
            UserCount = userCount,
            DriverPercent = driverPercent
        };
        result.Payload = userCountResult;
        return result;
    }

    public async Task<OperationResult<RevenueDataResponse>> GetRevenueDataByTime(DateTime from, DateTime to)
    {
        var result = new OperationResult<RevenueDataResponse>();
        try
        {
            var adminId = new Guid("5f00b441-a792-4d46-eece-08dbbe590f42");
            var include = new[] { "Transactions" };
            var wallet = await _unitOfWork.WalletRepository.GetSingleByCondition(x => x.UserId == adminId,include);
            var trans = wallet.Transactions!.Where(x => x.CreationDate >= from && x.CreationDate <= to.AddDays(1)).ToList();
            // xu ly data
            var test = CalculateTransactionsStatistics(trans);
            var transByDate = trans.GroupBy(x => x.CreationDate.Date).ToDictionary(x => x.Key, g => g.ToList());
            var listDateGroup = new List<RevenueDataDayByDayResponse>();
            foreach (var dateGroup in transByDate)
            {
                var test2 = CalculateTransactionsStatistics(dateGroup.Value);
                var group = new RevenueDataDayByDayResponse
                {
                    Date = dateGroup.Key,
                    TotalRevenueReceived = test2.totalRevenueReceived,
                    TotalPayouts = test2.totalPayouts,
                    TotalExpensesForDriver = test2.totalExpensesForDriver
                };
  
                listDateGroup.Add(group);
            }

            var revenueDataResponse = new RevenueDataResponse()
            {
                TotalRevenueReceived = test.totalRevenueReceived,
                TotalPayouts = test.totalPayouts,
                TotalExpensesForDriver = test.totalExpensesForDriver,
                RevenueDataDayByDayResponses = listDateGroup
            };
            result.Payload = revenueDataResponse;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    /*public async Task<OperationResult<long>> GetUserLoginCount()
    {
        var result = new OperationResult<long>();
        try
        {
            /*IDatabase redisDb = _redisConnection.GetDatabase();
            string key = "user_logins_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd");
            result.Payload = redisDb.SetLength(key);#1#
            
            IDatabase redisDb = _redisConnection.GetDatabase();
            string loginCountKey = "login_count_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd");
            result.Payload = (long)redisDb.StringGet(loginCountKey);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }*/
    
    public async Task<OperationResult<UserLoginResponse>> GetUserLoginCount(DateTime from, DateTime to)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            if (to > _currentTime.GetCurrentTime())
            {
                result.AddError(ErrorCode.ServerError,"'to' cannot be greater than the current time");
                return result;
            }
            var includes = new[] { "loggedInUser" };
            var userLogins = await _unitOfWork.UserLoginRepository.GetMulti(x => x.CreationDate >= from && x.CreationDate <= to.AddDays(1),includes);
            var totalUserlogin = CountUserLogin(userLogins);
            // xu ly data
            var userLoginDay = userLogins.GroupBy(x => x.CreationDate.Date).ToDictionary(x => x.Key, g => g.ToList());
            var list = new List<UserLoginDayByDay>();
            foreach (var group in userLoginDay)
            {
                var count = CountUserLogin(group.Value);
                var userLoginData = new UserLoginDayByDay()
                {
                    Date = group.Key,
                    TotalUserLogin = count
                };
                list.Add(userLoginData);
            }

            var userLoginResponse = new UserLoginResponse()
            {
                TotalUserLogin = totalUserlogin,
                UserLoginDayByDays = list
            };
            result.Payload = userLoginResponse;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<List<VehicleCountResponse>>> GetVehicleByVehicleType()
    {
        var result = new OperationResult<List<VehicleCountResponse>>();
        try
        {
            var includes = new[] { "Vehicles" };
            var vehicleTypes = await _unitOfWork.VehicleTypeReposiory.GetAllAsync(includes);
            var list = new List<VehicleCountResponse>();
            foreach (var vehicleType in vehicleTypes)
            {
                var vehicleCount = new VehicleCountResponse()
                {
                    VehicleType = vehicleType.VehicleTypeName,
                    Quantity = vehicleType.Vehicles.Count
                };
                list.Add(vehicleCount);
            }
            result.Payload = list;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    private (double totalRevenueReceived, double totalPayouts, double totalExpensesForDriver) CalculateTransactionsStatistics(List<Transaction> trans)
    {
        var totalRevenueReceived = 0M;
        var totalPayouts = 0M;
        var totalExpensesForDriver = 0M;
        foreach (var tr in trans)
        {
            if (tr.TransactionType == TransactionType.Plus)
            {
                totalRevenueReceived += tr.Price;
            }
            if (tr.TransactionType == TransactionType.Minus)
            {
                totalPayouts += tr.Price;
            }

            if (tr.TransactionType == TransactionType.Minus && tr.Description.Contains("Thanh toán cho tài xế"))
            {
                totalExpensesForDriver += tr.Price;
            }
        }
        return ((double)totalRevenueReceived, (double)totalPayouts, (double)totalExpensesForDriver);
    }

    private long CountUserLogin(List<UserLogin> userLogin)
    {
        return userLogin.Count;
    }
}