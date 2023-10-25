using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface.V2;
using TsdDelivery.Application.Services.V2;

namespace TsdDelivery.Api.Controllers.V2;

[Route("api/v{version:apiVersion}/wallets")] 
[ApiController]
[ApiVersion("2.0")]
public class WalletController : BaseController
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetWallet(Guid id)
    {
        var response = await _walletService.GetWalletById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}