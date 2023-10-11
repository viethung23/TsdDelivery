using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Coordinates;
using TsdDelivery.Application.Models.Reservation.Enum;
using TsdDelivery.Application.Models.Reservation.Request;
using TsdDelivery.Application.Models.Reservation.Response;

namespace TsdDelivery.Application.Interface;

public interface IReservationService
{
    public Task<OperationResult<CalculatePriceResponse>> CalculateTotalPrice(CalculatePriceRequest request);
    public Task<OperationResult<CreateReservationResponse>> CreateReservation(CreateReservationRequest request);
    public Task<OperationResult<List<ReservationsResponse>>> GetAllReservation();
    public Task<OperationResult<List<ReservationAwaitingDriverResponse>>> GetAwaitingDriverReservation(CurrentCoordinates currentCoordinates,DestinationCoordinates destinationCoordinates,bool isNow);
    public Task<OperationResult<ReservationAwaitingDriverDetailResponse>> GetAwaitingDriverReservationDetail(Guid reservationId,CurrentCoordinates currentCoordinates,DestinationCoordinates destinationCoordinates);
    public Task<OperationResult<ReservationsResponse>> DriverReservationAction(Guid driverId, Guid reservationId, DriverReservationAction action);
    public Task<OperationResult<List<ReservationHistoryResponse>>> GetReservationHistoryForUser();
    public Task<OperationResult<ReservationHistoryDetailResponse>> GetReservationHistoryDetailForUser(Guid reservationId);
    public Task<OperationResult<ReservationsResponse>> GetCurrentAcceptedReservationbyDriver(Guid dirverId);
}