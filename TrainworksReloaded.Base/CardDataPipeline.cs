using HarmonyLib;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base
{
    public class CardDataPipeline : IDataPipeline<IRegister<CardData>>
    {
        private readonly PluginAtlas atlas;

        public CardDataPipeline(PluginAtlas atlas)
        {
            this.atlas = atlas;
        }

        public void Run(IRegister<CardData> service)
        {
            foreach (var config in atlas.PluginDefinitions)
            {
                var ids = LoadCards(service, config.Key, config.Value.Configuration);
            }
        }

        private List<string> LoadCards(IRegister<CardData> service, string key, IConfiguration pluginConfig)
        {
            var ids = new List<string>();
            foreach (var child in pluginConfig.GetSection("cards").GetChildren())
            {
                var id = child.GetSection("id").Value;
                if (id == null)
                {
                    continue;
                }
                var namekey = $"{key}_{id}";
                LoadCardConfiguration(service, namekey, child);
                ids.Add(namekey);
            }
            return ids;
        }

        private void LoadCardConfiguration(IRegister<CardData> service, string nameKey, IConfiguration configuration)
        {
            var data = ScriptableObject.CreateInstance<CardData>();
            AccessTools.Field(typeof(CardData), "nameKey").SetValue(data, nameKey);
            service.Register(nameKey, data);
        }

    }
}
