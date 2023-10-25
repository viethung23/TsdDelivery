using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Wallet.Response;

namespace TsdDelivery.Application.Interface.V2;

public interface IWalletService
{
    Task<OperationResult<WalletResponse>> GetWalletById(Guid id);
}