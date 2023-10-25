using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.DashBoard.Response;
using TsdDelivery.Application.Models.Reservation.Response;

namespace TsdDelivery.Application.Interface;

public interface IDashBoardService
{
    public Task<OperationResult<UserCountResult>> GetCountPercentUser();
    public Task<OperationResult<RevenueDataResponse>> GetRevenueDataByTime(DateTime from, DateTime to);
    public Task<OperationResult<UserLoginResponse>> GetUserLoginCount(DateTime from, DateTime to);
    public Task<OperationResult<List<VehicleCountResponse>>> GetVehicleByVehicleType();
    public Task<OperationResult<TotalReservationResponse>> GetTotalReservationByDay(DateTime from, DateTime to);
    public Task<OperationResult<dynamic>> GetTotalReservationByVehicleType();
}