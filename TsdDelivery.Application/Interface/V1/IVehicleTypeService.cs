using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.VehicleType.Request;
using TsdDelivery.Application.Models.VehicleType.Response;

namespace TsdDelivery.Application.Interface.V1;

public interface IVehicleTypeService
{
    public Task<OperationResult<List<VehicleTypeResponse>>> GetAllVehicleType();
    public Task<OperationResult<VehicleTypeResponse>> CreateVehicleType(CreateVehicleType request, IFormFile? blob = null);
    public Task<OperationResult<VehicleTypeResponse>> DeleteVehicleType(Guid id);
    public Task<OperationResult<VehicleTypeDetailResponse>> GetVehicleTypeDetail(Guid id);

}
