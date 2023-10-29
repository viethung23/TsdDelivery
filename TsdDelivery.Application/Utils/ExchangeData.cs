

using Newtonsoft.Json;

namespace TsdDelivery.Application.Utils;

public static class ExchangeData
{
    public static async Task<decimal> ConvertToUsdAsync(this decimal amount, string exchangeRateKey)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("apikey", exchangeRateKey);
      
            var request = new HttpRequestMessage(HttpMethod.Get, 
                $"https://api.apilayer.com/exchangerates_data/convert?to=USD&from=VND&amount={amount}");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
      
            // Parse response to get converted amount
            dynamic convertedAmount = JsonConvert.DeserializeObject(responseString);
            var amountUsd = (decimal)convertedAmount.result;
            var value = Math.Round(amountUsd, 2);
            return value;
        }
    }
}