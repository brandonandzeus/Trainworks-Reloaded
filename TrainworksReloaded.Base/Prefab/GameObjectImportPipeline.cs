using System.Collections.Generic;
using System.IO;
using SimpleInjector;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace TrainworksReloaded.Base.Prefab
{
    public class GameObjectImportPipeline : IDataPipeline<IRegister<GameObject>, GameObject>
    {
        private readonly PluginAtlas atlas;

        public GameObjectImportPipeline(PluginAtlas atlas)
        {
            this.atlas = atlas;
        }

        public List<IDefinition<GameObject>> Run(IRegister<GameObject> service)
        {
            var definitions = new List<IDefinition<GameObject>>();
            foreach (var config in atlas.PluginDefinitions)
            {
                var key = config.Key;
                foreach (
                    var gameObjectConfig in config
                        .Value.Configuration.GetSection("game_objects")
                        .GetChildren()
                )
                {
                    var id = gameObjectConfig.GetSection("id").Value;
                    if (id == null)
                    {
                        continue;
                    }
                    var name = key.GetId("GameObject", id);

                    var gameObject = new GameObject { name = name, layer = 5 };
                    GameObject.DontDestroyOnLoad(gameObject);

                    service.Register(name, gameObject);
                    var definition = new GameObjectDefinition(key, gameObject, gameObjectConfig)
                    {
                        Id = id,
                        IsModded = true,
                    };
                    definitions.Add(definition);
                }
            }
            return definitions;
        }
    }
}
