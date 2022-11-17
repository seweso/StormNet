using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StormNet
{
    public class SignalRHub : Hub
    {
        private readonly DataOrchestrator _orchestrator;

        public SignalRHub(DataOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected {0}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task SetToken(string token)
        {
            Console.WriteLine("Token {0}", token);
            _orchestrator.AddClient(Context.ConnectionId, token);
        }
        
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("Disconnected {0}", Context.ConnectionId);
            _orchestrator.RemoveClient(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateFromPony(int index, double value)
        {
            _orchestrator.UpdateFromPony(Context.ConnectionId, index, value);
        }
    }
  
}