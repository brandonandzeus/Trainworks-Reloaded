using Microsoft.Extensions.Configuration;

namespace TrainworksReloaded.Core.Impl
{
    public class PluginDefinition
    {
        public IConfiguration Configuration { get; set; }
        public List<string> AssetDirectories { get; } = new List<string>();

        public PluginDefinition(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
