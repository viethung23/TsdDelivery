using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.Vehicle.request;

namespace TsdDelivery.Api.Controllers;

public class VehicleController : BaseController
{
    private readonly IVehicleService _vehicleService;
    private readonly IBlobStorageAzureService _blobStorageAzureService;
    public VehicleController(IVehicleService vehicleService, IBlobStorageAzureService blobStorageAzureService)
    {
        _vehicleService = vehicleService;
        _blobStorageAzureService = blobStorageAzureService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVehicle()
    {
        var response = await _vehicleService.GetAllVehicle();
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }

        return Ok(response.Payload);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateVehicle([FromQuery] CreateVehicle request, IFormFile blob = null)
    {
        var response = await _vehicleService.CreateVehicle(request, blob);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("Create Success");
    }


    [HttpDelete]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        var response = await _vehicleService.DeleteVehicle(id);
        if(response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("Delete Success");
    }
}
