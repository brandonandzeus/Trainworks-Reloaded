using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Effect
{
    public class CardEffectFinalizer : IDataFinalizer
    {
        private readonly IRegister<CharacterData> characterDataRegister;
        private readonly ICache<IDefinition<CardEffectData>> cache;

        public CardEffectFinalizer(
            IRegister<CharacterData> characterDataRegister,
            ICache<IDefinition<CardEffectData>> cache
        )
        {
            this.characterDataRegister = characterDataRegister;
            this.cache = cache;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeCardEffectData(definition);
            }
            cache.Clear();
        }

        private void FinalizeCardEffectData(IDefinition<CardEffectData> definition)
        {
            var configuration = definition.Configuration;
            var key = definition.Key;
            var data = definition.Data;

            var characterConfig = configuration.GetSection("param_character_data").Value;
            if (
                characterConfig != null
                && characterDataRegister.TryLookupName(
                    characterConfig.ToId(key, "Character"),
                    out var characterData,
                    out var _
                )
            )
            {
                AccessTools
                    .Field(typeof(CardEffectData), "paramCharacterData")
                    .SetValue(data, characterData);
            }

            var characterConfig2 = configuration.GetSection("param_character_data_2").Value;
            if (
                characterConfig2 != null
                && characterDataRegister.TryLookupName(
                    characterConfig2.ToId(key, "Character"),
                    out var characterData2,
                    out var _
                )
            )
            {
                AccessTools
                    .Field(typeof(CardEffectData), "paramAdditionalCharacterData")
                    .SetValue(data, characterData2);
            }

            //card pools
            var characterDataPool = new List<CharacterData>();
            var characterDataPoolConfig = configuration
                .GetSection("param_character_data_pool")
                .GetChildren()
                .Select(x => x.Value);
            foreach (var characterDataConfig in characterDataPoolConfig)
            {
                if (
                    characterDataConfig != null
                    && characterDataRegister.TryLookupName(
                        characterDataConfig.ToId(key, "Character"),
                        out var card,
                        out var _
                    )
                )
                {
                    characterDataPool.Add(card);
                }
            }
            AccessTools
                .Field(typeof(CardEffectData), "paramCharacterDataPool")
                .SetValue(data, characterDataPool);
        }
    }
}
