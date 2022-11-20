using System;
using StormNet;
using StormNet.Model;
using Xunit;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test()
        {
            var token = new StormToken
            {
                ToStormworksIndexes = new []{4,76},
                ToPonyIndexes = new []{54,665},
            };

            var toString = token.ToEncodedString();
            var fromString = toString.FromEncodedString<StormToken>();
            var toString2 = fromString.ToEncodedString();
            
            Assert.Equal(toString, toString2);
            Assert.Equal("CAQITBA2EJkF", toString);
            Assert.Equal(token.ToStormworksIndexes, fromString.ToStormworksIndexes);
            Assert.Equal(token.ToPonyIndexes, fromString.ToPonyIndexes);
        }
        
        [Fact]
        public void Test1()
        {
            var token = new StormToken
            {
                ToPonyFirstValue = 1,
                ToPonyStartIndex = 7, 
                ToPonyIndexes = new []{1,2,3,4,5,6,7,8},
                ToStormWorksStartIndex = 1,
                ToStormworksIndexes = new []{1,2,3,4,5,6,7,8},
            };
            var toString = token.ToEncodedString();
            Assert.Equal("CAEIAggDCAQIBQgGCAcICBABEAIQAxAEEAUQBhAHEAgYASAHKAE=", toString);

            var toString2 = new StormToken(1, 4).ToEncodedString();
            Assert.Equal(toString, toString2);

            Assert.Equal(-1, token.IndexToPony(0));
            Assert.Equal(8, token.IndexToPony(1));
            Assert.Equal(15, token.IndexToPony(8));
            Assert.Equal(-1, token.IndexToPony(9));
            
            Assert.Equal(-1, token.IndexToStormworks(0));
            Assert.Equal(1, token.IndexToStormworks(1));
            Assert.Equal(8, token.IndexToStormworks(8));
            Assert.Equal(-1, token.IndexToStormworks(9));
        }
        
        [Fact]
        public void Test2()
        {
            var token = new StormToken
            {
                ToPonyFirstValue = 2,
                ToPonyStartIndex = 7, 
                ToPonyIndexes = new []{9,10,11,12,13,14,15,16},
                ToStormWorksStartIndex = 1,
                ToStormworksIndexes = new []{9,10,11,12,13,14,15,16},
            };
            var toString = token.ToEncodedString();
            Assert.Equal("CAkICggLCAwIDQgOCA8IEBAJEAoQCxAMEA0QDhAPEBAYASAHKAI=", toString);
            
            var toString2 = new StormToken(2, 4).ToEncodedString();
            Assert.Equal(toString, toString2);
            
            Assert.Equal(-1, token.IndexToPony(7));
            Assert.Equal(8, token.IndexToPony(9));
            Assert.Equal(15, token.IndexToPony(16));
            Assert.Equal(-1, token.IndexToPony(17));
            
            Assert.Equal(-1, token.IndexToStormworks(0));
            Assert.Equal(9, token.IndexToStormworks(1));
            Assert.Equal(16, token.IndexToStormworks(8));
            Assert.Equal(-1, token.IndexToStormworks(9));
        }
        
        [Fact]
        public void Test3()
        {
            var token = new StormToken
            {
                ToPonyFirstValue = 3,
                ToPonyStartIndex = 7, 
                ToPonyIndexes = new []{17,18,19,20,21,22,23,24},
                ToStormWorksStartIndex = 1,
                ToStormworksIndexes = new []{17,18,19,20,21,22,23,24},    
            };
            var toString = token.ToEncodedString();
            Assert.Equal("CBEIEggTCBQIFQgWCBcIGBAREBIQExAUEBUQFhAXEBgYASAHKAM=", toString);
            
            var toString2 = new StormToken(3, 4).ToEncodedString();
            Assert.Equal(toString, toString2);
            
            Assert.Equal(-1, token.IndexToPony(16));
            Assert.Equal(8, token.IndexToPony(17));
            Assert.Equal(15, token.IndexToPony(24));
            Assert.Equal(-1, token.IndexToPony(25));
            
            Assert.Equal(-1, token.IndexToStormworks(0));
            Assert.Equal(17, token.IndexToStormworks(1));
            Assert.Equal(24, token.IndexToStormworks(8));
            Assert.Equal(-1, token.IndexToStormworks(9));
        }
        
        [Fact]
        public void Test4()
        {
            var token = new StormToken
            {
                ToPonyFirstValue = 4,
                ToPonyStartIndex = 7, 
                ToPonyIndexes = new []{25,26,27,28,29,30,31,32},
                ToStormWorksStartIndex = 1,
                ToStormworksIndexes = new []{25,26,27,28,29,30,31,32},
            };
            var toString = token.ToEncodedString();
            Assert.Equal("CBkIGggbCBwIHQgeCB8IIBAZEBoQGxAcEB0QHhAfECAYASAHKAQ=", toString); 
            
            var toString2 = new StormToken(4, 4).ToEncodedString();
            Assert.Equal(toString, toString2);
            
            Assert.Equal(-1, token.IndexToPony(24));
            Assert.Equal(8, token.IndexToPony(25));
            Assert.Equal(15, token.IndexToPony(32));
            Assert.Equal(-1, token.IndexToPony(33));
            
            Assert.Equal(-1, token.IndexToStormworks(0));
            Assert.Equal(25, token.IndexToStormworks(1));
            Assert.Equal(32, token.IndexToStormworks(8));
            Assert.Equal(-1, token.IndexToStormworks(9));
        }
    }
}