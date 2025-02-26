using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.CardUpgrade
{
    public class CardUpgradePipeline : IDataPipeline<IRegister<CardUpgradeData>>
    {
        private readonly PluginAtlas atlas;
        private readonly IModLogger<CardUpgradePipeline> logger;
        private readonly Container container;
        private readonly IEnumerable<IDataPipelineSetup<CardUpgradeData>> setups;
        private readonly IEnumerable<IDataPipelineFinalizer<CardUpgradeData>> finalizers;

        public CardUpgradePipeline(
            PluginAtlas atlas,
            IModLogger<CardUpgradePipeline> logger,
            Container container,
            IEnumerable<IDataPipelineSetup<CardUpgradeData>> setups,
            IEnumerable<IDataPipelineFinalizer<CardUpgradeData>> finalizers
        )
        {
            this.atlas = atlas;
            this.logger = logger;
            this.container = container;
            this.setups = setups;
            this.finalizers = finalizers;
        }

        public void Run(IRegister<CardUpgradeData> service)
        {
            // We load all cards and then finalize them to avoid dependency loops
            var processList = new List<CardUpgradeDefinition>();
            foreach (var config in atlas.PluginDefinitions)
            {
                processList.AddRange(LoadCards(service, config.Key, config.Value.Configuration));
            }

            foreach (var definition in processList)
            {
                FinalizeCardUpgradeData(service, definition);
                foreach (var finalizer in finalizers)
                {
                    finalizer.Finalize(definition);
                }
            }
        }

        /// <summary>
        /// Loads the Card Definitions in
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <param name="pluginConfig"></param>
        /// <returns></returns>
        private List<CardUpgradeDefinition> LoadCards(
            IRegister<CardUpgradeData> service,
            string key,
            IConfiguration pluginConfig
        )
        {
            var processList = new List<CardUpgradeDefinition>();
            foreach (var child in pluginConfig.GetSection("upgrades").GetChildren())
            {
                var data = LoadCardConfiguration(service, key, child);
                if (data != null)
                {
                    foreach (var setup in setups)
                    {
                        setup.Setup(data);
                    }
                    processList.Add(data);
                }
            }
            return processList;
        }

        private CardUpgradeDefinition? LoadCardConfiguration(
            IRegister<CardUpgradeData> service,
            string key,
            IConfiguration configuration
        )
        {
            var id = configuration.GetSection("id").ParseString();
            if (id == null)
            {
                return null;
            }

            var name = key.GetId("Upgrade", id);
            var titleKey = $"CardUpgrade_titleKey-{name}";
            var descriptionKey = $"CardUpgrade_descriptionKey-{name}";
            var notificationKey = $"CardUpgrade_notificationKey-{name}";
            var checkOverride = configuration.GetSection("override").ParseBool() ?? false;

            string guid;
            if (checkOverride && service.TryLookupName(id, out CardUpgradeData? data, out var _))
            {
                logger.Log(Core.Interfaces.LogLevel.Info, $"Overriding Upgrade {id}... ");
                titleKey = data.GetUpgradeTitleKey();
                descriptionKey = data.GetUpgradeDescriptionKey();
                notificationKey = data.GetUpgradeNotificationKey();
                guid = data.GetID();
            }
            else
            {
                data = ScriptableObject.CreateInstance<CardUpgradeData>();
                data.name = name;
                guid = Guid.NewGuid().ToString();
            }

            //handle id
            AccessTools.Field(typeof(CardUpgradeData), "id").SetValue(data, guid);
            var termRegister = container.GetInstance<IRegister<LocalizationTerm>>();

            //handle titles
            var localizationNameTerm = configuration.GetSection("titles").ParseLocalizationTerm();
            if (localizationNameTerm != null)
            {
                AccessTools
                    .Field(typeof(CardUpgradeData), "upgradeTitleKey")
                    .SetValue(data, titleKey);
                localizationNameTerm.Key = titleKey;
                termRegister.Register(titleKey, localizationNameTerm);
            }

            //handle description
            var localizationDescTerm = configuration
                .GetSection("descriptions")
                .ParseLocalizationTerm();
            if (localizationDescTerm != null)
            {
                AccessTools
                    .Field(typeof(CardUpgradeData), "upgradeDescriptionKey")
                    .SetValue(data, descriptionKey);
                localizationDescTerm.Key = descriptionKey;
                termRegister.Register(descriptionKey, localizationDescTerm);
            }

            //handle notifications
            var localizationNotificationTerm = configuration
                .GetSection("notifications")
                .ParseLocalizationTerm();
            if (localizationNotificationTerm != null)
            {
                AccessTools
                    .Field(typeof(CardUpgradeData), "upgradeNotificationKey")
                    .SetValue(data, notificationKey);
                localizationNotificationTerm.Key = notificationKey;
                termRegister.Register(notificationKey, localizationNotificationTerm);
            }

            //bools

            var allowSecondaryTooltipPlacement =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CardUpgradeData), "allowSecondaryTooltipPlacement")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CardUpgradeData), "allowSecondaryTooltipPlacement")
                .SetValue(
                    data,
                    configuration.GetSection("allow_secondary_tooltip_placement").ParseBool()
                        ?? allowSecondaryTooltipPlacement
                );

            var hideUpgradeIconOnCard =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CardUpgradeData), "hideUpgradeIconOnCard")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CardUpgradeData), "hideUpgradeIconOnCard")
                .SetValue(
                    data,
                    configuration.GetSection("hide_icon_on_card").ParseBool()
                        ?? hideUpgradeIconOnCard
                );

            var useUpgradeHighlightTextTags =
                checkOverride
                || (bool)
                    AccessTools
                        .Field(typeof(CardUpgradeData), "useUpgradeHighlightTextTags")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CardUpgradeData), "useUpgradeHighlightTextTags")
                .SetValue(
                    data,
                    configuration.GetSection("use_upgrade_hightlight_tags").ParseBool()
                        ?? useUpgradeHighlightTextTags
                );

            var showUpgradeDescriptionOnCards =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CardUpgradeData), "showUpgradeDescriptionOnCards")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CardUpgradeData), "showUpgradeDescriptionOnCards")
                .SetValue(
                    data,
                    configuration.GetSection("show_description_on_cards").ParseBool()
                        ?? showUpgradeDescriptionOnCards
                );

            var isUnique =
                checkOverride
                && (bool)AccessTools.Field(typeof(CardUpgradeData), "isUnique").GetValue(data);
            AccessTools
                .Field(typeof(CardUpgradeData), "isUnique")
                .SetValue(data, configuration.GetSection("is_unique").ParseBool() ?? isUnique);

            var restrictSizeToRoomCapacity =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CardUpgradeData), "restrictSizeToRoomCapacity")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CardUpgradeData), "restrictSizeToRoomCapacity")
                .SetValue(
                    data,
                    configuration.GetSection("restrict_to_room_capacity").ParseBool()
                        ?? restrictSizeToRoomCapacity
                );

            var doNotReplaceExistingUnitAbility =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CardUpgradeData), "doNotReplaceExistingUnitAbility")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CardUpgradeData), "doNotReplaceExistingUnitAbility")
                .SetValue(
                    data,
                    configuration.GetSection("does_not_replace_ability").ParseBool()
                        ?? doNotReplaceExistingUnitAbility
                );

            var bonusDamage = checkOverride
                ? (int)AccessTools.Field(typeof(CardUpgradeData), "bonusDamage").GetValue(data)
                : 0;
            AccessTools
                .Field(typeof(CardUpgradeData), "bonusDamage")
                .SetValue(data, configuration.GetSection("bonus_damage").ParseInt() ?? bonusDamage);

            var bonusHP = checkOverride
                ? (int)AccessTools.Field(typeof(CardUpgradeData), "bonusHP").GetValue(data)
                : 0;
            AccessTools
                .Field(typeof(CardUpgradeData), "bonusHP")
                .SetValue(data, configuration.GetSection("bonus_hp").ParseInt() ?? bonusHP);

            var costReduction = checkOverride
                ? (int)AccessTools.Field(typeof(CardUpgradeData), "costReduction").GetValue(data)
                : 0;
            AccessTools
                .Field(typeof(CardUpgradeData), "costReduction")
                .SetValue(
                    data,
                    configuration.GetSection("cost_reduction").ParseInt() ?? costReduction
                );

            var xCostReduction = checkOverride
                ? (int)AccessTools.Field(typeof(CardUpgradeData), "xCostReduction").GetValue(data)
                : 0;
            AccessTools
                .Field(typeof(CardUpgradeData), "xCostReduction")
                .SetValue(
                    data,
                    configuration.GetSection("x_cost_reduction").ParseInt() ?? xCostReduction
                );

            var bonusHeal = checkOverride
                ? (int)AccessTools.Field(typeof(CardUpgradeData), "bonusHeal").GetValue(data)
                : 0;
            AccessTools
                .Field(typeof(CardUpgradeData), "bonusHeal")
                .SetValue(data, configuration.GetSection("bonus_heal").ParseInt() ?? bonusHeal);

            var bonusSize = checkOverride
                ? (int)AccessTools.Field(typeof(CardUpgradeData), "bonusSize").GetValue(data)
                : 0;
            AccessTools
                .Field(typeof(CardUpgradeData), "bonusSize")
                .SetValue(data, configuration.GetSection("bonus_size").ParseInt() ?? bonusSize);

            var bonusEquipment = checkOverride
                ? (int)AccessTools.Field(typeof(CardUpgradeData), "bonusEquipment").GetValue(data)
                : 0;
            AccessTools
                .Field(typeof(CardUpgradeData), "bonusEquipment")
                .SetValue(
                    data,
                    configuration.GetSection("bonus_equipment").ParseInt() ?? bonusEquipment
                );

            //List<String>
            var removeTraitUpgrades =
                (List<string>)
                    AccessTools
                        .Field(typeof(CardUpgradeData), "removeTraitUpgrades")
                        .GetValue(data);
            var upgrades = configuration.GetSection("remove_trait_upgrades").GetChildren();
            if (upgrades.Count() > 0)
            {
                removeTraitUpgrades.Clear();
            }
            foreach (var upgrade in upgrades)
            {
                var val = upgrade.Value;
                if (val == null)
                {
                    continue;
                }
                removeTraitUpgrades.Add(val);
            }

            if (!checkOverride)
                service.Register(name, data);
            return new CardUpgradeDefinition(key, data, configuration, checkOverride);
        }

        private void FinalizeCardUpgradeData(
            IRegister<CardUpgradeData> service,
            CardUpgradeDefinition definition
        ) { }
    }
}
