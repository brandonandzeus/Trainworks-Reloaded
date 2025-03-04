using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IRegister<CharacterTriggerData> triggerRegister;
        private readonly IRegister<VfxAtLoc> vfxRegister;
        private readonly FallbackDataProvider dataProvider;

        public CharacterDataFinalizer(
            IModLogger<CharacterDataFinalizer> logger,
            ICache<IDefinition<CharacterData>> cache,
            IRegister<AssetReferenceGameObject> assetReferenceRegister,
            IRegister<CharacterTriggerData> triggerRegister,
            IRegister<VfxAtLoc> vfxRegister,
            FallbackDataProvider dataProvider
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.assetReferenceRegister = assetReferenceRegister;
            this.triggerRegister = triggerRegister;
            this.vfxRegister = vfxRegister;
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

            //handle triggers
            var triggerDatas = new List<CharacterTriggerData>();
            var triggerDatasConfigs = configuration
                .GetSection("triggers")
                .GetChildren()
                .Select(xs => xs.GetSection("id").ParseString())
                .ToList();
            foreach (var triggerData in triggerDatasConfigs)
            {
                if (triggerData == null)
                {
                    continue;
                }
                logger.Log(Core.Interfaces.LogLevel.Info, $"Looking for {triggerData}");
                if (
                    triggerRegister.TryLookupId(
                        triggerData.ToId(key, "CTrigger"),
                        out var card,
                        out var _
                    )
                )
                {
                    logger.Log(Core.Interfaces.LogLevel.Info, $"Found {triggerData}");
                    triggerDatas.Add(card);
                }
            }
            AccessTools.Field(typeof(CharacterData), "triggers").SetValue(data, triggerDatas);

            var projectilePrefab = configuration.GetSection("projectile_vfx").ParseString() ?? "";
            if (
                vfxRegister.TryLookupId(
                    projectilePrefab.ToId(key, "Vfx"),
                    out var projectile_vfx,
                    out var _
                )
            )
            {
                AccessTools
                    .Field(typeof(CharacterData), "projectilePrefab")
                    .SetValue(data, projectile_vfx);
            }

            var attackVFX = configuration.GetSection("attack_vfx").ParseString() ?? "";
            if (vfxRegister.TryLookupId(attackVFX.ToId(key, "Vfx"), out var attack_vfx, out var _))
            {
                AccessTools.Field(typeof(CharacterData), "attackVFX").SetValue(data, attack_vfx);
            }

            var impactVFX = configuration.GetSection("impact_vfx").ParseString() ?? "";
            if (vfxRegister.TryLookupId(impactVFX.ToId(key, "Vfx"), out var impact_vfx, out var _))
            {
                AccessTools.Field(typeof(CharacterData), "impactVFX").SetValue(data, impact_vfx);
            }

            var deathVFX = configuration.GetSection("death_vfx").ParseString() ?? "";
            if (vfxRegister.TryLookupId(deathVFX.ToId(key, "Vfx"), out var death_vfx, out var _))
            {
                AccessTools.Field(typeof(CharacterData), "deathVFX").SetValue(data, death_vfx);
            }

            var bossSpellCastVFX = configuration.GetSection("boss_cast_vfx").ParseString() ?? "";
            if (
                vfxRegister.TryLookupId(
                    bossSpellCastVFX.ToId(key, "Vfx"),
                    out var boss_cast_vfx,
                    out var _
                )
            )
            {
                AccessTools
                    .Field(typeof(CharacterData), "bossSpellCastVFX")
                    .SetValue(data, boss_cast_vfx);
            }

            var bossRoomSpellCastVFX =
                configuration.GetSection("boss_room_cast_vfx").ParseString() ?? "";
            if (
                vfxRegister.TryLookupId(
                    bossRoomSpellCastVFX.ToId(key, "Vfx"),
                    out var boss_room_cast_vfx,
                    out var _
                )
            )
            {
                AccessTools
                    .Field(typeof(CharacterData), "bossRoomSpellCastVFX")
                    .SetValue(data, boss_room_cast_vfx);
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
