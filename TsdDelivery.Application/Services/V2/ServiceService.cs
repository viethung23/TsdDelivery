using MapsterMapper;
using TsdDelivery.Application.Interface.V2;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Service.Request;
using TsdDelivery.Application.Models.Service.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Services.V2;

public class ServiceService : IService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ServiceService(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<OperationResult<List<ServiceResponse>>> GetServices()
    {
        var result = new OperationResult<List<ServiceResponse>>();
        try
        {
            var services = await _unitOfWork.ServiceRepository.GetAllAsync();
            var list = new List<ServiceResponse>();

            foreach (var x in services)
            {
                var response = new ServiceResponse()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Price = x.Price,
                    ServiceName = x.ServiceName,
                };
                list.Add(response);
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

    public async Task<OperationResult<ServiceResponse>> GetService(Guid id)
    {
        var result = new OperationResult<ServiceResponse>();
        try
        {
            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(id);
            result.Payload = _mapper.Map<ServiceResponse>(service);
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

    public async Task<OperationResult<ServiceResponse>> CreateService(CreateServiceRequest request)
    {
        var result = new OperationResult<ServiceResponse>();
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var vehicleType = await _unitOfWork.VehicleTypeReposiory.GetByIdAsync(request.VehicleTypeId);
            
            var service = new Service()
            {
                ServiceName = request.ServiceName,
                Price = request.Price,
                Description = request.Description,
                VehicleType = vehicleType,
            };
            var entity = await _unitOfWork.ServiceRepository.AddAsync(service);
            await _unitOfWork.SaveChangeAsync();

            if (request.ShippingRateDtos.Any())
            {
                var listShippingRates = new List<ShippingRate>();
                foreach (var dto in request.ShippingRateDtos)
                {
                    var shippingRate = new ShippingRate()
                    {
                        KmFrom = dto.KmFrom,
                        KmTo = dto.KmTo,
                        Price = dto.Price,
                        Service = entity
                    };
                    listShippingRates.Add(shippingRate);
                }

                await _unitOfWork.ShippingRateRepository.AddRangeAsync(listShippingRates);
                await _unitOfWork.SaveChangeAsync();
            }
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
            await transaction.RollbackAsync();
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<ServiceResponse>> DeleteService(Guid serviceId, Guid vehicleTypeId)
    {
        var result = new OperationResult<ServiceResponse>();
        try
        {
            var vehicleType = await _unitOfWork.VehicleTypeReposiory.GetByIdAsync(vehicleTypeId);
            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(serviceId);
            await _unitOfWork.ServiceRepository.Delete(service);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess) throw new Exception();
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