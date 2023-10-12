using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Services.Momo.Request;

namespace TsdDelivery.Api.Controllers;

[Route("api/payment")]
[ApiController]
public class PaymentsController : BaseController
{
    private readonly IMomoService _momoService;
    private readonly IZaloPayService _zaloPayService;
    public PaymentsController(IMomoService momoService,IZaloPayService zaloPayService)
    {
        _momoService = momoService;
        _zaloPayService = zaloPayService;
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
        return response.IsError ? HandleErrorResponse(response.Errors) : Redirect($"{response.Payload}/order-success");
    }

    [HttpPost]
    [Route("momo-ipn")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> MomoIpn(MomoOneTimePaymentResultRequest request)
    {
        //var response = await _momoService.ProcessMomoPaymentReturn(request);
        return StatusCode(204);
    }
}