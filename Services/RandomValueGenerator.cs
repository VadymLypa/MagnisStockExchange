using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagnisStockExchange.Abstractions;

namespace MagnisStockExchange.Services
{
    public class RandomGenerateService : IRandomGenerateValue
    {
        private static readonly Random _random = new Random();

        public async Task<List<StockExchange>> GenerateData()
        {
            var stocksList = new List<StockExchange>();                                                                                      

            for (int i = 0; i < 1000; i++)
            {
                stocksList.Add(new StockExchange()
                {
                    Symbol = GetRandomSymbol(),
                    Volume = GetRandomVolume(),
                    Price = GetRandomPrice(-30000, 40000),
                    DateTime = GetRandomDateTime(DateTime.Now.AddYears(-8), DateTime.Now)
                });
            }
            return stocksList;
        }

        public string GetRandomSymbol()
        {
            var symbolsList = new List<string>
            {
                "AUD/CAD",
                "AUD/CHF",
                "AUD/JPY",
                "AUD/NZD",
                "AUD/USD",
                "BGN/RON",
                "CAD/CHF",
                "CAD/JPY",
                "CHF/BGN",
                "CHF/JPY"
            };
            return symbolsList[_random.Next(0, symbolsList.Count)];
        }

        public long GetRandomPrice(long minValue, long maxValue)
        {
            byte[] buf = new byte[8];
            _random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (maxValue - minValue)) + minValue);
        }

        public int GetRandomVolume()
        {
            return _random.Next(Int32.MinValue, Int32.MaxValue);
        }

        public DateTime GetRandomDateTime(DateTime from, DateTime to)
        {
            var range = to - from;
            var randTimeSpan = new TimeSpan((long)(_random.NextDouble() * range.Ticks));

            return from + randTimeSpan;
        }
    }
}
