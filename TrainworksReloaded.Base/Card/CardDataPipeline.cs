using BepInEx.Logging;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using ShinyShoe;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Card
{
    public class CardDataDefinition(CardData data, IConfiguration configuration, bool isOverride)
    {
        public CardData Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
        public bool IsOverride { get; set; } = isOverride;
    }

    public class CardDataPipeline : IDataPipeline<IRegister<CardData>>
    {
        private readonly PluginAtlas atlas;
        private readonly IModLogger<CardDataPipeline> logger;
        private readonly Container container;

        public CardDataPipeline(PluginAtlas atlas, IModLogger<CardDataPipeline> logger, Container container)
        {
            this.atlas = atlas;
            this.logger = logger;
            this.container = container;
        }

        public void Run(IRegister<CardData> service)
        {
            // We load all cards and then finalize them to avoid dependency loops
            var processList = new List<CardDataDefinition>();
            foreach (var config in atlas.PluginDefinitions)
            {
                processList.AddRange(LoadCards(service, config.Key, config.Value.Configuration));
            }

            foreach(var definition in processList)
            {
                FinalizeCardData(service, definition);
            }
        }

        /// <summary>
        /// Loads the Card Definitions in 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <param name="pluginConfig"></param>
        /// <returns></returns>
        private List<CardDataDefinition> LoadCards(IRegister<CardData> service, string key, IConfiguration pluginConfig)
        {
            var processList = new List<CardDataDefinition>();
            foreach (var child in pluginConfig.GetSection("cards").GetChildren())
            {
                var data = LoadCardConfiguration(service, key, child);
                if (data != null)
                {
                    processList.Add(data);
                }
            }
            return processList;
        }

        private CardDataDefinition? LoadCardConfiguration(IRegister<CardData> service, string key, IConfiguration configuration)
        {
            var id = configuration.GetSection("id").ParseString();
            if (id == null)
            {
                return null;
            }

            var name = $"{key}-{id}";
            var namekey = $"CardData_nameKey-{name}";
            var descriptionKey = $"CardData_descriptionKey-{name}";
            var checkOverride = configuration.GetSection("override").ParseBool() ?? false;

            string guid;
            if (checkOverride && service.TryLookupName(id, out CardData? data, out var _))
            {
                logger.Log(Core.Interfaces.LogLevel.Info, $"Overriding Card {id}... ");
                descriptionKey = data.GetOverrideDescriptionKey();
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

            //handle id
            AccessTools.Field(typeof(CardData), "id").SetValue(data, guid);

            var termRegister = container.GetInstance<IRegister<LocalizationTerm>>();

            //handle names
            AccessTools.Field(typeof(CardData), "nameKey").SetValue(data, namekey);
            var localizationNameTerm = configuration.GetSection("names").ParseLocalizationTerm();
            if (localizationNameTerm != null)
            {
                localizationNameTerm.Key = namekey;
                termRegister.Register(namekey, localizationNameTerm);
            }

            //handle description
            AccessTools.Field(typeof(CardData), "overrideDescriptionKey").SetValue(data, descriptionKey);
            var localizationDescTerm = configuration.GetSection("extra_description").ParseLocalizationTerm();
            if (localizationDescTerm != null)
            {
                localizationDescTerm.Key = descriptionKey;
                termRegister.Register(descriptionKey, localizationDescTerm);
            }

            //handle tooltips
            int tooltip_count = 0;
            var tooltips = checkOverride ? [] : (List<String>)AccessTools.Field(typeof(CardData), "cardLoreTooltipKeys").GetValue(data);
            foreach (var tooltip in configuration.GetSection("lore_tooltips").GetChildren())
            {
                var localizationTooltipTerm = tooltip.ParseLocalizationTerm();
                if (localizationTooltipTerm != null)
                {
                    string tooltipKey = $"CardData_tooltipKey{tooltip_count}-{name}";
                    if (checkOverride && tooltips.Contains(localizationTooltipTerm.Key))
                    {
                        tooltipKey = localizationTooltipTerm.Key;
                    }
                    else
                    {
                        localizationTooltipTerm.Key = tooltipKey;
                    }
                    termRegister.Register(tooltipKey, localizationTooltipTerm);
                    tooltip_count++;
                }
            }

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

            var defaultAbility = checkOverride && (bool)AccessTools.Field(typeof(CardData), "isUnitAbility").GetValue(data);
            AccessTools.Field(typeof(CardData), "isUnitAbility").SetValue(data, configuration.GetSection("ability").ParseBool() ?? defaultAbility);

            var defaultTargetsRoom = checkOverride && (bool)AccessTools.Field(typeof(CardData), "targetsRoom").GetValue(data);
            AccessTools.Field(typeof(CardData), "targetsRoom").SetValue(data, configuration.GetSection("targets_room").ParseBool() ?? defaultTargetsRoom);

            var defaultTargetless = checkOverride && (bool)AccessTools.Field(typeof(CardData), "targetless").GetValue(data);
            AccessTools.Field(typeof(CardData), "targetless").SetValue(data, configuration.GetSection("targetless").ParseBool() ?? defaultTargetless);

            var defaultRarity = checkOverride ? (CollectableRarity)AccessTools.Field(typeof(CardData), "rarity").GetValue(data) : CollectableRarity.Common;
            AccessTools.Field(typeof(CardData), "rarity").SetValue(data, configuration.GetSection("rarity").ParseRarity() ?? defaultRarity);

            var defaultDLC = checkOverride ? (DLC)AccessTools.Field(typeof(CardData), "requiredDLC").GetValue(data) : DLC.None;
            AccessTools.Field(typeof(CardData), "requiredDLC").SetValue(data, configuration.GetSection("dlc").ParseDLC() ?? defaultDLC);

            var defaultUnlockLevel = checkOverride ? (int)AccessTools.Field(typeof(CardData), "unlockLevel").GetValue(data) : 0;
            AccessTools.Field(typeof(CardData), "unlockLevel").SetValue(data, configuration.GetSection("unlock_level").ParseInt() ?? defaultUnlockLevel);

            var ignoreWhenCountingMastery = checkOverride && (bool)AccessTools.Field(typeof(CardData), "ignoreWhenCountingMastery").GetValue(data);
            AccessTools.Field(typeof(CardData), "ignoreWhenCountingMastery").SetValue(data, configuration.GetSection("count_for_mastery").ParseBool() ?? ignoreWhenCountingMastery);

            var hideInLogbook = checkOverride && (bool)AccessTools.Field(typeof(CardData), "hideInLogbook").GetValue(data);
            AccessTools.Field(typeof(CardData), "hideInLogbook").SetValue(data, configuration.GetSection("hide_in_logbook").ParseBool() ?? hideInLogbook);

            var initialKeyboardTarget = checkOverride ? (CardInitialKeyboardTarget)AccessTools.Field(typeof(CardData), "initialKeyboardTarget").GetValue(data) : CardInitialKeyboardTarget.FrontFriendly;
            AccessTools.Field(typeof(CardData), "initialKeyboardTarget").SetValue(data, configuration.GetSection("target_assist").ParseKeyboardTarget() ?? initialKeyboardTarget);

            var canAbilityTargetOtherFloors = checkOverride && (bool)AccessTools.Field(typeof(CardData), "canAbilityTargetOtherFloors").GetValue(data);
            AccessTools.Field(typeof(CardData), "canAbilityTargetOtherFloors").SetValue(data, configuration.GetSection("ability_effects_other_floors").ParseBool() ?? canAbilityTargetOtherFloors);

            var artistAttribution = checkOverride ? (string)AccessTools.Field(typeof(CardData), "artistAttribution").GetValue(data) : "";
            AccessTools.Field(typeof(CardData), "artistAttribution").SetValue(data, configuration.GetSection("artist").ParseString() ?? artistAttribution);


            //register before filling in data using 
            if (!checkOverride)
                service.Register(name, data);

            return new CardDataDefinition(data, configuration, checkOverride);
        }

        /// <summary>
        /// Finalize Card Definitions
        /// Handles Data to avoid lookup looks for names and ids
        /// </summary>
        /// <param name="definition"></param>
        private void FinalizeCardData(IRegister<CardData> service, CardDataDefinition definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;

            //handle linked class
            var classRegister = container.GetInstance<IRegister<ClassData>>();
            var classfield = configuration.GetSection("class").ParseString();
            if (classfield != null && classRegister.TryLookupName(classfield, out var lookup, out var _))
            {
                AccessTools.Field(typeof(CardData), "linkedClass").SetValue(data, lookup);
            }

            //handle discovery cards
            var SharedDiscoveryCards = new List<CardData>();
            var SharedDiscoveryCardConfig = configuration.GetSection("shared_discovery_cards").GetChildren().Select(x => x.ParseString());
            foreach (var ConfigCard in SharedDiscoveryCardConfig)
            {
                if (ConfigCard == null)
                {
                    continue;
                }

                if(service.TryLookupName(ConfigCard, out var card, out var _)){
                    SharedDiscoveryCards.Add(card);
                }
            }
            AccessTools.Field(typeof(CardData), "sharedDiscoveryCards").SetValue(data, SharedDiscoveryCards);

            //handle mastery cards
            var SharedMasteryCards = new List<CardData>();
            var SharedMasteryCardConfig = configuration.GetSection("shared_mastery_cards").GetChildren().Select(x => x.ParseString());
            foreach (var ConfigCard in SharedMasteryCardConfig)
            {
                if (ConfigCard == null)
                {
                    continue;
                }

                if (service.TryLookupName(ConfigCard, out var card, out var _))
                {
                    SharedMasteryCards.Add(card);
                }
            }
            AccessTools.Field(typeof(CardData), "sharedMasteryCards").SetValue(data, SharedMasteryCards);


            var MasteryCardInfo = configuration.GetSection("mastery_card").ParseString();
            if (MasteryCardInfo != null && service.TryLookupName(MasteryCardInfo, out var MasteryCard, out var _))
            {
                AccessTools.Field(typeof(CardData), "linkedMasteryCard").SetValue(data, MasteryCard);
            }


        }

    }
}
