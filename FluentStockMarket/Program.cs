using FluentAPI;

Console.WriteLine("Stock Data Analyser: Please enter a valid ticker symbol...");

string ticker = Console.ReadLine();
var data = await AlphaVantageService.GetSPYDailyPriceDataAsync(ticker ?? "SPY");

Console.WriteLine("Enter a date to start analysis...");
string from = Console.ReadLine() ?? DateTime.Now.ToString("en-GB");

Console.WriteLine("Enter a date to end analysis...");
string to = Console.ReadLine() ?? DateTime.Now.ToString("en-GB");
    
(decimal high, decimal low, decimal sma, decimal ema) = AnalysisAPI
    .Initialise(data)
    .From(from)
    .To(to)
    .Analyse();

Console.WriteLine($"For the stock {ticker}");
Console.WriteLine($"The high during this period was: ${high}");
Console.WriteLine($"The low during this period was: ${low}");
Console.WriteLine($"The SMA for this period was: ${sma}");
Console.WriteLine($"The EMA during this period was: ${ema}");