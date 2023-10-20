using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Interface.V1;

namespace TsdDelivery.Api.Controllers.V1;

public class WalletController : BaseController
{
    private readonly IWalletService _walletService;
    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetWalletById(Guid id)
    {
        var response = await _walletService.GetWalletById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetWalletByUserId(Guid userId)
    {
        var response = await _walletService.GetWalletByUserId(userId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}