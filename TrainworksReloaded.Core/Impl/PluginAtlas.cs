using System.Reflection;

namespace TrainworksReloaded.Core.Impl
{
    public class PluginAtlas
    {
        public Dictionary<string, PluginDefinition> PluginDefinitions { get; set; } =
            new Dictionary<string, PluginDefinition>();
        public Dictionary<string, Assembly> PluginAssemblies { get; set; } = [];
    }
}
