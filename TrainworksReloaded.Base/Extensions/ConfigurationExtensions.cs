using BepInEx.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Extensions
{
    public static class ConfiguirationExtensions
    {
        internal static ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(ConfiguirationExtensions));

        public static IConfigurationSection GetDeprecatedSection(this IConfiguration configuration, string name, string newName)
        {
            var section = configuration.GetSection(name);
            if (section.Exists())
            {
                Logger.LogWarning($"[Deprecation] Field name \"{name}\" is deprecated, use \"{newName}\" instead");
                return section;
            }
            else
            {
                return configuration.GetSection(newName);
            }
        }
    }
}
