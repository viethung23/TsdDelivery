using MapsterMapper;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Wallet.Response;

namespace TsdDelivery.Application.Services.V1;

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
        return result;
    }

    public async Task<OperationResult<WalletResponse>> GetWalletByUserId(Guid userId)
    {
        var result = new OperationResult<WalletResponse>();
        try
        {
            var wallet = await _unitOfWork.WalletRepository.GetSingleByCondition(x => x.UserId == userId);
            result.Payload = _mapper.Map<WalletResponse>(wallet);
        }
        catch (InvalidOperationException e)
        {
            result.AddUnknownError($"Not found ID:[{userId}]");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}