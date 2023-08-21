using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Models.VehicleType.Request;
using TsdDelivery.Application.Interface;

namespace TsdDelivery.Api.Controllers;

public class VehicleTypeController : BaseController
{
    private readonly IVehicleTypeService _vehicleTypeService;
    public VehicleTypeController(IVehicleTypeService vehicleService)
    {
        _vehicleTypeService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVehicleType()
    {
        var response = await _vehicleTypeService.GetAllVehicleType();
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
        var response = await _vehicleTypeService.CreateVehicleType(request, blob);
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
        var response = await _vehicleTypeService.DeleteVehicleType(id);
        if(response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("Delete Success");
    }
}
