using FluentStockMarket;

namespace FluentAPI;

public class AnalysisAPI : IStartDateSelectionStage, IEndDateSelectionStage
{
    //instance members
    private readonly List<DailyPriceData> _data;
    private DateTime _startDate;
    private DateTime _endDate;

    //ctor
    private AnalysisAPI(List<DailyPriceData> data) { _data = data; }

    //fluent methods for defining date range
    public static AnalysisAPI Initialise(List<DailyPriceData> data)
    {
        return new AnalysisAPI(data);
    }
    
    public IStartDateSelectionStage From(string date)
    {
        _startDate = DateTime.Parse(date);
        return this;
    }

    public IEndDateSelectionStage To(string date)
    {
        _endDate = DateTime.Parse(date);
        return this;
    }

    public (decimal high, decimal low, decimal sma, decimal ema) Analyse()
    {
        var analysisPeriod = _data
            .Where(item => item.Date >= _startDate && item.Date <= _endDate)
            .ToList();

        var high = High(analysisPeriod);
        var low = Low(analysisPeriod);
        var sma = SMA(analysisPeriod);
        var ema = EMA(analysisPeriod);

        return (high, low, sma, ema);
    }

    //analysis methods 
    private decimal High(List<DailyPriceData> data)
    {
        return data.Max(item => item.High);
    }
    private decimal Low(List<DailyPriceData> data)
    {
        return data.Min(item => item.Low);
    }
    private decimal SMA(List<DailyPriceData> data)
    {
        var closePrices = data.Select(item => item.Close).ToList();
        decimal sma = closePrices.Average();

        return sma;
    }
    private decimal EMA(List<DailyPriceData> data)
    {
        var closePrices = data.Select(item => item.Close).ToList();
        decimal smoothingFactor = 2.0m / (closePrices.Count + 1);

        decimal initialEMA = closePrices.First();

        decimal ema = closePrices.Skip(1).Aggregate(initialEMA, (currentEMA, closePrice) =>
        {
            return (closePrice - currentEMA) * smoothingFactor + currentEMA;
        });

        return ema;
    }
}

#region FluentInterfaces

public interface IStartDateSelectionStage
{
    public IEndDateSelectionStage To(string date);
}

public interface IEndDateSelectionStage
{
    public (decimal high, decimal low, decimal sma, decimal ema) Analyse();
} 

#endregion
