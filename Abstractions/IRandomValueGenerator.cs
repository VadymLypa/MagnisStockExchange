using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MagnisStockExchange.Abstractions
{
    public interface IRandomGenerateValue
    {
        Task<List<StockExchange>> GenerateData();
        string GetRandomSymbol();
        long GetRandomPrice(long minValue, long maxValue);
        int GetRandomVolume();
        DateTime GetRandomDateTime(DateTime from, DateTime to);
    }
}
