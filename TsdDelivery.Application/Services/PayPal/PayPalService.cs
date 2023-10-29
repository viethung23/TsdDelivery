using System.Net.Http.Headers;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using TsdDelivery.Application.Models;
using TsdDelivery.Application.Models.Reservation.Request;
using TsdDelivery.Application.Services.Momo.Request;
using TsdDelivery.Application.Services.PayPal.Request;
using TsdDelivery.Application.Utils;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Services.PayPal;

public class PayPalService : IPayPalService
{
    private readonly AppConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public PayPalService(AppConfiguration appConfiguration,IUnitOfWork unitOfWork)
    {
        _configuration = appConfiguration;
        _unitOfWork = unitOfWork;
    }
    public async Task<(bool, string?, string?)> CreatePaypalPayment(CreateReservationRequest request, Reservation reservation)
    {
        var token = await GenerateAccessToken(_configuration.PaypalConfig.ClientId, _configuration.PaypalConfig.SecretKey, _configuration.PaypalConfig.PayPalUrl);
        var payPalPaymentRequest = new PayPalPaymentRequest("CAPTURE", "Thanh toán đặt xe TSD",reservation.Id.ToString(), await reservation.TotallPrice.ConvertToUsdAsync(_configuration.ApiLayerConfig.ExchangeRateKey),
            _configuration.PaypalConfig.ReturnUrl, _configuration.PaypalConfig.CancelUrl,token);
        var link = await payPalPaymentRequest.GetLink(_configuration.PaypalConfig.PayPalUrl);
        if (link.IsNullOrEmpty())
        {
            return (false, null, null);
        }
        return (true, link, link);
    }

    /*public async Task<OperationResult<string>> ProcessPayPalPaymentReturn(MomoOneTimePaymentResultRequest paymentResultRequest)
    {
        var result = new OperationResult<string>();
        const string methodName = "AutoCancelReservationWhenOverAllowPaymentTime";
        try
        {
            
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
    }*/

    public async Task<string> GenerateAccessToken(string clientId, string clientSecret, string paypalUrl)
    {
        using HttpClient client = new HttpClient();

        // Thiết lập header Authorization dùng Basic Auth
        var basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthValue);

        // Body request dạng x-www-form-urlencoded
        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        // Gọi API
        var response = await client.PostAsync($"{paypalUrl}/v1/oauth2/token", requestData);

        // Xử lý response lấy access token 
        var responseString = await response.Content.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(responseString);
        return data.access_token;
    }
}