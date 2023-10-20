using TsdDelivery.Application.Models.Reservation.Request;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Interface;

public interface IZaloPayService
{
    //public Task<OperationResult<string>> ProcessZaloPayPaymentCallBack();
    public Task<(bool, string?)> CreateZaloPayment(CreateReservationRequest request,Reservation reservation);
}