using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.Vehicle.request;

namespace TsdDelivery.Api.Controllers;

public class VehicleController : BaseController
{
    private readonly IVehicleService _vehicleService;
    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVehicle()
    {
        var response = await _vehicleService.GetAllVehicle();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateVehicle([FromQuery] CreateVehicle request, IFormFile blob = null)
    {
        var response = await _vehicleService.CreateVehicle(request, blob);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Create Success");
    }


    [HttpDelete]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        var response = await _vehicleService.DeleteVehicle(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Delete Success");
    }
}
