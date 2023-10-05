using Newtonsoft.Json;
using TsdDelivery.Application.Services.ZaloPay.Response;
using TsdDelivery.Application.Utils;

namespace TsdDelivery.Application.Services.ZaloPay.Request;

public class CreateZalopayPayRequest
{
    public CreateZalopayPayRequest(int appId, string appUser, long appTime,
            long amount, string appTransId, string bankCode, string description,string callbackUrl)
        {
            AppId = appId;
            AppUser = appUser;
            AppTime = appTime;
            Amount = amount;
            AppTransId = appTransId;
            BankCode = bankCode;
            Description = description;
            CallbackUrl = callbackUrl;
        }
        public int AppId { get; set; }
        public string AppUser { get; set; } = string.Empty;
        public long AppTime { get; set; }
        public long Amount { get; set; }
        public string AppTransId { get; set; } = string.Empty;
        public string ReturnUrl { get; }
        public string EmbedData { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public string Mac { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public void MakeSignature(string key)
        {
            var data = AppId + "|" + AppTransId + "|" + AppUser + "|" + Amount + "|"
              + AppTime + "|" + "{}" + "|" + "[]";

            this.Mac = HashHelper.HmacSHA256(data, key);
        }

        public Dictionary<string, string> GetContent()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            keyValuePairs.Add("app_id", AppId.ToString());
            keyValuePairs.Add("app_user", AppUser);
            keyValuePairs.Add("app_time", AppTime.ToString());
            keyValuePairs.Add("amount", Amount.ToString());
            keyValuePairs.Add("app_trans_id", AppTransId);
            keyValuePairs.Add("embed_data", "{}");
            keyValuePairs.Add("item", "[]");
            keyValuePairs.Add("callback_url",CallbackUrl);
            keyValuePairs.Add("description", Description);
            keyValuePairs.Add("bank_code", "zalopayapp");
            keyValuePairs.Add("mac", Mac);

            return keyValuePairs;
        }

        public (bool, string) GetLink(string paymentUrl)
        {
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(GetContent());
            var response = client.PostAsync(paymentUrl, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var responseData = JsonConvert
                    .DeserializeObject<CreateZalopayPayResponse>(responseContent);
                if (responseData.return_code == 1)
                {
                    return (true, responseData.order_url);
                }
                else
                {
                    return (false, responseData.return_message);
                }
            }
            else
            {
                return (false, response.ReasonPhrase ?? string.Empty);
            }
        }
}