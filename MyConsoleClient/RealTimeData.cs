using MyConsoleClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using WebSocket4Net;

namespace MyConsoleClient
{ 
    public class RealTimeData
    {
        private WebSocket _websocketClient;
        public static string URL = "ws://localhost:65055/chat";

        public void SocketHandler()
        {
            _websocketClient = new WebSocket(URL);

            _websocketClient.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocketClient_Error);
            _websocketClient.Opened += new EventHandler(websocketClient_Opened);
            _websocketClient.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocketClient_MessageReceived);
        }

        public void StartFeedData()
        {
            _websocketClient.Open();
            char keyStroked;

            while (true)
            {
                ShowAvailableOptions();
                keyStroked = Console.ReadKey().KeyChar;

                if (keyStroked.Equals('q'))
                {
                    _websocketClient.Close();
                    return;
                }
                else
                {
                    if (keyStroked.Equals('s'))
                    {
                        Console.Write("\nPlease choose symbol : ");
                        var message = Console.ReadLine();
                        _websocketClient.Send(message);
                    }
                }
            }
        }

        public void GetSymbols()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:65055/");
                client.DefaultRequestHeaders.Accept.Clear();

                var response = client.GetAsync("api/symbols").Result;
                if (response.IsSuccessStatusCode)
                {

                    var json = response.Content.ReadAsStringAsync().Result;

                    List<string> symbols = JsonConvert.DeserializeObject<List<string>>(json);

                    foreach (var symbol in symbols)
                    {
                        Console.WriteLine(symbol);
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
        }

        private void ShowAvailableOptions()
        {
            Console.WriteLine();
            Console.WriteLine("Available options: ");
            Console.WriteLine("Press 's' key to subscribe to the symbol");
            Console.WriteLine("Press 'q' key to close connection");
        }

        private void websocketClient_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Client successfully connected.");
            Console.WriteLine();
        }

        private void websocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine();
            var deserializeData = JsonConvert.DeserializeObject<StockExchange>(e.Message);
            
            var stock = new List<StockExchange>();
            stock.Add(deserializeData);
            foreach (var item in stock)
            {
                Console.WriteLine(item.Symbol);
                Console.WriteLine(item.Price);
                Console.WriteLine(item.Volume);
                Console.WriteLine(item.DateTime);
            }
        }

        private void websocketClient_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine(e.Exception.GetType() + ": " + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);

            if (e.Exception.InnerException != null)
            {
                Console.WriteLine(e.Exception.InnerException.GetType());
            }

            return;
        }
    }
}