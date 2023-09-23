using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Vehicle.request;
using TsdDelivery.Application.Models.Vehicle.Response;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageAzureService _blobStorageAzureService;
    public VehicleService(IUnitOfWork unitOfWork, IBlobStorageAzureService lobStorageAzureService)
    {
        _unitOfWork = unitOfWork;
        _blobStorageAzureService = lobStorageAzureService;
    }
    public async Task<OperationResult<VehicleResponse>> CreateVehicle(CreateVehicle request, IFormFile blob = null)
    {
        var result = new OperationResult<VehicleResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetSingleByCondition(x => x.Id == Guid.Parse(request.UserId),
                new[] { "Role" });

            if (!user.Role.RoleName.Equals("DRIVER"))
            {
                result.AddError(ErrorCode.ServerError,
                    $"Can not create vehicle with UserId: [{request.UserId}] because it's not the driver");
                return result;
            }

            var vehicleType = await _unitOfWork.VehicleTypeReposiory.GetByIdAsync(Guid.Parse(request.VehicleTypeId));

            var vehicle = new Vehicle()
            {
                NameVehicle = request.NameVehicle,
                Description = request.Description,
                LicensePlate = request.LicensePlate,
                VehicleLoad = request.VehicleLoad,
                User = user,
                VehicleType = vehicleType,
            };

            if (blob != null)
            {
                vehicle.ImageVehicle = await _blobStorageAzureService.SaveImageAsync(blob);
            }

            await _unitOfWork.VehicleRepository.AddAsync(vehicle);
            await _unitOfWork.SaveChangeAsync();
        }
        catch (InvalidOperationException e)
        {
            result.AddUnknownError($"Not Found by ID: [{request.UserId}]");
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

    public async Task<OperationResult<VehicleResponse>> DeleteVehicle(Guid id)
    {
        var result = new OperationResult<VehicleResponse>();
        try
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
            await _unitOfWork.VehicleRepository.Delete(vehicle);
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

    public async Task<OperationResult<List<VehicleResponse>>> GetAllVehicle()
    {
        var result = new OperationResult<List<VehicleResponse>>();

        try
        {
            var vehicles = await _unitOfWork.VehicleRepository.GetAllAsync();
            var list = new List<VehicleResponse>();
            foreach (var vehicle in vehicles)
            {
                var vResponse = new VehicleResponse
                {
                    Id = vehicle.Id,
                    NameVehicle = vehicle.NameVehicle,
                    Description = vehicle.Description,
                    ImageVehicle = vehicle.ImageVehicle,
                    LicensePlate = vehicle.LicensePlate,
                    VehicleLoad = vehicle.VehicleLoad
                };
                list.Add(vResponse);
            }
            result.Payload = list;
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
}
