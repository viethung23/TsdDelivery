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

    [HttpGet]
    public async Task<IActionResult> GetAllService()
    {
        var response = await _service.GetAllService();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateService(CreateServiceRequest request)
    {
        var response = await _service.CreateService(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Create Success");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteService(Guid serviceId, Guid vehicleTypeId)
    {
        var response = await _service.DeleteService(serviceId,vehicleTypeId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Delete Success");
    }
}