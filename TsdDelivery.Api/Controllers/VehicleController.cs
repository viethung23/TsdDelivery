using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetAllVehicle()
    {
        var response = await _vehicleService.GetAllVehicle();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="request"></param>
    /// <param name="blob"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateVehicle([FromQuery] CreateVehicle request, IFormFile blob = null)
    {
        var response = await _vehicleService.CreateVehicle(request, blob);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Create Success");
    }


    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [ValidateGuid("id")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        var response = await _vehicleService.DeleteVehicle(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Delete Success");
    }
}
