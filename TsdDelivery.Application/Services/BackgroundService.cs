using TsdDelivery.Application.Interface;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Services;

public class BackgroundService : IBackgroundService
{
    private readonly IUnitOfWork _unitOfWork;
    public BackgroundService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task AutoCancelReservationWhenOverAllowPaymentTime(Guid reservationId)
    {
        try
        {
            var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(reservationId);
            if (reservation.ReservationStatus == ReservationStatus.AwaitingPayment)
            {
                reservation.ReservationStatus = ReservationStatus.Cancelled;
                await _unitOfWork.SaveChangeAsync();
            }
            
        }
        catch (Exception e)
        {
            throw new Exception($"Error at BackgroundService.AutoCancelReservationWhenOverAllowPaymentTime: Message {e.Message}");
        }
    }
}