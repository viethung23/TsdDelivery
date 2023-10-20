using MapsterMapper;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Transaction.Response;

namespace TsdDelivery.Application.Services.V1;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<OperationResult<List<TransactionResponse>>> GetTransactionByUserId(Guid userId)
    {
        var result = new OperationResult<List<TransactionResponse>>();
        try
        {
            var wallet = await _unitOfWork.WalletRepository.GetSingleByCondition(x => x.UserId == userId);
            var transactions = await _unitOfWork.TransactionRepository.GetMulti(x => x.WalletId == wallet.Id);

            var map = _mapper.Map<List<TransactionResponse>>(transactions);
            result.Payload = map;
        }
        catch (InvalidOperationException)
        {
            result.AddUnknownError($"Not found ID:[{userId}]");
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