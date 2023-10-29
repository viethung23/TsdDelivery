using TsdDelivery.Application.Models;
using TsdDelivery.Application.Services.Momo.Request;

namespace TsdDelivery.Application.Interface;

public interface IPaymentService
{
    public Task<OperationResult<string>> ProcessPaymentReturn(string paymentMethod,object paymentResultRequest);
    public Task<OperationResult<string>> CancelPayPalPayment(string token);
}