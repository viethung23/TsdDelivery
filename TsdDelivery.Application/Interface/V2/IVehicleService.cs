using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Vehicle.request;
using TsdDelivery.Application.Models.Vehicle.Response;

namespace TsdDelivery.Application.Interface.V2;

public interface IVehicleService
{
    public Task<OperationResult<List<VehicleResponse>>> GetVehicles();
    public Task<OperationResult<VehicleResponse>> GetVehicle(Guid id);
    public Task<OperationResult<VehicleResponse>> CreateVehicle(CreateVehicle request, IFormFile blob = null);
    public Task<OperationResult<VehicleResponse>> DeleteVehicle(Guid id);
}