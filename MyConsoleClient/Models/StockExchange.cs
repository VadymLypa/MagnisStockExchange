using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleClient.Models
{
    public class StockExchange
    {
        public string Symbol { get; set; }
        public long Price { get; set; }
        public int Volume { get; set; }
        public DateTime DateTime { get; set; }
    }
}
