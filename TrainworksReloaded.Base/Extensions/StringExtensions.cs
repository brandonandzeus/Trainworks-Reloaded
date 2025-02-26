using System;
using System.Collections.Generic;
using System.Text;

namespace TrainworksReloaded.Base.Extensions
{
    public static class StringExtensions
    {
        public static string GetId(this string key, string template, string id)
        {
            return $"{key}-{template}-{id}";
        }

        public static string ToId(this string baseString, string key, string template)
        {
            if (baseString.StartsWith("@"))
            {
                return $"{key}-{template}-{baseString.Substring(1)}";
            }
            else
            {
                return baseString;
            }
        }
    }
}
