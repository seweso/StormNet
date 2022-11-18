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
                InputRange = new []{4,76},
                OutputRange = new []{54,665},
            };

            var toString = token.ToEncodedString();
            var fromString = toString.FromEncodedString<StormToken>();
            var toString2 = fromString.ToEncodedString();
            
            Assert.Equal(toString, toString2);
            Assert.Equal("CAQITBA2EJkF", toString);
            Assert.Equal(token.InputRange, fromString.InputRange);
            Assert.Equal(token.OutputRange, fromString.OutputRange);
        }
        
        [Fact]
        public void Test1()
        {
            var token = new StormToken
            {
                InputRange = new []{1,8},
                OutputRange = new []{1,8},
            };
            var toString = token.ToEncodedString();
            Assert.Equal("CAEICBABEAg=", toString);
        }
        
        [Fact]
        public void Test2()
        {
            var token = new StormToken
            {
                InputRange = new []{9,16},
                OutputRange = new []{9,16},
            };
            var toString = token.ToEncodedString();
            Assert.Equal("CAkIEBAJEBA=", toString);
        }
        
        [Fact]
        public void Test3()
        {
            var token = new StormToken
            {
                InputRange = new []{17,24},
                OutputRange = new []{17,24},
            };
            var toString = token.ToEncodedString();
            Assert.Equal("CBEIGBAREBg=", toString);
        }
        
        [Fact]
        public void Test4()
        {
            var token = new StormToken
            {
                InputRange = new []{25,32},
                OutputRange = new []{25,32},
            };
            var toString = token.ToEncodedString();
            Assert.Equal("CBkIIBAZECA=", toString);
        }
    }
}