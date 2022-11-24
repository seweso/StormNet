using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ProtoBuf;
using StormNet.Model;

namespace StormNet
{
    public static class ExtensionMethods
    {
        public static string GetAndDelete(this IDictionary<string, string> dictionary, string key)
        {
            var result = dictionary[key];
            dictionary.Remove(key);
            return result;
        }

        public static string ToUrlX(this byte[] rawBytes)
        {
            return Convert.ToBase64String(rawBytes)
                .Replace('+', '@').Replace('/', '~');
        }

        public static byte[] FromUrlX(this string urlData)
        {
            return Convert.FromBase64String(urlData
                .Replace('@', '+').Replace('~', '/'));
        }

        // TODO: Move to SignalR class
        public static async Task SendDoubleToPony(this IClientProxy proxy, int index, double d)
        {
            await proxy.SendAsync("SetDouble", index, d);
        }
        
        public static async Task SendBoolToPony(this IClientProxy proxy, int index, bool b)
        {
            await proxy.SendAsync("SetBool", index, b);
        }
        
        
        public static T FromEncodedString<T>(this string urlEncodedString)
        {
            var bytes = urlEncodedString.FromUrlX();
            using var memoryStream = new MemoryStream(bytes);
            return Serializer.Deserialize<T>(memoryStream);
        }
        
        public static string ToEncodedString(this object obj)
        {
            using var memoryStream = new MemoryStream();
            Serializer.Serialize(memoryStream, obj);
            return memoryStream.ToArray().ToUrlX();
        }
        
    }
}