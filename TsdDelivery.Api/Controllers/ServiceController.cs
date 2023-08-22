using Microsoft.AspNetCore.Mvc;
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
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok(response.Payload);
    }

    [HttpPost]
    public async Task<IActionResult> CreateService(CreateServiceRequest request)
    {
        var response = await _service.CreateService(request);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("Create Success");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteService(Guid serviceId, Guid vehicleTypeId)
    {
        var response = await _service.DeleteService(serviceId,vehicleTypeId);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("Delete Success");
    }
}