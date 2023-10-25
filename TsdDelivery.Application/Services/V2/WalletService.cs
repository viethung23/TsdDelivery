using MapsterMapper;
using TsdDelivery.Application.Interface.V2;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Wallet.Response;

namespace TsdDelivery.Application.Services.V2;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WalletService(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<OperationResult<WalletResponse>> GetWalletById(Guid id)
    {
        var result = new OperationResult<WalletResponse>();
        try
        {
            var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(id);
            result.Payload = _mapper.Map<WalletResponse>(wallet);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }
}