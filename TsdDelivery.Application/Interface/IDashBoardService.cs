using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.DashBoard.Response;

namespace TsdDelivery.Application.Interface;

public interface IDashBoardService
{
    public Task<OperationResult<UserCountResult>> GetCountPercentUser();
    public Task<OperationResult<RevenueDataResponse>> GetRevenueDataByTime(DateTime from, DateTime to);
    public Task<OperationResult<UserLoginResponse>> GetUserLoginCount(DateTime from, DateTime to);
}