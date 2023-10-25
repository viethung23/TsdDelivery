using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface.V2;
using TsdDelivery.Application.Models.Vehicle.request;

namespace TsdDelivery.Api.Controllers.V2;

[Route("api/v{version:apiVersion}/vehicles")] 
[ApiController]
[ApiVersion("2.0")]
public class VehicleController : BaseController
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }
    
    [HttpGet]
    [ValidateGuid]
    //[Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetVehicles()
    {
        var response = await _vehicleService.GetVehicles();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("{id}")]
    //[Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetVehicle(Guid id)
    {
        var response = await _vehicleService.GetVehicle(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateVehicle([FromQuery] CreateVehicle request, IFormFile blob = null)
    {
        var response = await _vehicleService.CreateVehicle(request, blob);
        return response.IsError ? HandleErrorResponse(response.Errors) : CreatedAtAction(nameof(GetVehicle), new {id = response.Payload?.Id},response.Payload);
    }
    
    [HttpDelete]
    [ValidateGuid("id")]
    //[Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        var response = await _vehicleService.DeleteVehicle(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
    }
}