using Microsoft.Extensions.Configuration;
using SimpleInjector;

namespace TrainworksReloaded.Core.Interfaces
{
    public interface ILazyBuilder
    {
        public void Configure(string pluginId, Action<IConfigurationBuilder> action);
        public void ConfigureLoaders(Action<Container> action);
    }
}
