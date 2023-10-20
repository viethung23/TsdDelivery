using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Transaction.Response;

namespace TsdDelivery.Application.Interface.V1;

public interface ITransactionService
{
    public Task<OperationResult<List<TransactionResponse>>> GetTransactionByUserId(Guid userId);
}