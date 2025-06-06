﻿using Microsoft.Extensions.Configuration;

namespace TrainworksReloaded.Core.Extensions
{
    public static class ParseExtensions
    {
        public static string? ParseString(this IConfigurationSection section)
        {
            return section.Value;
        }

        public static int? ParseInt(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            if (int.TryParse(val, out var i))
            {
                return i;
            }
            return null;
        }

        public static float? ParseFloat(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            if (float.TryParse(val, out var i))
            {
                return i;
            }
            return null;
        }

        public static bool? ParseBool(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            if (bool.TryParse(val, out var i))
            {
                return i;
            }
            return null;
        }
    }
}
