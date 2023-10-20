using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Wallet.Response;

namespace TsdDelivery.Application.Interface.V1;

public interface IWalletService
{
    Task<OperationResult<WalletResponse>> GetWalletById(Guid id);
    Task<OperationResult<WalletResponse>> GetWalletByUserId(Guid userId);
}