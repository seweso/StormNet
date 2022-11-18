

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StormNet
{
    public class DataProxy 
    {
        // Stormwork > Pony
        private readonly List<double?> _doublesFromStormwork = new ();

        // Pony > Stormworks
        private readonly double[] _doublesFromPony = new double[32];
        private DataOrchestrator _orchestrator;

        public DataProxy(DataOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;

            // Initialize with 32 nulls 
            for (var i = 0; i < 32; i++)
            {
                _doublesFromStormwork.Add(null);
            }
        }
        
        public async Task UpdateFromStormworks(string data)
        {
            // Read bytes
            var bytes = data.FromUrlX();

            for (var i = 0; i < 32; i++)
            {
                var newD = BitConverter.ToDouble(bytes, i * 8);
                var oldD = _doublesFromStormwork[i];

                if (newD != oldD)
                {
                    await _orchestrator.Send(i, newD);
                }
                _doublesFromStormwork[i] = newD;
            }
        }

        public string GetForStormworks()
        {
            var bytes = new byte[_doublesFromPony.Length * sizeof(double)];
            Buffer.BlockCopy(_doublesFromPony, 0, bytes, 0, bytes.Length);
            return bytes.ToUrlX();
        }

        public void UpdateFromPony(int index, double value)
        {
            _doublesFromPony[index - 1] = value;
        }
    }

}