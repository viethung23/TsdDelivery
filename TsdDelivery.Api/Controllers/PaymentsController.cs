using Mapster;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Services.Momo.Request;

namespace TsdDelivery.Api.Controllers;

[Route("api/payment")]
[ApiController]
public class PaymentsController : BaseController
{
    private readonly IMomoService _momoService;
    public PaymentsController(IMomoService momoService)
    {
        _momoService = momoService;
    }
    
    /// <summary>
    /// Api for System
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("momo-return")]
    public async Task<IActionResult> MomoReturn([FromQuery]MomoOneTimePaymentResultRequest request)
    {
        var response = await _momoService.ProcessMomoPaymentReturn(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}