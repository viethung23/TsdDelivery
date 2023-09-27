using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Reservation.DTO;
using TsdDelivery.Application.Models.Reservation.Request;
using TsdDelivery.Application.Models.Reservation.Response;
using TsdDelivery.Application.Services.Momo.Request;
using TsdDelivery.Domain.Entities;
using TsdDelivery.Domain.Entities.Enums;

namespace TsdDelivery.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly AppConfiguration _configuration;
    private readonly ICurrentTime _currentTime;
    private readonly IClaimsService _claimsService;

    public ReservationService(IUnitOfWork unitOfWork,IMapper mapper,AppConfiguration appConfiguration,ICurrentTime currentTime,IClaimsService claimsService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = appConfiguration;
        _currentTime = currentTime;
        _claimsService = claimsService;
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

    public async Task<OperationResult<CreateReservationResponse>> CreateReservation(CreateReservationRequest request)
    {
        var result = new OperationResult<CreateReservationResponse>();
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
                    PickUpDateTime = request.PickUpDateTime ?? _currentTime.GetCurrentTime(),
                    Goods = goods,
                    TotallPrice = request.TotalPrice,
                    UserId = _claimsService.GetCurrentUserId,
                    ReservationStatus = ReservationStatus.AwaitingPayment,
                    latitudeSendLocation = request.latitudeSendLocation,
                    longitudeSendLocation = request.longitudeSendLocation
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
                
                // thưc hiện chức năng thanh toán ở đây sau khi tạo xong đơn 
                // đang để defaut phương thức thanh toán là MOMO
                var paymentMethod = "Momo";
                var paymentUrl = string.Empty;
                var momoOneTimePayRequest = new MomoOneTimePaymentRequest(_configuration.MomoConfig.PartnerCode,
                    DateTime.Now.Ticks.ToString()+entity.Id ?? string.Empty, (long)request.TotalPrice!, entity.Id!.ToString() ?? string.Empty,
                    "Thanh toán đặt xe TSD"?? string.Empty, _configuration.MomoConfig.ReturnUrl,_configuration.MomoConfig.IpnUrl, "captureWallet",
                    string.Empty);
                momoOneTimePayRequest.MakeSignature(_configuration.MomoConfig.AccessKey, _configuration.MomoConfig.SecretKey);
                (bool createMomoLinkResult, string? createMessage, string? deepLink) = momoOneTimePayRequest.GetLink(_configuration.MomoConfig.PaymentUrl);
                if (createMomoLinkResult)
                {
                    result.Payload = new CreateReservationResponse()
                    {
                        Id = entity.Id,
                        PaymentUrl = createMessage,
                        deeplink = deepLink
                    };
                }
                else
                {
                    throw new Exception(createMessage);
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

    public async Task<OperationResult<List<ReservationResponse>>> GetAllReservation()
    {
        var result = new OperationResult<List<ReservationResponse>>();
        try
        {
            var reservations = await _unitOfWork.ReservationRepository.GetAllAsync();
            var list = _mapper.Map<List<ReservationResponse>>(reservations);
            result.Payload = list;
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

    public async Task<OperationResult<List<ReservationResponse>>> GetAwaitingDriverReservation(Coordinates coordinates)
    {
        var result = new OperationResult<List<ReservationResponse>>();
        try
        {
            var reservations = await _unitOfWork.ReservationRepository.GetMulti(x =>x.ReservationStatus == ReservationStatus.AwaitingDriver);
            var list = reservations.Select(x => new ReservationResponse
                {
                    Id = x.Id,
                    RecipientName = x.RecipientName,
                    RecipientPhone = x.RecipientPhone,
                    ReciveLocation = x.ReciveLocation,
                    SendLocation = x.SendLocation,
                    PickUpDateTime = x.PickUpDateTime,
                    ReservationStatus = x.ReservationStatus.ToString(),
                    TotallPrice = x.TotallPrice,
                    Distance = x.Distance,
                    GoodsDto = new GoodsDto
                    {
                        Width = x.Goods.Width,
                        Length = x.Goods.Length,
                        Name = x.Goods.Name,
                        Weight = x.Goods.Weight,
                        Height = x.Goods.Height
                    }
                })
                .ToList();
            //var list = _mapper.Map<List<ReservationResponse>>(reservations);

            result.Payload = list;
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

    public async Task<OperationResult<ReservationResponse>> GetAwaitingDriverReservationDetail(Guid id, Coordinates coordinates)
    {
        var result = new OperationResult<ReservationResponse>();
        try
        {
            var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(id);
            result.Payload = _mapper.Map<ReservationResponse>(reservation);
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