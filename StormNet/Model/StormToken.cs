using System;
using System.IO;
using ProtoBuf;

namespace StormNet.Model
{
    [ProtoContract]
    public partial class StormToken
    {
        [ProtoMember(1)]
        public int[] InputRange { get; set; }
        
        [ProtoMember(2)]
        public int[] OutputRange { get; set; }
    }
}