using MapsterMapper;
using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V2;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Service.Response;
using TsdDelivery.Application.Models.VehicleType.Request;
using TsdDelivery.Application.Models.VehicleType.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Services.V2;

public class VehicleTypeService : IVehicleTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBlobStorageAzureService _blobStorageAzureService;

    public VehicleTypeService(IUnitOfWork unitOfWork,IMapper mapper,IBlobStorageAzureService blobStorageAzureService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _blobStorageAzureService = blobStorageAzureService;
    }
    
    public async Task<OperationResult<List<VehicleTypeResponse>>> GetVehicleTypes()
    {
        var result = new OperationResult<List<VehicleTypeResponse>>();
        try
        {
            var vType = await _unitOfWork.VehicleTypeReposiory.GetAllAsync();
            result.Payload = _mapper.Map<List<VehicleTypeResponse>>(vType);
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<VehicleTypeResponse>> CreateVehicleType(CreateVehicleType request, IFormFile? blob = null)
    {
        var result = new OperationResult<VehicleTypeResponse>();
        try
        {
            var entity = new VehicleType()
            {
                VehicleTypeName = request.VehicleTypeName,
                Description = request.Description,
            };
            if(blob != null)
            {
                entity.VehicleTypeImage = await _blobStorageAzureService.SaveImageAsync(blob);
            }

            await _unitOfWork.VehicleTypeReposiory.AddAsync(entity);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if(!isSuccess)
            {
                throw new Exception();
            }
            var vehicleTypeRespone = new VehicleTypeResponse()
            {
                Id = entity.Id,
                VehicleTypeName = entity.VehicleTypeName,
                Description= entity.Description,
                VehicleTypeImage = entity.VehicleTypeImage
            };
            result.Payload = vehicleTypeRespone;
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<VehicleTypeResponse>> DeleteVehicleType(Guid id)
    {
        var result = new OperationResult<VehicleTypeResponse>();
        try
        {
            var entity = await _unitOfWork.VehicleTypeReposiory.GetByIdAsync(id);
            await _unitOfWork.VehicleTypeReposiory.Delete(entity);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if(!isSuccess) { throw new Exception(); }
        }
        catch(Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<VehicleTypeDetailResponse>> GetVehicleType(Guid id)
    {
        var result = new OperationResult<VehicleTypeDetailResponse>();
        try
        {
            string[] include = {"services"};
            var vehicleType = await _unitOfWork.VehicleTypeReposiory.GetSingleByCondition(x => x.Id == id, include);
            var data = _mapper.Map<VehicleTypeDetailResponse>(vehicleType);
            //var data = vehicleType.Adapt<VehicleTypeDetailResponse>();
            result.Payload = data;
        }
        catch (Exception e)
        {
            result.AddUnknownError($"Not Found by ID: [{id}]");
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<List<ServiceResponseDetail>>> GetServicesByVehicleType(Guid id)
    {
        var result = new OperationResult<List<ServiceResponseDetail>>();
        try
        {
            string[] include = {"services"};
            var vehicleType = await _unitOfWork.VehicleTypeReposiory.GetSingleByCondition(x => x.Id == id, include);
            var services = vehicleType.services.ToList();
            var list = new List<ServiceResponseDetail>();
            foreach (var service in services)
            {
                var serviceResponseDetail = new ServiceResponseDetail()
                {
                    Id = service.Id,
                    ServiceName = service.ServiceName,
                    Description = service.Description,
                    Price = service.Price,
                    IsShow = service.Price != 0M
                };
                list.Add(serviceResponseDetail);
            }
            result.Payload = list;
        }
        catch (Exception e)
        {
            result.AddUnknownError($"Not Found by ID: [{id}]");
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }
}