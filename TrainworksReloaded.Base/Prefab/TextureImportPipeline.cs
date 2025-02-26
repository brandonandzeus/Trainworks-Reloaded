using System.Collections.Generic;
using System.IO;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace TrainworksReloaded.Base.Prefab
{
    public class TextureImportPipeline : IDataPipeline<IRegister<GameObject>>
    {
        private readonly PluginAtlas atlas;

        private readonly IEnumerable<IDataPipelineSetup<TextureImport>> setups;

        public TextureImportPipeline(
            PluginAtlas atlas,
            IEnumerable<IDataPipelineSetup<TextureImport>> setups
        )
        {
            this.atlas = atlas;
            this.setups = setups;
        }

        public void Run(IRegister<GameObject> service)
        {
            foreach (var config in atlas.PluginDefinitions)
            {
                foreach (
                    var texture in config.Value.Configuration.GetSection("textures").GetChildren()
                )
                {
                    var id = texture.GetSection("id").Value;
                    var path = texture.GetSection("path").Value;
                    if (path == null || id == null)
                    {
                        continue;
                    }
                    var name = $"{config.Key}-{id}";

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

                        var gameObject = new GameObject { name = name, layer = 5 };
                        GameObject.DontDestroyOnLoad(gameObject);

                        service.Register(name, gameObject);
                        foreach (var setup in setups)
                        {
                            setup.Setup(
                                new TextureImportDefinition(
                                    name,
                                    new TextureImport(texture2d, gameObject),
                                    texture
                                )
                            );
                        }
                        break;
                    }
                }
            }
        }
    }
}
