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
        private readonly ICache<IDefinition<CardTriggerEffectData>> cache;

        public CardTriggerEffectFinalizer(
            IModLogger<CardTriggerEffectFinalizer> logger,
            IRegister<CardEffectData> effectRegister,
            IRegister<CardUpgradeData> upgradeRegister,
            ICache<IDefinition<CardTriggerEffectData>> cache
        )
        {
            this.logger = logger;
            this.effectRegister = effectRegister;
            this.upgradeRegister = upgradeRegister;
            this.cache = cache;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeTriger(definition);
            }
            cache.Clear();
        }

        private void FinalizeTriger(IDefinition<CardTriggerEffectData> definition)
        {
            var configuration = definition.Configuration;
            var key = definition.Key;
            var data = definition.Data;

            logger.Log(
                Core.Interfaces.LogLevel.Info,
                $"Finalizing Card {key.GetId("Trigger", definition.Id)}... "
            );

            var triggers = new List<CardTriggerData>();
            foreach (var triggerEffect in configuration.GetSection("trigger_effects").GetChildren())
            {
                var triggerData = new CardTriggerData();

                triggerData.persistenceMode =
                    configuration.GetSection("persistence").ParsePersistenceMode()
                    ?? PersistenceMode.SingleRun;
                triggerData.paramInt = configuration.GetSection("param_int").ParseInt() ?? 0;
                var effect = configuration.GetSection("trigger_effect").Value;
                if (effect == null)
                {
                    continue;
                }
                triggerData.cardTriggerEffect = effect;

                var buffEffect = configuration.GetSection("buff_effect").Value;
                if (buffEffect == null)
                {
                    continue;
                }
                triggerData.buffEffectType = buffEffect;

                var paramUpgrade = configuration.GetSection("param_upgrade").Value;
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
                var paramEffect = configuration.GetSection("id").Value;
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
