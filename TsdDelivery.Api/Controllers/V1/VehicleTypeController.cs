﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Models.VehicleType.Request;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;

namespace TsdDelivery.Api.Controllers.V1;

public class VehicleTypeController : BaseController
{
    private readonly IVehicleTypeService _vehicleTypeService;
    public VehicleTypeController(IVehicleTypeService vehicleService)
    {
        _vehicleTypeService = vehicleService;
    }

    /// <summary>
    /// Api for Admin,User
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllVehicleType()
    {
        var response = await _vehicleTypeService.GetAllVehicleType();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="blob"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateModel]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> CreateVehicleType(IFormFile? blob,  string name, string? description)    
    {
        var request = new CreateVehicleType { VehicleTypeName = name,Description = description };
        var response = await _vehicleTypeService.CreateVehicleType(request, blob);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [ValidateGuid("id")]
    public async Task<IActionResult> DeleteVehicleType(Guid id)
    {
        var response = await _vehicleTypeService.DeleteVehicleType(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Delete Success");
    }

    [HttpGet]
    [ValidateGuid("id")]
    public async Task<IActionResult> GetVehicleTypeDetail(Guid id)
    {
        var response = await _vehicleTypeService.GetVehicleTypeDetail(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}
