using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MagnisStockExchange.WebSockets
{
    public abstract class WebSocketHandler
    {
        public WebSocketConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socket);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;
            try
            {
                await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                        offset: 0,
                        count: message.Length),
                    messageType: WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None).ConfigureAwait(false);

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task SendExchangeData(WebSocket socket, StockExchange member)
        {
            if (socket.State != WebSocketState.Open)
                return;
            
                var sendStock = JsonConvert.SerializeObject(new StockExchange
                {
                    Symbol = member.Symbol,
                    Volume = member.Volume,
                    Price = member.Price,
                    DateTime = member.DateTime
                });

            Monitor.Enter(socket);

            try
            {
                await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(sendStock),
                        offset: 0,
                        count: sendStock.Length),
                    messageType: WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None);
            }
            finally
            {
                Monitor.Exit(socket);
            }                                 
        }

        public abstract Task ReceiveAsync(WebSocket sender, WebSocketReceiveResult result, byte[] buffer);
    }
}
