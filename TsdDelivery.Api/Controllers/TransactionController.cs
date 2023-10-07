using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Interface;

namespace TsdDelivery.Api.Controllers;

public class TransactionController : BaseController
{
    private readonly ITransactionService _transactionService;
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllTransactionsByUserId(Guid userId)
    {
        var response = await _transactionService.GetTransactionByUserId(userId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}