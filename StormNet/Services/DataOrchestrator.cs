using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public async Task SendDoubleToPony(int index, double newD)
        {
            foreach (var (connectionId, token) in _connections)
            {
                await token.SendToPony(index + 1, newD, _hubcontext.Clients.Client(connectionId).SendDoubleToPony);
            }
        }
        
        public async Task SendBoolToPony(int index, bool newB)
        {
            foreach (var (connectionId, token) in _connections)
            {
                await token.SendToPony(index + 1, newB, _hubcontext.Clients.Client(connectionId).SendBoolToPony);
            }
        }
        
        public void SendDoubleToStormworks(string contextConnectionId, int index, double value)
        {
            _connections[contextConnectionId]
                .SendToStormworks(index, value, _dataProxy.Value.UpdateDoubleFromPony);
        }

        public void SendBoolToStormworks(string contextConnectionId, int index, bool value)
        {
            _connections[contextConnectionId]
                .SendToStormworks(index, value, _dataProxy.Value.UpdateBoolFromPony);
        }
       
        
        public void RemoveClient(string contextConnectionId)
        {
            var token = _connections[contextConnectionId];
            _connections.Remove(contextConnectionId);

            // All connections closed for token
            if (!_connections.Values.Any(x => x.Equals(token)))
            {
                token.ResetValues(_dataProxy.Value.UpdateDoubleFromPony, _dataProxy.Value.UpdateBoolFromPony);
            }
        }


    }
}