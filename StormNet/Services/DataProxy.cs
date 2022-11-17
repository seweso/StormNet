

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StormNet
{
    public class DataProxy
    {
        private readonly IHubContext<SignalRHub> _hubcontext;

        // Stormwork > Pony
        private readonly List<double?> _doublesFromStormwork = new ();

        // Pony > Stormworks
        private readonly double[] _doublesFromPony = new double[32];
        
        public DataProxy(IHubContext<SignalRHub> hubcontext)
        {
            _hubcontext = hubcontext;
            
            // Initialize with 32 nulls 
            for (var i = 0; i < 32; i++)
            {
                _doublesFromStormwork.Add(null);
            }
        }
        
        public async Task UpdateFromStormworks(string data)
        {
            // Read bytes
            var bytes = Convert.FromBase64String(data
                .Replace('@', '+').Replace('~', '/'));

            for (var i = 0; i < 32; i++)
            {
                var newD = BitConverter.ToDouble(bytes, i * 8);
                var oldD = _doublesFromStormwork[i];

                if (newD != oldD)
                {
                    // Send to Controllers (TODO: Refactor to own class)
                    await _hubcontext.Clients.All.SendAsync("GetDouble",  i, newD);
                }

                _doublesFromStormwork[i] = newD;
            }
        }

        public string GetForStormworks()
        {
            var bytes = new byte[_doublesFromPony.Length * sizeof(double)];
            Buffer.BlockCopy(_doublesFromPony, 0, bytes, 0, bytes.Length);
            return Convert.ToBase64String(bytes).
                Replace('+', '@').Replace('/', '~');
        }

        public void UpdateFromPony(int index, double value)
        {
            _doublesFromPony[index - 1] = value;
        }
    }

}