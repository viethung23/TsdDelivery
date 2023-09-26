using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Transaction.Response;

namespace TsdDelivery.Application.Interface;

public interface ITransactionService
{
    public Task<OperationResult<List<TransactionResponse>>> GetTransactionByUserId(Guid userId);
}