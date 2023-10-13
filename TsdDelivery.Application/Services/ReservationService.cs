using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using StackExchange.Redis;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Coordinates;
using TsdDelivery.Application.Models.Reservation.DTO;
using TsdDelivery.Application.Models.Reservation.Enum;
using TsdDelivery.Application.Models.Reservation.Request;
using TsdDelivery.Application.Models.Reservation.Response;
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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConnectionMultiplexer _redisConnection;

    public ReservationService(IUnitOfWork unitOfWork, IMapper mapper, AppConfiguration appConfiguration,
        ICurrentTime currentTime, IClaimsService claimsService, IMapService mapService, IMomoService momoService,
        IZaloPayService zaloPayService,IHttpContextAccessor httpContextAccessor,IConnectionMultiplexer redisConnection)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = appConfiguration;
        _currentTime = currentTime;
        _claimsService = claimsService;
        _mapService = mapService;
        _momoService = momoService;
        _zaloPayService = zaloPayService;
        _httpContextAccessor = httpContextAccessor;
        _redisConnection = redisConnection;
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
        CreateReservationResponse createReservationResponse = new CreateReservationResponse();
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                // phải check xem thg request đã đang đặt đơn nào không vì nếu đơn chưa hoàn thành hoặc hủy thì k cho đặt đơn mới 
                if (await IsUserOrdering(_claimsService.GetCurrentUserId))
                {
                    throw new Exception("Bạn đã có đơn hàng trong quá trình xử lý, không thể thực hiện đặt cho tới khi đơn hàng hoàn thành hoặc bị hủy");
                }

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
                    PickUpDateTime = request.IsNow == true
                        ? _currentTime.GetCurrentTime()
                        : request.PickUpDateTime!.Value,
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
                        (bool createMomoLinkResult, string createMessage, string deepLink) =
                            await _momoService.CreateMomoPayment(request, entity);
                        if (createMomoLinkResult)
                        {
                            createReservationResponse.Id = entity.Id;
                            createReservationResponse.PaymentUrl = createMessage;
                            createReservationResponse.deeplink = deepLink;
                            result.Payload = createReservationResponse;
                        }
                        else
                        {
                            throw new Exception(createMessage);
                        }

                        break;

                    case "ZALOPAY":
                        (bool createZaloPayLinkResult, string? createZaloPayMessage) =
                            await _zaloPayService.CreateZaloPayment(request, entity);
                        if (createZaloPayLinkResult)
                        {
                            createReservationResponse.Id = entity.Id;
                            createReservationResponse.PaymentUrl = createZaloPayMessage;
                            createReservationResponse.deeplink = null;
                            result.Payload = createReservationResponse;
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

                // lấy host từ request
                var requestContext = _httpContextAccessor?.HttpContext?.Request;
                var clientHost = requestContext?.Headers["X-Client-Host"].ToString();
                // gọi cache ra set host và với key là ID reservation
                IDatabase cache = _redisConnection.GetDatabase();
                cache.StringSet(entity.Id.ToString(), clientHost, TimeSpan.FromMinutes(20));
                var json = JsonConvert.SerializeObject(createReservationResponse);
                cache.StringSet("Payment_" + entity.Id, json,TimeSpan.FromMinutes(10));

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

    public async Task<OperationResult<ReservationsResponse>> DriverReservationAction(Guid driverId, Guid reservationId, DriverReservationAction action)
    {
        var result = new OperationResult<ReservationsResponse>();
        try
        {
            var include = new [] {"Wallet"};
            // fix cứng Admin 
            var admin = await _unitOfWork.UserRepository.GetSingleByCondition(x => x.Role.RoleName == "ADMIN", include);
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
            
            var reservation = await _unitOfWork.ReservationRepository.GetReservationDetail(reservationId);
            // check them tai xe nay co dung cai loai xe no yeu cau khong
            if (!(driver.Vehicles.First()!.VehicleType?.Id).Equals(reservation.reservationDetails.First()!.Service?.VehicleType?.Id))
            {
                result.AddError(ErrorCode.ServerError,$"Your vehicle does not meet the requirements of the order for: [{reservation.reservationDetails.First()!.Service?.VehicleType?.VehicleTypeName}]");
                return result;
            }
            switch (action)
            {
                case Models.Reservation.Enum.DriverReservationAction.Accept : 
                    // Logic case 1
                    if (driver!.DriverStatus == DriverStatus.Busy)
                    {
                        result.AddError(ErrorCode.ServerError,"You cannot accept this order because you are currently delivering another order.");
                        return result;
                    }
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
                    // after accept the status will be change 
                    driver.DriverStatus = DriverStatus.Busy;                                 
                    reservation.ReservationStatus = ReservationStatus.OnTheWayToPickupPoint;
                    //reservation.ReservationStatus = ReservationStatus.AwaitingDriver;
                    reservation.Driver = driver;
                    await _unitOfWork.SaveChangeAsync();
                    result.Payload = _mapper.Map<ReservationsResponse>(reservation);
                    break;
                
                case Models.Reservation.Enum.DriverReservationAction.Delivery : 
                    // Logic case 2
                    if (driver!.DriverStatus != DriverStatus.Busy || !reservation.DriverId.Equals(driverId) || reservation.ReservationStatus != ReservationStatus.OnTheWayToPickupPoint)
                    {
                        result.AddError(ErrorCode.ServerError,$"Cannot do this action {action.ToString()}");
                        return result;
                    }
                    reservation.ReservationStatus = ReservationStatus.InDelivery;
                    await _unitOfWork.SaveChangeAsync();
                    result.Payload = _mapper.Map<ReservationsResponse>(reservation);
                    break;
                
                case Models.Reservation.Enum.DriverReservationAction.Done :
                    // Logic case 3
                    if (driver!.DriverStatus != DriverStatus.Busy || !reservation.DriverId.Equals(driverId) || reservation.ReservationStatus != ReservationStatus.InDelivery)
                    {
                        result.AddError(ErrorCode.ServerError,$"Cannot do this action {action.ToString()}");
                        return result;
                    }
                    reservation.ReservationStatus = ReservationStatus.Completed;
                    driver.DriverStatus = DriverStatus.Available;
                    
                    // add to driver wallet
                    var priceForDriver = reservation.TotallPrice * 0.7M;
                    driver.Wallet!.Balance += priceForDriver;
                    admin.Wallet!.Balance -= priceForDriver;

                    var transactionForAdmin = new Transaction()
                    {
                        Price = priceForDriver,
                        Status = TransactionStatus.success.ToString(),
                        PaymentMethod = "Thanh-toan-online",
                        Description = "Thanh toán cho tài xế đã nhận đơn hàng có mã : " + reservation.Id,
                        WalletId = admin.Wallet!.Id,
                        ReservationId = reservation.Id,
                        TransactionType = TransactionType.Minus
                    };
                    
                    var transactionForDriver = new Transaction()
                    {
                        Price = priceForDriver,
                        Status = TransactionStatus.success.ToString(),
                        PaymentMethod = "Thanh-toan-online",
                        Description = "Nhận tiền hoàn thành đơn hàng có mã : " + reservation.Id,
                        WalletId = driver.Wallet!.Id,
                        ReservationId = reservation.Id,
                        TransactionType = TransactionType.Plus
                    };

                    await _unitOfWork.TransactionRepository.AddRangeAsync(new List<Transaction>(){transactionForAdmin,transactionForDriver});
                    var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                    if (!isSuccess)
                    {
                        result.AddError(ErrorCode.ServerError,"Can not save");
                        return result;
                    }
                    result.Payload = _mapper.Map<ReservationsResponse>(reservation);
                    break;
            }

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

    public async Task<OperationResult<ReservationsResponse>> GetCurrentAcceptedReservationbyDriver(Guid dirverId)
    {
        var result = new OperationResult<ReservationsResponse>();
        try
        {
            var include = new[] { "User","Driver" };
            
            var re = await _unitOfWork.ReservationRepository
                .GetSingleByCondition(x => x.DriverId == dirverId && 
                                           (x.ReservationStatus == ReservationStatus.OnTheWayToPickupPoint || x.ReservationStatus == ReservationStatus.InDelivery),include);
            if (re.Driver!.DriverStatus == DriverStatus.Available)
            {
                result.AddError(ErrorCode.NoContent,"Tài khoản của bạn đang trong trạng thái busy");
                return result;
            }
            result.Payload = _mapper.Map<ReservationsResponse>(re);
        }
        catch (Exception e)
        {
            result.AddError(ErrorCode.NoContent,"khong tim thay don hang");
        }
        finally
        {
            _unitOfWork.Dispose();
        }
        return result;
    }

    public async Task<OperationResult<ReservationResponsee>> GetCurrentAcceptedReservationbyUser(Guid userId)
    {
        var result = new OperationResult<ReservationResponsee>();
        try
        {
            var re = await _unitOfWork.ReservationRepository
                .GetSingleByCondition(x => x.UserId == userId && 
                                           (x.ReservationStatus == ReservationStatus.OnTheWayToPickupPoint 
                                            || x.ReservationStatus == ReservationStatus.InDelivery
                                            || x.ReservationStatus == ReservationStatus.AwaitingPayment
                                            || x.ReservationStatus == ReservationStatus.AwaitingDriver));
            var dto = _mapper.Map<ReservationResponsee>(re);
            if (re.ReservationStatus == ReservationStatus.AwaitingPayment)
            {
                // lấy cái link lưu trong redis ra để show lên
                var cache = _redisConnection.GetDatabase();
                var paymentLink = cache.StringGet("Payment_"+re.Id).ToString();
                var paymentObj = JsonConvert.DeserializeObject<CreateReservationResponse>(paymentLink);
                dto.LinkPayment = paymentObj;
            }
            result.Payload = dto;
        }
        catch (Exception e)
        {
            result.AddError(ErrorCode.NoContent,"khong tim thay don hang");
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

    private async Task<bool> IsUserOrdering(Guid userId)
    {
        try
        {
            var re = await _unitOfWork.ReservationRepository
                .GetSingleByCondition(x => x.UserId == userId &&
                                           (x.ReservationStatus == ReservationStatus.OnTheWayToPickupPoint
                                            || x.ReservationStatus == ReservationStatus.InDelivery
                                            || x.ReservationStatus == ReservationStatus.AwaitingPayment
                                            || x.ReservationStatus == ReservationStatus.AwaitingDriver));
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }
}