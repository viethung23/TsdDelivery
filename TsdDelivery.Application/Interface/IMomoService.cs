using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Reservation.Request;
using TsdDelivery.Application.Services.Momo.Request;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Interface;

public interface IMomoService
{
    //public Task<OperationResult<string>> ProcessMomoPaymentReturn(MomoOneTimePaymentResultRequest MomoOneTimePaymentResultRequest);
    public Task<(bool, string?, string?)> CreateMomoPayment(CreateReservationRequest request,Reservation reservation);
}