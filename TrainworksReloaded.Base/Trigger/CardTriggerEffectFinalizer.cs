using System;
using System.Collections.Generic;
using HarmonyLib;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Trigger
{
    public class CardTriggerEffectFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CardTriggerEffectFinalizer> logger;
        private readonly IRegister<CardEffectData> effectRegister;
        private readonly IRegister<CardUpgradeData> upgradeRegister;
        private readonly IRegister<CardTriggerType> triggerEnumRegister;
        private readonly ICache<IDefinition<CardTriggerEffectData>> cache;

        public CardTriggerEffectFinalizer(
            IModLogger<CardTriggerEffectFinalizer> logger,
            IRegister<CardEffectData> effectRegister,
            IRegister<CardUpgradeData> upgradeRegister,
            IRegister<CardTriggerType> triggerEnumRegister,
            ICache<IDefinition<CardTriggerEffectData>> cache
        )
        {
            this.logger = logger;
            this.effectRegister = effectRegister;
            this.upgradeRegister = upgradeRegister;
            this.triggerEnumRegister = triggerEnumRegister;
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

        private void FinalizeTrigger(IDefinition<CardTriggerEffectData> definition)
        {
            var configuration = definition.Configuration;
            var key = definition.Key;
            var data = definition.Data;

            logger.Log(
                Core.Interfaces.LogLevel.Info,
                $"Finalizing Card Trigger {key.GetId(TemplateConstants.CardTrigger, definition.Id)}... "
            );

            //handle trigger
            var trigger = CardTriggerType.OnCast;
            var triggerSection = configuration.GetSection("trigger");
            if (triggerSection.Value != null)
            {
                var value = triggerSection.Value;
                if (
                    triggerEnumRegister.TryLookupId(
                        value.ToId(key, TemplateConstants.CardTriggerEnum),
                        out var triggerFound,
                        out var _
                    )
                )
                {
                    trigger = triggerFound;
                }
                else
                {
                    trigger = triggerSection.ParseCardTriggerType() ?? default;
                }
            }
            AccessTools
                .Field(typeof(CardTriggerEffectData), "trigger")
                .SetValue(data, trigger);


            var triggers = new List<CardTriggerData>();
            foreach (var triggerEffect in configuration.GetSection("trigger_effects").GetChildren())
            {
                var triggerData = new CardTriggerData();

                triggerData.persistenceMode =
                    triggerEffect.GetSection("persistence").ParsePersistenceMode()
                    ?? PersistenceMode.SingleRun;
                triggerData.paramInt = triggerEffect.GetSection("param_int").ParseInt() ?? 0;
                var effect = triggerEffect.GetSection("trigger_effect").Value;
                if (effect == null)
                {
                    continue;
                }
                triggerData.cardTriggerEffect = effect;

                var buffEffectType = "";
                triggerData.buffEffectType = triggerEffect.GetSection("buff_effect").Value ?? buffEffectType;

                var paramUpgrade = triggerEffect.GetSection("param_upgrade").Value;
                if (
                    paramUpgrade != null
                    && upgradeRegister.TryLookupId(
                        paramUpgrade.ToId(key, "Upgrade"),
                        out var lookup,
                        out var _
                    )
                )
                {
                    triggerData.paramUpgrade = lookup;
                }
                triggers.Add(triggerData);
            }
            AccessTools
                .Field(typeof(CardTriggerEffectData), "cardTriggerEffects")
                .SetValue(data, triggers);

            var effects = new List<CardEffectData>();
            foreach (var effect in configuration.GetSection("effects").GetChildren())
            {
                if (effect == null)
                {
                    continue;
                }
                var paramEffect = effect.GetSection("id").Value;
                if (
                    paramEffect != null
                    && effectRegister.TryLookupId(
                        paramEffect.ToId(key, "Effect"),
                        out var lookup,
                        out var _
                    )
                )
                {
                    effects.Add(lookup);
                }
            }
            AccessTools.Field(typeof(CardTriggerEffectData), "cardEffects").SetValue(data, effects);
        }
    }
}
