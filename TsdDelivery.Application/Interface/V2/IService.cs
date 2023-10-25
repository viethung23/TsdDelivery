using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Service.Request;
using TsdDelivery.Application.Models.Service.Response;

namespace TsdDelivery.Application.Interface.V2;

public interface IService
{
    public Task<OperationResult<List<ServiceResponse>>> GetServices();
    public Task<OperationResult<ServiceResponse>> GetService(Guid id);
    public Task<OperationResult<ServiceResponse>> CreateService(CreateServiceRequest request);
    public Task<OperationResult<ServiceResponse>> DeleteService(Guid serviceId,Guid vehicleTypeId);
}