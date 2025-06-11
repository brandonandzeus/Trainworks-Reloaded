﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Trigger;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Enums
{
    public class CharacterTriggerTypeFinalizer : IDataFinalizer
    {
        private readonly Lazy<SaveManager> SaveManager;
        private readonly GameDataClient client;
        private readonly IModLogger<CharacterTriggerTypeFinalizer> logger;
        private readonly IRegister<Sprite> spriteRegister;
        private readonly ICache<IDefinition<CharacterTriggerData.Trigger>> cache;

        public CharacterTriggerTypeFinalizer(
            IModLogger<CharacterTriggerTypeFinalizer> logger,
            GameDataClient client,
            IRegister<Sprite> spriteRegister,
            ICache<IDefinition<CharacterTriggerData.Trigger>> cache
        )
        {
            SaveManager = new Lazy<SaveManager>(() =>
            {
                if (client.TryGetProvider<SaveManager>(out var provider))
                {
                    return provider;
                }
                else
                {
                    return new SaveManager();
                }
            });
            this.client = client;
            this.logger = logger;
            this.spriteRegister = spriteRegister;
            this.cache = cache;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeTrigger(definition);
            }
            cache.Clear();
        }

        private void FinalizeTrigger(IDefinition<CharacterTriggerData.Trigger> definition)
        {
            var configuration = definition.Configuration;
            var key = definition.Key;
            var trigger = definition.Data;

            logger.Log(LogLevel.Debug, $"Finalizing Character Trigger {key.GetId(TemplateConstants.CharacterTriggerEnum, definition.Id)}...");

            var baseKey = "CharacterTrigger_" + definition.Id;

            var statusManager = StatusEffectManager.Instance;
            var statusEffectsDisplayData = (StatusEffectsDisplayData) AccessTools
                .Field(typeof(StatusEffectManager), "displayData")
                .GetValue(statusManager);

            CharacterTriggerData.TriggerToLocalizationExpression[trigger] = baseKey;

            var sprite = configuration.GetSection("sprite").ParseString();
            if (
                sprite != null
                && spriteRegister.TryLookupId(
                    sprite.ToId(key, "Sprite"),
                    out var lookup,
                    out var _
                )
            )
            {
                var triggerIcons = (StatusEffectsDisplayData.TriggerSpriteDict) AccessTools
                    .Field(typeof(StatusEffectsDisplayData), "triggerIcons")
                    .GetValue(statusEffectsDisplayData);
                triggerIcons[trigger] = lookup;
            }

            var isStateModifier = configuration.GetSection("is_state_modifier").ParseBool() ?? false;
            if (isStateModifier)
            {
                var triggers = (List<CharacterTriggerData.Trigger>) AccessTools
                    .Field(typeof(StatusEffectsDisplayData), "stateModifierTriggers")
                    .GetValue(statusEffectsDisplayData);
                triggers.Add(trigger);
            }

            if (configuration.GetSection("notifications").Value != null)
            {
                var notificationDict = (StatusEffectsDisplayData.TriggersNotificationDict) AccessTools
                    .Field(typeof(StatusEffectsDisplayData), "triggerNotificationList")
                    .GetValue(statusEffectsDisplayData);
                notificationDict[trigger] = new StatusEffectsDisplayData.LocalizedString
                { key = baseKey + "_NotificationText" };
            }

            var hidden = configuration.GetSection("hidden").ParseBool() ?? false;
            if (hidden)
            {
                var hiddenTriggers = (List<CharacterTriggerData.Trigger>) AccessTools
                    .Field(typeof(CharacterTriggerData), "TriggersHiddenInUI").GetValue(null);
                hiddenTriggers?.Add(trigger);
            }

            var noDelay = configuration.GetSection("no_delay").ParseBool() ?? false;
            if (noDelay)
            {
                CharacterTriggerData.TriggersWithoutDelay.Add(trigger);
            }

            var disableInDeployment = configuration.GetSection("disallow_in_deployment").ParseBool() ?? false;
            if (disableInDeployment)
            {
                var balanceData = SaveManager.Value.GetAllGameData().GetBalanceData();
                var cardTriggers = (List<CharacterTriggerData.Trigger>) AccessTools
                    .Field(typeof(BalanceData), "disallowedDeploymentPhaseCharacterTriggers").GetValue(balanceData);
                cardTriggers.Add(trigger);
            }

            // TODO CharacterTrigger => CardTrigger association.
        }
    }
}
