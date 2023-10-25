using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Service.Response;
using TsdDelivery.Application.Models.VehicleType.Request;
using TsdDelivery.Application.Models.VehicleType.Response;

namespace TsdDelivery.Application.Interface.V2;

public interface IVehicleTypeService
{
    public Task<OperationResult<List<VehicleTypeResponse>>> GetVehicleTypes();
    public Task<OperationResult<VehicleTypeResponse>> CreateVehicleType(CreateVehicleType request, IFormFile? blob = null);
    public Task<OperationResult<VehicleTypeResponse>> DeleteVehicleType(Guid id);
    public Task<OperationResult<VehicleTypeDetailResponse>> GetVehicleType(Guid id);
    public Task<OperationResult<List<ServiceResponseDetail>>> GetServicesByVehicleType(Guid id);
}