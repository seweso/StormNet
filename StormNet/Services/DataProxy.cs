

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StormNet
{
    public class DataProxy
    {
        private readonly IHubContext<SignalRHub> _hubcontext;
        private string _data;

        private readonly List<double?> _doubles = new ();

        public DataProxy(IHubContext<SignalRHub> hubcontext)
        {
            _hubcontext = hubcontext;
            
            // Initialize with 32 nulls 
            for (var i = 0; i < 32; i++)
            {
                _doubles.Add(null);
            }
        }
        
        public async Task UpdateFromStormworks(string data)
        {
            _data = data;

            // Read bytes
            var bytes = Convert.FromBase64String(data
                .Replace('@', '+').Replace('~', '/'));

            for (var i = 0; i < 32; i++)
            {
                var newD = BitConverter.ToDouble(bytes, i * 8);
                var oldD = _doubles[i];

                if (newD != oldD)
                {
                    // Send to Controllers (TODO: Refactor to own class)
                    await _hubcontext.Clients.All.SendAsync("GetDouble",  i, newD);
                }

                _doubles[i] = newD;
            }
        }

        public string GetForStormworks()
        {
            return _data;
        }
        
    }

}