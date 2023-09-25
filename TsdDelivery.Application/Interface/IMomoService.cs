using TsdDelivery.Application.Models;
using TsdDelivery.Application.Services.Momo.Request;

namespace TsdDelivery.Application.Interface;

public interface IMomoService
{
    public Task<OperationResult<string>> ProcessMomoPaymentReturn(MomoOneTimePaymentResultRequest MomoOneTimePaymentResultRequest);
}