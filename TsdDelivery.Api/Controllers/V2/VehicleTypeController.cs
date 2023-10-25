using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface.V2;
using TsdDelivery.Application.Models.VehicleType.Request;
using TsdDelivery.Application.Services.V2;

namespace TsdDelivery.Api.Controllers.V2;

[Route("api/v{version:apiVersion}/vehicle-types")] 
[ApiController]
[ApiVersion("2.0")]
public class VehicleTypeController : BaseController
{
    private readonly IVehicleTypeService _vehicleTypeService;

    public VehicleTypeController(IVehicleTypeService vehicleTypeService)
    {
        _vehicleTypeService = vehicleTypeService;
    }
    
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetVehicleTypes()
    {
        var response = await _vehicleTypeService.GetVehicleTypes();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("{id}")]
    [ValidateGuid("id")]
    public async Task<IActionResult> GetVehicleType(Guid id)
    {
        var response = await _vehicleTypeService.GetVehicleType(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpGet("{vTypeId}/services")]
    [ValidateGuid("vTypeId")]
    public async Task<IActionResult> GetServicesByVehicleType(Guid vTypeId)
    {
        var response = await _vehicleTypeService.GetServicesByVehicleType(vTypeId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
    
    [HttpPost]
    [ValidateModel]
    //[Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> CreateVehicleType(IFormFile? blob,  string name, string? description)    
    {
        var request = new CreateVehicleType { VehicleTypeName = name,Description = description };
        var response = await _vehicleTypeService.CreateVehicleType(request, blob);
        return response.IsError ? HandleErrorResponse(response.Errors) : CreatedAtAction(nameof(GetVehicleType), new {id = response.Payload?.Id},response.Payload);
    }
    
    [HttpDelete("{id}")]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteVehicleType(Guid id)
    {
        var response = await _vehicleTypeService.DeleteVehicleType(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : NoContent();
    }
}