using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace TsdDelivery.Application.Services.PayPal.Request;

public class PayPalPaymentRequest
{
    public string Intent { get; set; }
    public string Description { get; set; }
    public string Token { get; set; }
    public string ReferenceId { get; set; } 
    public decimal Amount { get; set; }
    public string Return_url { get; set; }
    public string Cancel_url { get; set; }

    public PayPalPaymentRequest(string intent, string description,string referenceId, decimal amount, string returnUrl, string cancelUrl,string token)
    {
        this.Intent = intent;
        this.Description = description;
        this.Amount = amount;
        this.Return_url = returnUrl;
        this.Cancel_url = cancelUrl;
        this.ReferenceId = referenceId;
        this.Token = token;
    }
    
    public async Task<string> GetLink(string paypalUrl)
    {
        using HttpClient client = new HttpClient();
        string link = "";

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
        
        var requestData = $@"{{
        ""intent"":""{Intent}"",
        ""purchase_units"":[
          {{
            ""reference_id"":""{ReferenceId}"",
            ""description"":""{Description}"",
            ""amount"":{{
              ""currency_code"":""USD"",
              ""value"":""{Amount}""    
            }}
          }}
        ],
        ""payment_source"":{{
          ""paypal"":{{
            ""experience_context"":{{
              ""return_url"":""{Return_url}"",
              ""cancel_url"":""{Cancel_url}""
            }}
           }}
        }}
      }}";
        
        var httpContent = new StringContent(requestData, Encoding.UTF8, "application/json");
        // Gửi request
        var response = await client.PostAsync($"{paypalUrl}/v2/checkout/orders", httpContent);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(result);
            link = data.links[1].href;
            return link;
        }
        return link;
    }
}