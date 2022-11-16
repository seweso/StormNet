using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StormNet
{
    public class SignalRHub: Hub
    {
        public async Task SendMessage(string message)
        {
            Console.WriteLine(message);
            
            await Clients.All.SendAsync("ReceiveMessage",  "From stormnet.net");
        }
    }
}