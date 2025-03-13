using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace TrainworksReloaded.Core.Impl
{
    public class PluginDefinition
    {
        public IConfiguration Configuration { get; set; }
        public List<string> AssetDirectories { get; } = [];
        public Assembly? Assembly { get; set; }

        public PluginDefinition(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
