using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Prefab
{
    public class TextureImport(Texture2D texture2D, GameObject gameObject)
    {
        public Texture2D Texture2D { get; } = texture2D;
        public GameObject GameObject { get; } = gameObject;
    }

    public class TextureImportDefinition(
        string key,
        TextureImport data,
        IConfiguration configuration
    ) : IDefinition<TextureImport>
    {
        public string Key { get; set; } = key;
        public TextureImport Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
    }
}
