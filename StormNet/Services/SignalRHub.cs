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
            _dataProxy = dataProxy;
        }
        public async Task UpdateFromPony(int index, double value)
        {
            _dataProxy.UpdateFromPony(index, value);
        }
    }
}