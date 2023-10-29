using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TsdDelivery.Application.Services.PayPal.Request;

public class PayPalRefundRequest
{
    public string Note_to_payer { get; set; }

    public PayPalRefundRequest(string note_to_payer)
    {
        this.Note_to_payer = note_to_payer;
    }
    
    public async Task<bool> Refund(string paypalUrl,string capture_id,string accessToken)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Add("Prefer", "return=representation");

        var requestData = JsonConvert.SerializeObject(this);
        HttpContent httpContent = new StringContent(requestData, Encoding.UTF8, "application/json");
        
        var response = client.PostAsync($"{paypalUrl}/v2/payments/captures/{capture_id}/refund", httpContent).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseData = response.Content.ReadAsStringAsync().Result;
            dynamic data = JsonConvert.DeserializeObject(responseData);
            dynamic status = data.status;
            if (status == "COMPLETED")
            {
                return true;
            }
        }
        return false;
    }
    
}