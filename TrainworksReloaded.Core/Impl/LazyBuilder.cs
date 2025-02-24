using Microsoft.Extensions.Configuration;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TrainworksReloaded.Core.Interfaces;


namespace TrainworksReloaded.Core.Impl
{
    public class LazyBuilder : ILazyBuilder
    {
        public Dictionary<String, List<Action<IConfigurationBuilder>>> configActions = [];
        public List<Action<Container>> containerActions = [];
        public void Configure(string pluginId, Action<IConfigurationBuilder> action)
        {
            if (configActions.TryGetValue(pluginId, out var actions)){
                actions.Add(action);
            }
            else
            {
                configActions.Add(pluginId, [action] );
            }
        }

        public void ConfigureLoaders(Action<Container> action)
        {
            containerActions.Add(action);
        }


        public void Build(Container container)
        {
            //build atlas from configuration
            var atlas = new PluginAtlas();
            foreach (var key in configActions.Keys)
            {
                var configuration = new ConfigurationBuilder();
                var directory = new HashSet<string>();
                foreach (var action in configActions[key])
                {
                    var basePath = Path.GetDirectoryName(action.Method.DeclaringType.Assembly.Location);
                    directory.Add(basePath);
                    configuration.SetBasePath(basePath);
                    action(configuration);
                }
                var definition = new PluginDefinition(configuration.Build());
                definition.AssetDirectories.AddRange(directory);
                atlas.PluginDefinitions.Add(key, definition);
            }
            container.RegisterInstance<PluginAtlas>(atlas);

            foreach (var action in containerActions)
            {
                action(container);
            }
        }
    }
}
