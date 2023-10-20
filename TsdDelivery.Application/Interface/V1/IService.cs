using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Service.Request;
using TsdDelivery.Application.Models.Service.Response;

namespace TsdDelivery.Application.Interface.V1;

public interface IService
{
    public Task<OperationResult<List<ServiceResponse>>> GetAllService();
    public Task<OperationResult<ServiceResponse>> CreateService(CreateServiceRequest request);
    public Task<OperationResult<ServiceResponse>> DeleteService(Guid serviceId,Guid vehicleTypeId);
    public Task<OperationResult<List<ServiceResponseDetail>>> GetServicesByVehicleId(Guid vehicleTypeId);
}