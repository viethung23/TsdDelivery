using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.VehicleType.Request;
using TsdDelivery.Application.Models.VehicleType.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Services;

public class VehicleTypeService : IVehicleTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageAzureService _blobStorageAzureService;
    public VehicleTypeService(IUnitOfWork unitOfWork, IBlobStorageAzureService blobStorageAzureService)
    {
        _unitOfWork = unitOfWork;
        _blobStorageAzureService = blobStorageAzureService;

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

    public async Task<OperationResult<List<VehicleTypeResponse>>> GetAllVehicleType()
    {
        var result = new OperationResult<List<VehicleTypeResponse>>();

        try
        {
            var vType = await _unitOfWork.VehicleTypeReposiory.GetAllAsync();
            var list = new List<VehicleTypeResponse>();

            foreach ( var vehicleType in vType)
            {
                var vehiscleResponse = new VehicleTypeResponse()
                {
                    Id = vehicleType.Id,
                    VehicleTypeName = vehicleType.VehicleTypeName,
                    VehicleTypeImage = vehicleType.VehicleTypeImage,
                    Description = vehicleType.Description,
                };
                list.Add(vehiscleResponse);
            }
            result.Payload = list;


        }catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }
}
