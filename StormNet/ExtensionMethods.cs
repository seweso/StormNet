using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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