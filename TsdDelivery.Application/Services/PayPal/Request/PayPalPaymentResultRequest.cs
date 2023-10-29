using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;

namespace TsdDelivery.Application.Services.PayPal.Request;

public class PayPalPaymentResultRequest
{
    public const string APPROVED = "APPROVED";  // The customer approved the payment through the PayPal wallet or another form of guest or unbranded payment. For example, a card, bank account, or so on.
    public const string COMPLETED = "COMPLETED"; // The payment was authorized or the authorized payment was captured for the order.
    public const string PAYER_ACTION_REQUIRED = "PAYER_ACTION_REQUIRED";
    public const string CREATED = "CREATED";
    public const string SAVED = "SAVED";
    public const string VOIDED = "VOIDED";
    
    public string Token { get; set; }
    public string PayerID { get; set; }

    public async Task<bool> IsApproved(string paypalUrl,string accessToken)
    {
        using HttpClient client = new HttpClient();
        //client.DefaultRequestHeaders.Add("Authorization",$"Bearer {AccessToken}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.GetAsync($"{paypalUrl}/v2/checkout/orders/{Token}");
        
        var content = await response.Content.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(content);

        if (data.status == APPROVED)
        {
            return true;
        }
        return false;
    }

    public async Task<(Guid,string)> CapturePaymentForOrder(string paypalUrl,string accessToken)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Add("Prefer","return=representation");
        
        var emptyContent = new StringContent("",Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{paypalUrl}/v2/checkout/orders/{Token}/capture", emptyContent);

        var content = await response.Content.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(content);
        var reference_id = data.purchase_units[0].reference_id;
        var capture_id = data.purchase_units[0].payments.captures[0].id;
        return (reference_id,capture_id);
    }
}