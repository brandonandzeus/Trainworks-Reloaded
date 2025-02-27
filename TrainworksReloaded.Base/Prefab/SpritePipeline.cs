using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace TrainworksReloaded.Base.Prefab
{
    public class SpritePipeline : IDataPipeline<IRegister<Sprite>>
    {
        private readonly PluginAtlas atlas;

        private readonly IEnumerable<IDataPipelineSetup<Sprite>> setups;

        public SpritePipeline(PluginAtlas atlas, IEnumerable<IDataPipelineSetup<Sprite>> setups)
        {
            this.atlas = atlas;
            this.setups = setups;
        }

        public void Run(IRegister<Sprite> service)
        {
            foreach (var config in atlas.PluginDefinitions)
            {
                var key = config.Key;
                foreach (
                    var spriteConfig in config
                        .Value.Configuration.GetSection("sprites")
                        .GetChildren()
                )
                {
                    var id = spriteConfig.GetSection("id").Value;
                    var path = spriteConfig.GetSection("path").Value;
                    if (path == null || id == null)
                    {
                        continue;
                    }
                    var name = key.GetId("Sprite", id);

                    foreach (var directory in config.Value.AssetDirectories)
                    {
                        var fullpath = Path.Combine(directory, path);
                        if (!File.Exists(fullpath))
                        {
                            continue;
                        }
                        var data = File.ReadAllBytes(fullpath);
                        var texture2d = new Texture2D(2, 2);
                        if (!texture2d.LoadImage(data))
                        {
                            continue;
                        }
                        var sprite = Sprite.Create(
                            texture2d,
                            new Rect(0, 0, texture2d.width, texture2d.height),
                            new Vector2(0.5f, 0.5f),
                            128f
                        );
                        sprite.name = name;
                        service.Register(name, sprite);
                        var definition = new SpriteDefinition(key, sprite, spriteConfig);
                        foreach (var setup in setups)
                        {
                            setup.Setup(definition);
                        }
                        break;
                    }
                }
            }
        }
    }
}
