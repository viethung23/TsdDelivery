using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.DashBoard.Response;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Services;

public class DashBoardService : IDashBoardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashBoardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
            var trans = wallet.Transactions!.Where(x => x.CreationDate >= from && x.CreationDate <= to).ToList();
            // xu ly data
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

                if (tr.TransactionType == TransactionType.Minus && tr.Description.Contains("thanh toán cho tài xế"))
                {
                    totalExpensesForDriver += tr.Price;
                }
            }

            var revenueDataResponse = new RevenueDataResponse()
            {
                TotalRevenueReceived = (double)totalRevenueReceived,
                TotalPayouts = (double)totalPayouts,
                TotalExpensesForDriver = (double)totalExpensesForDriver 
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
}