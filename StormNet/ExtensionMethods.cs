using System.Collections;
using System.Collections.Generic;

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
        
    }
}