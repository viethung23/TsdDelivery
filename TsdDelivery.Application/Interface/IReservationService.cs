using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Reservation.Request;
using TsdDelivery.Application.Models.Reservation.Response;

namespace TsdDelivery.Application.Interface;

public interface IReservationService
{
    public Task<OperationResult<CalculatePriceResponse>> CalculateTotalPrice(CalculatePriceRequest request);
    public Task<OperationResult<CreateReservationResponse>> CreateReservation(CreateReservationRequest request);
    public Task<OperationResult<List<ReservationResponse>>> GetAllReservation();
    public Task<OperationResult<List<ReservationAwaitingDriverResponse>>> GetAwaitingDriverReservation(Coordinates coordinates);
    public Task<OperationResult<ReservationAwaitingDriverResponse>> GetAwaitingDriverReservationDetail(Guid id,Coordinates coordinates);
    public Task<OperationResult<ReservationResponse>> AcceptReservation(Guid driverId, Guid reservationId);
}