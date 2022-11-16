using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StormNet
{
    public class SignalRHub : Hub
    {
        private readonly DataProxy _dataProxy;

        public SignalRHub(DataProxy dataProxy)
        {
            Console.WriteLine("connected?");
            _dataProxy = dataProxy;
        }
        public async Task SendMessage(string message)
        {
            Console.WriteLine(message);

            await Clients.All.SendAsync("ReceiveMessage",  "From stormnet.net");
        }
    }
}