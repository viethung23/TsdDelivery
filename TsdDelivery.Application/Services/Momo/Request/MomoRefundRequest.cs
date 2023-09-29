using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TsdDelivery.Application.Services.Momo.Response;
using TsdDelivery.Application.Utils;

namespace TsdDelivery.Application.Services.Momo.Request;

public class MomoRefundRequest
{
    public string partnerCode { get; set; } = string.Empty;
    public string orderId { get; set; } = string.Empty;
    public string requestId { get; set; } = string.Empty;
    public long amount { get; set; }
    public long transId { get; set; } 
    public string lang { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public string signature { get; set; } = string.Empty;
    
    public MomoRefundRequest(string partnerCode, string requestId,
        long amount, string orderId,long transId,string lang = "vi")
    {
        this.partnerCode = partnerCode;
        this.requestId = requestId;
        this.amount = amount;
        this.orderId = orderId;
        this.lang = lang;
        this.transId = transId;
    }
    
    public void MakeSignature(string accessKey, string secretKey)
    {
        var rawHash = "accessKey=" + accessKey +
                      "&amount=" + this.amount +
                      "&description=" + this.description +
                      "&orderId=" + this.orderId +
                      "&partnerCode=" + this.partnerCode +
                      "&requestId=" + this.requestId +
                      "&transId=" + this.transId;
        this.signature = HashHelper.HmacSHA256(rawHash, secretKey);
    }
    
    public (bool, string?, MomoRefundResponse?) Refund(string refundUrl)
    {
        using HttpClient client = new HttpClient();
        var requestData = JsonConvert.SerializeObject(this, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
        });
        var requestContent = new StringContent(requestData, Encoding.UTF8,"application/json");

        var createPaymentLinkRes = client.PostAsync(refundUrl, requestContent).Result;

        if(createPaymentLinkRes.IsSuccessStatusCode)
        {
            var responseContent = createPaymentLinkRes.Content.ReadAsStringAsync().Result;
            var responseData = JsonConvert
                .DeserializeObject<MomoRefundResponse>(responseContent);
            if(responseData!.resultCode == "0")
            {
                return (true,responseData.message, responseData);
            }
            else
            {
                return (false,responseData.message, responseData);
            }
        }
        else
        {
            return (false, createPaymentLinkRes.ReasonPhrase,null);
        }
    }
}