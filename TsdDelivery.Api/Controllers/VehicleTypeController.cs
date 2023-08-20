using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Models.VehicleType.Request;
using TsdDelivery.Application.Interface;

namespace TsdDelivery.Api.Controllers;

public class VehicleTypeController : BaseController
{
    private readonly IVehicleTypeService _vehicleService;
    public VehicleTypeController(IVehicleTypeService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVehicleType()
    {
        var response = await _vehicleService.GetAllVehicleType();
        if(response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok(response.Payload);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateVehicleType(IFormFile? blob,  string name, string? description)    
    {
        var request = new CreateVehicleType { VehicleTypeName = name,Description = description };
        var response = await _vehicleService.CreateVehicleType(request, blob);
        if(response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok(response.Payload);
    }

    [HttpDelete]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteVehicleType(Guid id)
    {
        var response = await _vehicleService.DeleteVehicleType(id);
        if(response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("Delete Success");
    }
}
