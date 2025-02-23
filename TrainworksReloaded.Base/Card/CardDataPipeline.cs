using BepInEx.Logging;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Card
{
    public class CardDataPipeline : IDataPipeline<ICustomRegister<CardData>>
    {
        private readonly PluginAtlas atlas;
        private readonly IRegister<CardData> register;
        private readonly IModLogger<CardDataPipeline> logger;

        public CardDataPipeline(PluginAtlas atlas, IRegister<CardData> register, IModLogger<CardDataPipeline> logger)
        {
            this.atlas = atlas;
            this.register = register;
            this.logger = logger;
        }

        public void Run(ICustomRegister<CardData> service)
        {
            foreach (var config in atlas.PluginDefinitions)
            {
                LoadCards(service, config.Key, config.Value.Configuration);
            }
        }

        private void LoadCards(ICustomRegister<CardData> service, string key, IConfiguration pluginConfig)
        {
            foreach (var child in pluginConfig.GetSection("cards").GetChildren())
            {
                LoadCardConfiguration(service, key, child);
            }
        }

        private void LoadCardConfiguration(ICustomRegister<CardData> service, string key, IConfiguration configuration)
        {
            var id = configuration.GetSection("id").ParseString();
            if (id == null)
            {
                return;
            }

            var name = $"{key}-{id}";
            var namekey = $"CardData_nameKey-{name}";
            var checkOverride = configuration.GetSection("override").ParseBool() ?? false;

            string guid;
            if (checkOverride && register.TryLookupName(id, out CardData? data, out bool? _))
            {
                logger.Log(Core.Interfaces.LogLevel.Info, $"Overriding Card {id}... ");
                namekey = data.GetNameKey();
                guid = data.GetID();
            }
            else
            {
                logger.Log(Core.Interfaces.LogLevel.Info, $"Registering Card {id}... ");
                data = ScriptableObject.CreateInstance<CardData>();
                data.name = name;
                guid = Guid.NewGuid().ToString();
            }

            //handle name key
            AccessTools.Field(typeof(CardData), "nameKey").SetValue(data, namekey);
            AccessTools.Field(typeof(CardData), "id").SetValue(data, guid);

            //handle one-to-one values
            var defaultCost = checkOverride ? (int)AccessTools.Field(typeof(CardData), "cost").GetValue(data) : 0;
            AccessTools.Field(typeof(CardData), "cost").SetValue(data, configuration.GetSection("cost").ParseInt() ?? defaultCost);

            var defaultCostType = checkOverride ? (CardData.CostType)AccessTools.Field(typeof(CardData), "costType").GetValue(data) : CardData.CostType.Default;
            AccessTools.Field(typeof(CardData), "costType").SetValue(data, configuration.GetSection("cost_type").ParseCostType() ?? defaultCostType);

            var defaultCardType = checkOverride ? (CardType)AccessTools.Field(typeof(CardData), "cardType").GetValue(data) : CardType.Spell;
            AccessTools.Field(typeof(CardData), "cardType").SetValue(data, configuration.GetSection("type").ParseCardType() ?? defaultCardType);

            var defaultInitCooldown = checkOverride ? (int)AccessTools.Field(typeof(CardData), "cooldownAtSpawn").GetValue(data) : 0;
            AccessTools.Field(typeof(CardData), "cooldownAtSpawn").SetValue(data, configuration.GetSection("initial_cooldown").ParseInt() ?? defaultInitCooldown);

            var defaultCooldown = checkOverride ? (int)AccessTools.Field(typeof(CardData), "cooldownAfterActivated").GetValue(data) : 0;
            AccessTools.Field(typeof(CardData), "cooldownAfterActivated").SetValue(data, configuration.GetSection("cooldown").ParseInt() ?? defaultCooldown);

            var defaultAbility = checkOverride ? (bool)AccessTools.Field(typeof(CardData), "isUnitAbility").GetValue(data) : false;
            AccessTools.Field(typeof(CardData), "isUnitAbility").SetValue(data, configuration.GetSection("ability").ParseBool() ?? defaultAbility);

            var defaultTargetsRoom = checkOverride ? (bool)AccessTools.Field(typeof(CardData), "targetsRoom").GetValue(data) : false;
            AccessTools.Field(typeof(CardData), "targetsRoom").SetValue(data, configuration.GetSection("targets_room").ParseBool() ?? defaultTargetsRoom);

            var defaultTargetless = checkOverride ? (bool)AccessTools.Field(typeof(CardData), "targetless").GetValue(data) : false;
            AccessTools.Field(typeof(CardData), "targetless").SetValue(data, configuration.GetSection("targetless").ParseBool() ?? defaultTargetless);

            //handle names

            //handle description
            //AccessTools.Field(typeof(CardData), "overrideDescriptionKey").SetValue(data, configuration.GetSection("override_description_key").ParseString());
            
            //register before filling in data using 
            if (!checkOverride)
                service.Register(name, data);
        }

    }
}
