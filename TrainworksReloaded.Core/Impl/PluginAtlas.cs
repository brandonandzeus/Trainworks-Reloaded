using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainworksReloaded.Core.Impl
{
    public class PluginAtlas
    {
        public Dictionary<string, PluginDefinition> PluginDefinitions { get; set; } = new Dictionary<string, PluginDefinition>();
    }
}
