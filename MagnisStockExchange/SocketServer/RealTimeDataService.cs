using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using MagnisStockExchange.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MagnisStockExchange.WebSockets
{
    public class RealTimeDataService : WebSocketHandler
    {
        private readonly ConcurrentDictionary<string, HashSet<WebSocket>> _rooms;
        private readonly IRandomGenerateValue _randomGenerateValue;

        private List<string> _symbols = new List<string>()
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

        public RealTimeDataService(WebSocketConnectionManager webSocketConnectionManager,
            IRandomGenerateValue randomGenerateValue) : base(webSocketConnectionManager)
        {
            _rooms = new ConcurrentDictionary<string, HashSet<WebSocket>>();
            _randomGenerateValue = randomGenerateValue;
        }
        
        public override async Task ReceiveAsync(WebSocket sender, WebSocketReceiveResult result, byte[] buffer)
        {
            string roomName = Encoding.UTF8.GetString(buffer, 0, result.Count);

            if (_symbols.Contains(roomName))
            {
                if (_rooms.ContainsKey(roomName))
                {
                    var roomMember = _rooms[roomName];

                    //already subscribe in symbol
                    if (roomMember.Contains(sender))
                    {
                        return;
                    }

                    roomMember.Add(sender);
                    await StartDataFlow();
                }

                //created room and subscribe 
                else
                {
                    _rooms.TryAdd(roomName, new HashSet<WebSocket>()
                    {
                        sender
                    });
                    await StartDataFlow();
                }
            }

            //doesn't existing room
            else
            {
                return;
            }
        }

        private async Task StartDataFlow()
        {
            while (true)
            {
                await Task.Run(async () => await SendData());
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private  async Task SendData()
        {
            var realTimeData = await _randomGenerateValue.GenerateData();

            foreach (var stockExchange in realTimeData)
            {
                await SendDataToRoom(stockExchange.Symbol, stockExchange);
            }
        }

        private async Task SaveDataAsync(string roomName, StockExchange dataExchange)
        {
            using (var context = new StockExchangeContext())
            {
                var updateStockExchange = await context.StockExchange.FirstOrDefaultAsync(s => s.Symbol == roomName);
                if (updateStockExchange != null)
                {
                    updateStockExchange.Price = dataExchange.Price;
                    updateStockExchange.Volume = dataExchange.Volume;
                    updateStockExchange.DateTime = dataExchange.DateTime;

                    await context.SaveChangesAsync();
                }
                else
                {
                    await context.AddAsync(dataExchange);
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task SendDataToRoom(string roomName, StockExchange dataExchange)
        {
            if (_rooms.ContainsKey(roomName))
            {
                await SaveDataAsync(roomName, dataExchange);

                var webSocket = _rooms[roomName];
                foreach (var socket in webSocket)
                {
                    await SendExchangeData(socket, dataExchange);
                }
            }
        }
    }
}
