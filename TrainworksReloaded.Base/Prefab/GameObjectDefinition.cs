using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Prefab
{
    public class GameObjectDefinition(string key, GameObject data, IConfiguration configuration)
        : IDefinition<GameObject>
    {
        public string Key { get; set; } = key;
        public GameObject Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
    }
}
