using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MagnisStockExchange.Controllers
{
    public class StockExchangeController : Controller
    {
        /// <summary>
        /// Get list of pair 
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/symbols")]
        public IActionResult GetSymbols()
        {
            var listOfPair = new List<string>()
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

            return Json(listOfPair);
        }

    }
}