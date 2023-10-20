using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Api.Filters;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Models.ShippingRate.Request;

namespace TsdDelivery.Api.Controllers.V1;

public class ShippingRateController : BaseController
{
    private readonly IShippingRateService _shippingRateService;
    public ShippingRateController(IShippingRateService shippingRateService)
    {
        _shippingRateService = shippingRateService;
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetAllShippingRate()
    {
        var response = await _shippingRateService.GetAllShippingRate();
        if (response.IsError)
        {
            return HandleErrorResponse(response.Errors);
        }
        return Ok(response.Payload);
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateModel]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> CreateShippingRate(CreateShippingRateRequest request)
    {
        var response = await _shippingRateService.CreateShippingRate(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Create Success");
    }

    /// <summary>
    /// Api for Admin
    /// </summary>
    /// <param name="shippingRateId"></param>
    /// <param name="serviceId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> DeleteShippingRate(Guid shippingRateId,Guid serviceId)
    {
        var response = await _shippingRateService.DeleteShippingRate(shippingRateId, serviceId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok("Delete Success");
    }

    
    [HttpGet]
    public async Task<IActionResult> GetShippingRatesByService(Guid serviceId)
    {
        var response = await _shippingRateService.GetShippingRatesByService(serviceId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}