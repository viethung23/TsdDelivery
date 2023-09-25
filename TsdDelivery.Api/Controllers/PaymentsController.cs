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
    
    [HttpGet]
    [Route("momo-return")]
    public async Task<IActionResult> MomoReturn([FromQuery]MomoOneTimePaymentResultRequest request)
    {
        /*string returnUrl = string.Empty;
        var returnModel = new PaymentReturnDtos();
        var processResult = await mediator.Send(response.Adapt<ProcessMomoPaymentReturn>());

        if (processResult.Success)
        {
            returnModel = processResult.Data.Item1 as PaymentReturnDtos;
            returnUrl = processResult.Data.Item2 as string;
        }

        if (returnUrl.EndsWith("/"))
            returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
        return Redirect($"{returnUrl}?{returnModel.ToQueryString()}");*/

        var response = await _momoService.ProcessMomoPaymentReturn(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        
        //return Ok("HAHAHAHAHA");
    }
}