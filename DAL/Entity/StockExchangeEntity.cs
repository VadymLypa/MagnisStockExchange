using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    public class StockExchangeEntity
    {
        [Key]
        public int Id { get; set; }

        public string Symbol { get; set; }
        public long Price { get; set; }
        public int Volume { get; set; }
        public DateTime DateTime { get; set; }

    }
}
