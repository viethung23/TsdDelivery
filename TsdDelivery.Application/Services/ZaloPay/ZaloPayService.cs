using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models.Reservation.Request;
using TsdDelivery.Application.Services.ZaloPay.Request;
using TsdDelivery.Application.Utils;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Services.ZaloPay;

public class ZaloPayService : IZaloPayService
{
    private readonly AppConfiguration _configuration;
    private readonly ICurrentTime _currentTime;
    public ZaloPayService(AppConfiguration appConfiguration,ICurrentTime currentTime)
    {
        _configuration = appConfiguration;
        _currentTime = currentTime;
    }
    
    public async Task<(bool, string?)> CreateZaloPayment(CreateReservationRequest request, Reservation reservation)
    {
        var zalopayPayRequest = new CreateZalopayPayRequest(_configuration.ZaloPayConfig.AppId, _configuration.ZaloPayConfig.AppUser,
            _currentTime.GetCurrentTime().GetTimeStamp(), (long)request.TotalPrice!, _currentTime.GetCurrentTime().ToString("yyMMdd") + "_" + reservation.Code,
            "zalopayapp", $"Thanh toán đặt xe TSD",_configuration.ZaloPayConfig.CallbackUrl);
        zalopayPayRequest.MakeSignature(_configuration.ZaloPayConfig.Key1);
        return  zalopayPayRequest.GetLink(_configuration.ZaloPayConfig.PaymentUrl);
    }
}