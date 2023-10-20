using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.DashBoard.Response;

namespace TsdDelivery.Application.Interface.V1;

public interface IDashBoardService
{
    public Task<OperationResult<UserCountResult>> GetCountPercentUser();
    public Task<OperationResult<RevenueDataResponse>> GetRevenueDataByTime(DateTime from, DateTime to);
    public Task<OperationResult<UserLoginResponse>> GetUserLoginCount(DateTime from, DateTime to);
    public Task<OperationResult<List<VehicleCountResponse>>> GetVehicleByVehicleType();
}