using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.Service.Request;

namespace TsdDelivery.Api.Controllers;

public class ServiceController : BaseController
{
    private readonly IService _service;
    public ServiceController(IService service)
    {
        _service = service;
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllService()
    {
        var response = await _service.GetAllService();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateService(CreateServiceRequest request)
    {
        var response = await _service.CreateService(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Create Success");
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="serviceId"></param>
    /// <param name="vehicleTypeId"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteService(Guid serviceId, Guid vehicleTypeId)
    {
        var response = await _service.DeleteService(serviceId,vehicleTypeId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Delete Success");
    }

    [HttpGet]
    public async Task<IActionResult> GetServicesByVehicleTypeId(Guid vehicleTypeId)
    {
        var response = await _service.GetServicesByVehicleId(vehicleTypeId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}