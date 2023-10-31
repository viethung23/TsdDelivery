using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Services.Momo.Request;
using TsdDelivery.Application.Services.PayPal.Request;

namespace TsdDelivery.Api.Controllers.V1;

[Route("api/payment")]
[ApiController]
public class PaymentsController : BaseController
{
    private readonly IPaymentService _paymentService;
    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    /// <summary>
    /// Api for System
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("momo-return")]
    public async Task<IActionResult> MomoReturn([FromQuery] MomoOneTimePaymentResultRequest request)
    {
        var paymentMethod = "MOMO";
        var response = await _paymentService.ProcessPaymentReturn(paymentMethod, request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Redirect($"{response.Payload}/order-success");
    }


    /// <summary>
    /// Api for System
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("paypal-return")]
    public async Task<IActionResult> PayPalReturn([FromQuery] PayPalPaymentResultRequest request)
    {
        var paymentMethod = "PAYPAL";
        var response = await _paymentService.ProcessPaymentReturn(paymentMethod, request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Redirect($"{response.Payload}/order-success");
    }
    
    /// <summary>
    /// Api for MA GIÁO
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("zalopay-handle-return")]
    public async Task<IActionResult> ZaloPayReturn_Handle([FromQuery]string orderId)
    {
        var paymentMethod = "ZALOPAY";
        var response = await _paymentService.ProcessPaymentReturn(paymentMethod,orderId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Redirect($"{response.Payload}/order-success");
    }

    /// <summary>
    /// Api for System
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("paypal-cancel")]
    public async Task<IActionResult> PayPalCancel([FromQuery]string token)
    {
        var response = await _paymentService.CancelPayPalPayment(token);
        return response.IsError ? HandleErrorResponse(response.Errors) : Redirect($"{response.Payload}/user");
    }
    
    
    
}