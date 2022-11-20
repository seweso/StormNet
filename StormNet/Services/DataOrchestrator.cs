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
        
        public async Task AddClient(string contextConnectionId, string tokenString)
        {
            var token = tokenString.FromEncodedString<StormToken>();
            _connections[contextConnectionId] = token;
            
            // Send First value
            var (index, value) = token.GetFirstValue();
            await _hubcontext.Clients.Client(contextConnectionId).SendDoubleToPony(index, value);
        }
        
        public async Task SendToPony(int index, double newD)
        {
            foreach (var (connectionId, token) in _connections)
            {
                // Test if index can be send, and transform index... based on token
                await token.StormWorksToPony(index + 1, newD, _hubcontext.Clients.Client(connectionId).SendDoubleToPony);
            }
        }
        
        public void SendToStormworks(string contextConnectionId, int index, double value)
        {
            _connections[contextConnectionId]
                .PonyToStormworks(index, value, _dataProxy.Value.UpdateFromPony);
        }
        
        public void RemoveClient(string contextConnectionId)
        {
            _connections.Remove(contextConnectionId);
        }


    }
}