using Hangfire;
using Mapster;
using MapsterMapper;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Coordinates;
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
    private readonly IMapService _mapService;
    private readonly IClaimsService _claimsService;
    private readonly IMomoService _momoService;
    private readonly IZaloPayService _zaloPayService;

    public ReservationService(IUnitOfWork unitOfWork, IMapper mapper, AppConfiguration appConfiguration,
        ICurrentTime currentTime, IClaimsService claimsService, IMapService mapService, IMomoService momoService,
        IZaloPayService zaloPayService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = appConfiguration;
        _currentTime = currentTime;
        _claimsService = claimsService;
        _mapService = mapService;
        _momoService = momoService;
        _zaloPayService = zaloPayService;
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
                totalAmount += service.Price + CalculateShippingRateByKm(request.Distance, shippingRates);
            }

            result.Payload = new CalculatePriceResponse() { Distance = request.Distance, TotalAmount = totalAmount };
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

    public async Task<OperationResult<CreateReservationResponse>> CreateReservation(CreateReservationRequest request)
    {
        var result = new OperationResult<CreateReservationResponse>();
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                if (request.IsNow == false && request.PickUpDateTime < _currentTime.GetCurrentTime())
                {
                    throw new Exception($"pickUpDateTime can not be less than now");
                }
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
                    ReciveLocation = request.ReceiveLocation,
                    SendLocation = request.SendLocation,
                    IsNow = request.IsNow,
                    PickUpDateTime = request.IsNow == true ? _currentTime.GetCurrentTime() : request.PickUpDateTime!.Value,
                    Goods = goods,
                    TotallPrice = request.TotalPrice,
                    UserId = _claimsService.GetCurrentUserId,
                    ReservationStatus = ReservationStatus.AwaitingPayment,
                    latitudeSendLocation = request.latitudeSendLocation,
                    longitudeSendLocation = request.longitudeSendLocation,
                    latitudeReciveLocation = request.latitudeReciveLocation,
                    longitudeReceiveLocation = request.longitudeReceiveLocation
                };
                var entity = await _unitOfWork.ReservationRepository.AddAsync(reservation);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new NotImplementedException();

                foreach (var serviceId in request.ServiceIds)
                {
                    var service = await _unitOfWork.ServiceRepository.GetByIdAsync(serviceId);
                    await _unitOfWork.ReservationDetailRepository.AddAsync(new ReservationDetail
                        { Reservation = entity, Service = service });
                    await _unitOfWork.SaveChangeAsync();
                }

                // thưc hiện chức năng thanh toán ở đây sau khi tạo xong đơn 
                var paymentMethod = request.PaymentMethod.ToUpper();
                switch (paymentMethod)
                {
                    case "MOMO":
                        (bool createMomoLinkResult, string createMessage, string deepLink) = await _momoService.CreateMomoPayment(request, entity);
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
                        break;
                    
                    case "ZALOPAY":
                        (bool createZaloPayLinkResult, string? createZaloPayMessage) = await _zaloPayService.CreateZaloPayment(request, entity);
                        if (createZaloPayLinkResult)
                        {
                            result.Payload = new CreateReservationResponse()
                            {
                                Id = entity.Id,
                                PaymentUrl = createZaloPayMessage,
                                deeplink = null
                            };
                        }
                        else
                        {
                            throw new Exception(createZaloPayMessage);
                        }
                        break;
                    
                    case "VNPAY":
                        
                        break;
                }
                
                await transaction.CommitAsync();
                
                // goi Background Service Check status sau 5p
                var timeToCancel = DateTime.UtcNow.AddMinutes(5);
                string id = BackgroundJob.Schedule<IBackgroundService>(
                    x => x.AutoCancelReservationWhenOverAllowPaymentTime(entity.Id), timeToCancel);
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

    public async Task<OperationResult<List<ReservationsResponse>>> GetAllReservation()
    {
        var result = new OperationResult<List<ReservationsResponse>>();
        try
        {
            var include = new[] {"User"};
            var reservations = await _unitOfWork.ReservationRepository.GetAllAsync(include);
            var list = _mapper.Map<List<ReservationsResponse>>(reservations);
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

    public async Task<OperationResult<List<ReservationAwaitingDriverResponse>>> GetAwaitingDriverReservation(CurrentCoordinates currentCoordinates,DestinationCoordinates destinationCoordinates,bool isNow)
    {
        var result = new OperationResult<List<ReservationAwaitingDriverResponse>>();
        string reciveLocation;
        string sendLocation;
        try
        {
            var reservations =
                await _unitOfWork.ReservationRepository.GetMulti(x =>
                    x.ReservationStatus == ReservationStatus.AwaitingDriver && x.IsNow == isNow);
             reservations = reservations.OrderByDescending(x => x.CreationDate).ToList();
            var list = new List<ReservationAwaitingDriverResponse>();
            foreach (var x in reservations)
            {
                double? dis = null;
                bool highPriorityLevel = false;
                
                if (CheckCurrentCoordinatesHasValue(currentCoordinates))
                {
                    if (CheckDestinationCoordinatesHasValue(destinationCoordinates))
                    {
                        // goi ham check
                        var checkXemDonCoNgonKhong = await CheckXemDonCoNgonKhong(currentCoordinates, 
                            x.latitudeSendLocation, x.longitudeSendLocation, x.latitudeReciveLocation,
                            x.longitudeReceiveLocation, destinationCoordinates); 
                        if (checkXemDonCoNgonKhong.Item1 == true)
                        {
                            highPriorityLevel = true;
                            dis = checkXemDonCoNgonKhong.Item2;
                        }
                        if (checkXemDonCoNgonKhong.Item1 == false)
                        {
                            dis = checkXemDonCoNgonKhong.Item2;
                        }
                    }
                    else
                    {
                        dis = await GetDistanseKm(currentCoordinates.Latitude, currentCoordinates.Longitude, x.latitudeSendLocation,
                            x.longitudeSendLocation);
                        if (dis! <= 10)        // if distance between driver and reservation < 10km
                        {
                            highPriorityLevel = true;
                        }
                    }
                }

                reciveLocation = CatChuoi(x.ReciveLocation);
                sendLocation = CatChuoi(x.SendLocation);
                var response = new ReservationAwaitingDriverResponse()
                {
                    Id = x.Id,
                    RecipientName = x.RecipientName,
                    RecipientPhone = x.RecipientPhone,
                    ReciveLocation = reciveLocation,
                    SendLocation = sendLocation,
                    IsNow = x.IsNow,
                    PickUpDateTime = x.PickUpDateTime,
                    ReservationStatus = x.ReservationStatus.ToString(),
                    TotallPrice = x.TotallPrice,
                    Distance = x.Distance,
                    distanceFromCurrentReservationToYou = dis ?? null,
                    HighPriorityLevel = highPriorityLevel,
                    GoodsDto = new GoodsDto
                    {
                        Width = x.Goods.Width,
                        Length = x.Goods.Length,
                        Name = x.Goods.Name,
                        Weight = x.Goods.Weight,
                        Height = x.Goods.Height
                    }
                };
                list.Add(response);
            }

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

    public async Task<OperationResult<ReservationAwaitingDriverDetailResponse>> GetAwaitingDriverReservationDetail(Guid reservationId, CurrentCoordinates currentCoordinates, DestinationCoordinates destinationCoordinates)
    {
        var result = new OperationResult<ReservationAwaitingDriverDetailResponse>();
        double? dis = null;
        bool highPriorityLevel = false;
        try
        {
            var reservation = await _unitOfWork.ReservationRepository.GetByIdAsync(reservationId);
            if (reservation!.ReservationStatus != ReservationStatus.AwaitingDriver)
            {
                result.AddError(ErrorCode.ServerError,"This reservation is no longer in AwaitingDriver status!");
                return result;
            }
            if (CheckCurrentCoordinatesHasValue(currentCoordinates))
            {
                
                if (CheckDestinationCoordinatesHasValue(destinationCoordinates))
                {
                    // goi ham check
                    var checkXemDonCoNgonKhong = await CheckXemDonCoNgonKhong(currentCoordinates, 
                        reservation.latitudeSendLocation, reservation.longitudeSendLocation, reservation.latitudeReciveLocation,
                        reservation.longitudeReceiveLocation, destinationCoordinates); 
                    if (checkXemDonCoNgonKhong.Item1)
                    {
                        highPriorityLevel = true;
                        dis = checkXemDonCoNgonKhong.Item2;
                    }
                    else
                    {
                        dis = checkXemDonCoNgonKhong.Item2;
                    }
                }
                else
                {
                    dis = await GetDistanseKm(currentCoordinates.Latitude!, currentCoordinates.Longitude!, reservation.latitudeSendLocation,
                        reservation.longitudeSendLocation);
                    if (dis! <= 10)        // if distance between driver and reservation < 10km
                    {
                        highPriorityLevel = true;
                    }
                }
            }
            var test = await _unitOfWork.ReservationRepository.GetReservationDetail(reservationId);
            var response = _mapper.Map<ReservationAwaitingDriverDetailResponse>(reservation!);
            response.HighPriorityLevel = highPriorityLevel;
            response.distanceFromCurrentReservationToYou = dis ?? null;
            result.Payload = response;
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

    public async Task<OperationResult<ReservationsResponse>> AcceptReservation(Guid driverId, Guid reservationId)
    {
        var result = new OperationResult<ReservationsResponse>();
        try
        {
            var driver = await _unitOfWork.UserRepository.GetDriverDetail(driverId);
            var claimId = _claimsService.GetCurrentUserId;
            if (!claimId.Equals(driverId))
            {
                result.AddError(ErrorCode.ServerError,"The driverID does not match the account used to login");
                return result;
            }
            // check xem tai xe này đã đăng kí xe lên hệ thống chưa
            if (driver.Vehicles.Count == 0)
            {
                result.AddError(ErrorCode.ServerError,"Your account has not registered a vehicle with the system");
                return result;
            }
            if (driver!.DriverStatus == DriverStatus.Busy)
            {
                result.AddError(ErrorCode.ServerError,"You cannot accept this order because you are currently delivering another order.");
                return result;
            }
            var reservation = await _unitOfWork.ReservationRepository.GetReservationDetail(reservationId);
            if (!reservation!.ReservationStatus.Equals(ReservationStatus.AwaitingDriver))
            {
                switch (reservation.ReservationStatus)
                {
                    case ReservationStatus.AwaitingPayment:
                        result.AddError(ErrorCode.ServerError,"đơn hàng này đang trong trạng thái chờ thanh toán");
                        break;
                    case ReservationStatus.Cancelled:
                        result.AddError(ErrorCode.ServerError,"đơn hàng này đã bị cancel");
                        break;
                    case ReservationStatus.Completed:
                        result.AddError(ErrorCode.ServerError,"đơn hàng này đã hoàn thành");
                        break;
                    default:
                        result.AddError(ErrorCode.ServerError,"đơn hàng này đã được nhận bởi tài xế khác");
                        break;
                }
                return result;
            }
            // check them tai xe nay co dung cai loai xe no yeu cau khong
            if (!(driver.Vehicles.First()!.VehicleType?.Id).Equals(reservation.reservationDetails.First()!.Service?.VehicleType?.Id))
            {
                result.AddError(ErrorCode.ServerError,$"Your vehicle does not meet the requirements of the order for: [{reservation.reservationDetails.First()!.Service?.VehicleType?.VehicleTypeName}]");
                return result;
            }
            
            // after accept the status will be change 
            driver.DriverStatus = DriverStatus.Busy;                                 
            reservation.ReservationStatus = ReservationStatus.OnTheWayToPickupPoint;
            reservation.Driver = driver;
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<ReservationsResponse>(reservation);
            
            // ở đây gọi hangfire để xóa schedule job
            
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

    public async Task<OperationResult<List<ReservationHistoryResponse>>> GetReservationHistoryForUser()
    {
        var result = new OperationResult<List<ReservationHistoryResponse>>();
        try
        {
            var loggedInUserId = _claimsService.GetCurrentUserId;
            var reHistory = await _unitOfWork.ReservationRepository.GetReservationHistoryForUser(loggedInUserId);
            result.Payload = _mapper.Map<List<ReservationHistoryResponse>>(reHistory);
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

    public async Task<OperationResult<ReservationHistoryDetailResponse>> GetReservationHistoryDetailForUser(Guid reservationId)
    {
        var result = new OperationResult<ReservationHistoryDetailResponse>();
        try
        {
            var reHistoryDetail = await _unitOfWork.ReservationRepository.GetReservationDetail(reservationId);
            if (_claimsService.Role.Equals("ADMIN"))
            {
                result.Payload = _mapper.Map<ReservationHistoryDetailResponse>(reHistoryDetail);
            }
            else
            {
                if (!_claimsService.GetCurrentUserId.Equals(reHistoryDetail.UserId))
                {
                    result.AddError(ErrorCode.ServerError,"This reservation does not belong to you.");
                    return result;
                }
                result.Payload = _mapper.Map<ReservationHistoryDetailResponse>(reHistoryDetail);
            }
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

    //--------------------------------------------------------------------------------------------------------------------------------
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

    private async Task<double?> GetDistanseKm(double originLat, double originLon, double destLat, double destLon)
    {
        try
        {
            var distance =
                await _mapService.CalculateDistanceBetweenTwoCoordinates(originLat, originLon, destLat, destLon);
            var km = distance / 1000;
            return km;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    private async Task<(bool,double?)> CheckXemDonCoNgonKhong(CurrentCoordinates currentCoordinates, double originLat, double originLon,
        double destLat, double destLon, DestinationCoordinates coordinates)
    {
        var coordinateDistance = await _mapService.CalculateDistanceBetweenFourCoordinates(currentCoordinates, originLat, originLon,
            destLat, destLon, coordinates);
        // thực hiện logic 
        var abc = coordinateDistance!.DistanceFromDriverToReservation +
                coordinateDistance.DistanceFromReservationToDestinationReservation +
                coordinateDistance.DistanceFromDestinationReservationToDriverTargetDestination;
        var ad = coordinateDistance.DistanceFromDriverToDriverTargetDestination;
        if (abc <= ad + 15000) // neu khoang cach lay hang roi di ve khong lon hon qua 15km so voi khoang cach ban dau thi day la 1 don hang ngon
        {
            return (true,coordinateDistance.DistanceFromDriverToReservation/1000);
        }
        else
        {
            return (false,coordinateDistance.DistanceFromDriverToReservation/1000);
        }
    }

    private bool CheckCurrentCoordinatesHasValue(CurrentCoordinates currentCoordinates)
    {
        if (currentCoordinates.Longitude == 0.0D || currentCoordinates.Latitude == 0.0D)
        {
            return false;
        }
        return true;
    }
    private bool CheckDestinationCoordinatesHasValue(DestinationCoordinates destinationCoordinates)
    {
        if (destinationCoordinates.LatitudeDes == 0.0D || destinationCoordinates.LongitudeDes == 0.0D)
        {
            return false;
        }
        return true;
    }

    private string CatChuoi(string s)
    {
        string result = "";
        var x = "";
        if (!s.Contains(','))
        {
            return s;
        }
        string[] arrListStr = s.Split(',');
        for (int i = arrListStr.Length - 1; i > 0; i--) {
            result += arrListStr[i-1] +","+ arrListStr[i];
            x = result.Trim();
            break;
        }
        return x;
    }
}