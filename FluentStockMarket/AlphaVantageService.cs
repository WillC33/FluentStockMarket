using FluentStockMarket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FluentAPI;

public static class AlphaVantageService
{
    
    //static members
    private const string ApiKey = devenv.API_KEY;
    private static readonly HttpClient HttpClient = new();
    
    public static async Task<List<DailyPriceData>> GetSPYDailyPriceDataAsync(string symbol)
    {
        var response = await HttpClient.GetAsync($"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={ApiKey}");

        if (!response.IsSuccessStatusCode) return new List<DailyPriceData>();
        
        var content = await response.Content.ReadAsStringAsync();
        var jsonData = JsonConvert.DeserializeObject<JObject>(content);

        if (jsonData["Time Series (Daily)"] is not JObject dailyData) return new List<DailyPriceData>();
        var dailyPriceDataList = new List<DailyPriceData>();

        foreach (var item in dailyData)
        {
            if (!DateTime.TryParse(item.Key, out DateTime date) ||
                item.Value is not JObject dailyValues) continue;
            
            var dailyPriceData = new DailyPriceData
            {
                Date = date,
                Open = Convert.ToDecimal(dailyValues["1. open"]),
                High = Convert.ToDecimal(dailyValues["2. high"]),
                Low = Convert.ToDecimal(dailyValues["3. low"]),
                Close = Convert.ToDecimal(dailyValues["4. close"]),
                Volume = Convert.ToInt64(dailyValues["5. volume"])
            };

            dailyPriceDataList.Add(dailyPriceData);
        }
        return dailyPriceDataList;
    }
}