using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using StormNet.Model;

namespace StormNet
{
    public class DataOrchestrator
    {
        private readonly Dictionary<string, StormToken> _connections = new ();

        private readonly IHubContext<SignalRHub> _hubcontext;
        private readonly Lazy<DataProxy> _dataProxy;

        public DataOrchestrator(IHubContext<SignalRHub> hubcontext, Lazy<DataProxy> dataProxy)
        {
            _dataProxy = dataProxy;
            _hubcontext = hubcontext;
        }
        
        public async Task Send(int i, double newD)
        {
            foreach (var (connectionId, token) in _connections)
            {
                // Test if index can be send, and transform index... based on token
                await _hubcontext.Clients.Client(connectionId).SendAsync("GetDouble", i, newD);  
            }
        }
        
        public void AddClient(string contextConnectionId, string token)
        {
            _connections[contextConnectionId] = token.FromEncodedString<StormToken>();
        }

        public void RemoveClient(string contextConnectionId)
        {
            _connections.Remove(contextConnectionId);
        }

        public void UpdateFromPony(string contextConnectionId, int index, double value)
        {
            _dataProxy.Value.UpdateFromPony(index, value);
        }
    }
}