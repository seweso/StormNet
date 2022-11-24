using System;
using System.Linq;
using System.Threading.Tasks;
using ProtoBuf;

namespace StormNet.Model
{
    [ProtoContract]
    public class StormToken
    {
        /// <summary>
        /// First index used to transmit values to Pony (Fixed value will be send first, then inputs)
        /// </summary>
        [ProtoMember(4)]
        public int ToPonyStartIndex { get; set; }

        /// <summary>
        /// First fixed value to send to Pony interface (mostly used for Player number)
        /// </summary>
        [ProtoMember(5)]
        public int ToPonyFirstValue { get; set; }
        
        /// <summary>
        /// Indexes of values from Stormworks to send to Pony interface
        /// </summary>
        [ProtoMember(2)]
        public int[] ToPonyIndexes { get; set; }
        
        /// <summary>
        /// Index to start sending outputs from Pony to Stormworks
        /// </summary>
        [ProtoMember(3)]
        public int ToStormWorksStartIndex { get; set; }

        /// <summary>
        /// Outputs from Pony to send to Stormworks
        /// </summary>
        [ProtoMember(1)]
        public int[] ToStormworksIndexes { get; set; }


        public StormToken()
        {
        }

        /// <summary>
        /// Create token for player (1 of 4 etc)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="nrOfPlayers"></param>
        /// <param name="startIndex"></param>
        public StormToken(int player, int nrOfPlayers, int startIndex = 7)
        {
            var count = 32 / nrOfPlayers;
            var range = Enumerable.Range(1 + (player - 1)  * count, count).ToList();
            
            ToPonyStartIndex = startIndex;
            ToPonyFirstValue = player;
            ToPonyIndexes = range.ToArray();
            ToStormWorksStartIndex = 1;
            ToStormworksIndexes = range.ToArray();
        }
        

        public async Task SendToPony<T>(int index, T newD, Func<int, T, Task> send)
        {
            var toPonyIndex = IndexToPony(index);
            if (toPonyIndex >= 0)
            {
                await send(toPonyIndex, newD);    
            }
        }

        public void SendToStormworks<T>(int index, T newD, Action<int, T> send)
        {
            var toStormworksIndex = IndexToStormworks(index);
            if (toStormworksIndex >= 0)
            {
                send(toStormworksIndex, newD);    
            }
        }

        public (int, double) GetFirstValue()
        {
            return (ToPonyStartIndex, ToPonyFirstValue);
        }

        /// <summary>
        /// Gets stormworks output index and translates it to index used to send ot to Pony
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int IndexToPony(int i)
        {
            var toPonyIndex = Array.FindIndex(ToPonyIndexes, x => x == i);
            
            if (toPonyIndex == -1)
                return -1;

            return toPonyIndex + 1 + ToPonyStartIndex;
        }

        /// <summary>
        /// Gets Pony output index and translates it to index used to send it to Stormworks
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int IndexToStormworks(int i)
        {
            if (i <= 0)
                return -1;
            if (i > ToStormworksIndexes.Length)
                return -1;

            return ToStormworksIndexes[i-1];
        }
    }
}