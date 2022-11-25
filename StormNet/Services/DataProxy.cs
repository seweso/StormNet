

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
        private readonly List<bool?> _boolsFromStormwork = new ();

        // Pony > Stormworks
        private readonly double[] _doublesFromPony = new double[32];
        private readonly byte[] _boolsFromPony = new byte[32];
        
        private readonly DataOrchestrator _orchestrator;

        public DataProxy(DataOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;

            // Initialize with 32 nulls 
            for (var i = 0; i < 32; i++)
            {
                _doublesFromStormwork.Add(null);
                _boolsFromStormwork.Add(null);
            }
        }
        
        public async Task UpdateFromStormworks(string data)
        {
            // Read bytes
            var bytes = data.FromUrlX();
            
            // Read doubles
            for (var i = 0; i < 32; i++)
            {
                var newD = BitConverter.ToDouble(bytes, i * 8);
                var oldD = _doublesFromStormwork[i];

                if (newD != oldD)
                {
                    await _orchestrator.SendDoubleToPony(i, newD);
                }
                _doublesFromStormwork[i] = newD;
            }

            // Backwards compability with old script (without bools)
            if (bytes.Length <= 32 * 8)
                return;

            // Read booleans
            for (var i = 0; i < 32; i++)
            {
                var newB = bytes[32 * 8 + i] == 1;
                var oldB = _boolsFromStormwork[i];

                if (newB != oldB)
                {
                    await _orchestrator.SendBoolToPony(i, newB);
                }
                _boolsFromStormwork[i] = newB;
            }
        }

        public string GetForStormworks()
        {
            var bytes = new byte[_doublesFromPony.Length * sizeof(double) + 
                                 _boolsFromPony.Length * sizeof(byte)];
            
            Buffer.BlockCopy(_doublesFromPony, 0, bytes, 0, _doublesFromPony.Length * sizeof(double));
            Buffer.BlockCopy(_boolsFromPony, 0, bytes, 32 * 8, _boolsFromPony.Length * sizeof(byte));
            return bytes.ToUrlX();
        }

        public void UpdateDoubleFromPony(int index, double value)
        {
            _doublesFromPony[index - 1] = value;
        }
        
        public void UpdateBoolFromPony(int index, bool value)
        {
            _boolsFromPony[index - 1] = (byte)(value ? 1 : 0);
        }
    }

}