using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var myClient = new RealTimeData();
            myClient.GetSymbols();
            myClient.SocketHandler();
            myClient.StartFeedData();
        }
    }
}
