using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Reservation.Request;
using TsdDelivery.Application.Models.Reservation.Response;
using TsdDelivery.Domain.Entities;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<OperationResult<CalculatePriceResponse>> CalculateTotalPrice(CalculatePriceRequest request)
    {
        // here implement logic 
        decimal totalAmount = 0;
        var result = new OperationResult<CalculatePriceResponse>();
        try
        {
            foreach (var id in request.ServiceIds)
            {
                var service = await _unitOfWork.ServiceRepository.GetByIdAsync(id);
                var shippingRates = await _unitOfWork.ShippingRateRepository.GetMulti(s => s.ServiceId == id);
                totalAmount += service.Price + CalculateShippingRateByKm(request.Distance,shippingRates) ;
            }
            result.Payload = new CalculatePriceResponse() { Distance = request.Distance, TotalAmount = totalAmount };
        }
        catch(Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<ReservationResponse>> CreateReservation(CreateReservationRequest request)
    {
        var result = new OperationResult<ReservationResponse>();
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var goods = new Goods()
                {
                    Height = request.GoodsDto.Height,
                    Length = request.GoodsDto.Length,
                    Name = request.GoodsDto.Name,
                    Weight = request.GoodsDto.Weight,
                    Width = request.GoodsDto.Width
                };
                var reservation = new Reservation()
                {
                    Distance = request.Distance,
                    RecipientName = request.RecipientName,
                    RecipientPhone = request.RecipientPhone,
                    ReciveLocation = request.ReciveLocation,
                    SendLocation = request.SendLocation,
                    PickUpDateTime = request.PickUpDateTime,
                    Goods = goods,
                    TotallPrice = request.TotalPrice,
                    ReservationStatus = ReservationStatus.pending
                };
                var entity = await _unitOfWork.ReservationRepository.AddAsync(reservation);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new NotImplementedException();
                
                foreach (var serviceId in request.ServiceIds)
                {
                    var service = await _unitOfWork.ServiceRepository.GetByIdAsync(serviceId);
                    await _unitOfWork.ReservationDetailRepository.AddAsync(new ReservationDetail { Reservation = entity, Service = service});
                    await _unitOfWork.SaveChangeAsync();
                }
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
                await transaction.RollbackAsync();
            }
            finally
            {
                _unitOfWork.Dispose();
            }
            return result;
        }
    }

    private decimal CalculateShippingRateByKm(decimal km, List<ShippingRate> list)
    {
        decimal total = 0;
        if (!list.Any()) return 0;
        foreach (var sr in list)
        {
            decimal multiplier;
            if (km > sr.KmTo)
            {
                multiplier = sr.KmTo - sr.KmFrom;
            }
            else if (km > sr.KmFrom && km < sr.KmTo)
            {
                multiplier = km - sr.KmFrom + 1;
            }
            else
            {
                multiplier = 0;
            }
            total += sr.Price * multiplier;
        }
        return total;
    }
}