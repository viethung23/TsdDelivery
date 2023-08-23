using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.ShippingRate.Request;

namespace TsdDelivery.Api.Controllers;

public class ShippingRateController : BaseController
{
    private readonly IShippingRateService _shippingRateService;
    public ShippingRateController(IShippingRateService shippingRateService)
    {
        _shippingRateService = shippingRateService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShippingRate()
    {
        var response = await _shippingRateService.GetAllShippingRate();
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok(response.Payload);
    }

    [HttpPost]
    public async Task<IActionResult> CreateShippingRate(CreateShippingRateRequest request)
    {
        var response = await _shippingRateService.CreateShippingRate(request);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("Create Success");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteShippingRate(Guid shippingRateId,Guid serviceId)
    {
        var response = await _shippingRateService.DeleteShippingRate(shippingRateId, serviceId);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok("Delete Success");
    }

    [HttpGet]
    public async Task<IActionResult> GetShippingRatesByService(Guid serviceId)
    {
        var response = await _shippingRateService.GetShippingRatesByService(serviceId);
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok(response.Payload);
    }
}