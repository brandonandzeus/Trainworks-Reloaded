using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using static RimLight;

namespace TrainworksReloaded.Base.Trigger
{
    public class CharacterTriggerFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CharacterTriggerFinalizer> logger;
        private readonly IRegister<CardEffectData> effectRegister;
        private readonly IRegister<CharacterTriggerData.Trigger> triggerEnumRegister;
        private readonly ICache<IDefinition<CharacterTriggerData>> cache;

        public CharacterTriggerFinalizer(
            IModLogger<CharacterTriggerFinalizer> logger,
            IRegister<CardEffectData> effectRegister,
            IRegister<CharacterTriggerData.Trigger> triggerEnumRegister,
            ICache<IDefinition<CharacterTriggerData>> cache
        )
        {
            this.logger = logger;
            this.effectRegister = effectRegister;
            this.triggerEnumRegister = triggerEnumRegister;
            this.cache = cache;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeCharacterTrigger(definition);
            }
            cache.Clear();
        }

        private void FinalizeCharacterTrigger(IDefinition<CharacterTriggerData> definition)
        {
            var configuration = definition.Configuration;
            var key = definition.Key;
            var data = definition.Data;

            logger.Log(
                Core.Interfaces.LogLevel.Info,
                $"Finalizing Character Trigger {key.GetId(TemplateConstants.CharacterTrigger, definition.Id)}... "
            );

            //handle trigger
            var trigger = CharacterTriggerData.Trigger.OnDeath;
            var triggerSection = configuration.GetSection("trigger");
            if (triggerSection.Value != null)
            {
                var value = triggerSection.Value;
                if (
                    triggerEnumRegister.TryLookupId(
                        value.ToId(key, TemplateConstants.CharacterTriggerEnum),
                        out var triggerFound,
                        out var _
                    )
                )
                {
                    trigger = triggerFound;
                }
                else
                {
                    trigger = triggerSection.ParseTrigger() ?? default;
                }
            }
            AccessTools
                .Field(typeof(CharacterTriggerData), "trigger")
                .SetValue(data, trigger);

            //handle effects cards
            var effectDatas = new List<CardEffectData>();
            var effectDatasConfig = configuration
                .GetSection("effects")
                .GetChildren()
                .Select(x => x.GetSection("id").ParseString());
            foreach (var effectData in effectDatasConfig)
            {
                if (effectData == null)
                {
                    continue;
                }

                if (
                    effectRegister.TryLookupId(
                        effectData.ToId(key, TemplateConstants.Effect),
                        out var effect,
                        out var _
                    )
                )
                {
                    logger.Log(
                        Core.Interfaces.LogLevel.Info,
                        $"Adding Effect {effect.GetEffectStateName()}"
                    );
                    effectDatas.Add(effect);
                }
            }
            AccessTools.Field(typeof(CharacterTriggerData), "effects").SetValue(data, effectDatas);
        }
    }
}
