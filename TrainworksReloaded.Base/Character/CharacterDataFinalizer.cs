using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Prefab;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TrainworksReloaded.Base.Character
{
    public class CharacterDataFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CharacterDataFinalizer> logger;
        private readonly ICache<IDefinition<CharacterData>> cache;
        private readonly IRegister<AssetReferenceGameObject> assetReferenceRegister;
        private readonly FallbackDataProvider dataProvider;

        public CharacterDataFinalizer(
            IModLogger<CharacterDataFinalizer> logger,
            ICache<IDefinition<CharacterData>> cache,
            IRegister<AssetReferenceGameObject> assetReferenceRegister,
            FallbackDataProvider dataProvider
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.assetReferenceRegister = assetReferenceRegister;
            this.dataProvider = dataProvider;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeCharacterData(definition);
            }
            cache.Clear();
        }

        public void FinalizeCharacterData(IDefinition<CharacterData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(Core.Interfaces.LogLevel.Info, $"Finalizing Character {data.name}... ");

            //handle art
            var characterArtReference = configuration.GetSection("character_art").ParseString();
            if (characterArtReference != null)
            {
                var gameObjectName = characterArtReference.ToId(key, "GameObject");
                logger.Log(Core.Interfaces.LogLevel.Info, $"Looking for {gameObjectName}");
                if (
                    assetReferenceRegister.TryLookupId(
                        gameObjectName,
                        out var gameObject,
                        out var _
                    )
                )
                {
                    logger.Log(Core.Interfaces.LogLevel.Info, $"Found {gameObjectName}");
                    AccessTools
                        .Field(typeof(CharacterData), "characterPrefabVariantRef")
                        .SetValue(data, gameObject);
                }
            }

            //status effects
            var status_effects = new List<StatusEffectStackData>();
            AccessTools
                .Field(typeof(CharacterData), "startingStatusEffects")
                .SetValue(data, status_effects.ToArray());

            AccessTools
                .Field(typeof(CharacterData), "fallbackData")
                .SetValue(data, dataProvider.FallbackData);
        }
    }
}
