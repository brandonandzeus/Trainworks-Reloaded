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
        private readonly ICache<IDefinition<CharacterTriggerData>> cache;

        public CharacterTriggerFinalizer(
            IModLogger<CharacterTriggerFinalizer> logger,
            IRegister<CardEffectData> effectRegister,
            ICache<IDefinition<CharacterTriggerData>> cache
        )
        {
            this.logger = logger;
            this.effectRegister = effectRegister;
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
                $"Finalizing Card {key.GetId("CTrigger", definition.Id)}... "
            );

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
                        effectData.ToId(key, "Effect"),
                        out var card,
                        out var _
                    )
                )
                {
                    effectDatas.Add(card);
                }
            }
            AccessTools.Field(typeof(CharacterTriggerData), "effects").SetValue(data, effectDatas);
        }
    }
}
