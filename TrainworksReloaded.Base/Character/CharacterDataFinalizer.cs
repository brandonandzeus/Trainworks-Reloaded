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
using static TrainworksReloaded.Base.Extensions.ParseReferenceExtensions;

namespace TrainworksReloaded.Base.Character
{
    public class CharacterDataFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CharacterDataFinalizer> logger;
        private readonly ICache<IDefinition<CharacterData>> cache;
        private readonly IRegister<AssetReferenceGameObject> assetReferenceRegister;
        private readonly IRegister<CharacterTriggerData> triggerRegister;
        private readonly IRegister<VfxAtLoc> vfxRegister;
        private readonly IRegister<StatusEffectData> statusRegister;
        private readonly IRegister<CardData> cardRegister;
        private readonly IRegister<CharacterChatterData> chatterRegister;
        private readonly IRegister<SubtypeData> subtypeRegister;
        private readonly IRegister<RoomModifierData> roomModifierRegister;
        private readonly IRegister<RelicData> relicRegister;
        private readonly FallbackDataProvider dataProvider;

        public CharacterDataFinalizer(
            IModLogger<CharacterDataFinalizer> logger,
            ICache<IDefinition<CharacterData>> cache,
            IRegister<AssetReferenceGameObject> assetReferenceRegister,
            IRegister<CharacterTriggerData> triggerRegister,
            IRegister<VfxAtLoc> vfxRegister,
            IRegister<StatusEffectData> statusRegister,
            IRegister<CardData> cardRegister,
            IRegister<CharacterChatterData> chatterRegister,
            IRegister<SubtypeData> subtypeRegister,
            IRegister<RoomModifierData> roomModifierRegister,
            IRegister<RelicData> relicRegister,
            FallbackDataProvider dataProvider
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.assetReferenceRegister = assetReferenceRegister;
            this.triggerRegister = triggerRegister;
            this.vfxRegister = vfxRegister;
            this.statusRegister = statusRegister;
            this.cardRegister = cardRegister;
            this.chatterRegister = chatterRegister;
            this.subtypeRegister = subtypeRegister;
            this.roomModifierRegister = roomModifierRegister;
            this.relicRegister = relicRegister;
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
            var characterArtReference = configuration.GetSection("character_art").ParseReference();
            if (characterArtReference != null)
            {
                if (
                    assetReferenceRegister.TryLookupId(
                        characterArtReference.ToId(key, TemplateConstants.GameObject),
                        out var gameObject,
                        out var _
                    )
                )
                {
                    AccessTools
                        .Field(typeof(CharacterData), "characterPrefabVariantRef")
                        .SetValue(data, gameObject);
                }
            }

            //handle ability
            var abilityReference = configuration.GetSection("ability").ParseReference();
            if (abilityReference != null)
            {
                cardRegister.TryLookupName(abilityReference.ToId(key, TemplateConstants.Card), out var abilityCard, out var _);
                AccessTools.Field(typeof(CharacterData), "unitAbility").SetValue(data, abilityCard);
            }

            //handle equipment
            var graftedEquipmentCardReference = configuration.GetSection("grafted_equipment").ParseReference();
            if (graftedEquipmentCardReference != null)
            {
                cardRegister.TryLookupName(graftedEquipmentCardReference.ToId(key, TemplateConstants.Card), out var equipmentCard, out var _);
                AccessTools.Field(typeof(CharacterData), "graftedEquipment").SetValue(data, equipmentCard);
            }

            var projectilePrefabId = configuration.GetSection("projectile_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (vfxRegister.TryLookupId(projectilePrefabId, out var projectile_vfx, out var _))
            {
                AccessTools
                    .Field(typeof(CharacterData), "projectilePrefab")
                    .SetValue(data, projectile_vfx);
            }

            var attackVFXId = configuration.GetSection("attack_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (vfxRegister.TryLookupId(attackVFXId, out var attack_vfx, out var _))
            {
                AccessTools.Field(typeof(CharacterData), "attackVFX").SetValue(data, attack_vfx);
            }

            var impactVFXId = configuration.GetSection("impact_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (vfxRegister.TryLookupId(impactVFXId, out var impact_vfx, out var _))
            {
                AccessTools.Field(typeof(CharacterData), "impactVFX").SetValue(data, impact_vfx);
            }

            var deathVFXId = configuration.GetSection("death_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (vfxRegister.TryLookupId(deathVFXId, out var death_vfx, out var _))
            {
                AccessTools.Field(typeof(CharacterData), "deathVFX").SetValue(data, death_vfx);
            }

            var bossSpellCastVFXId = configuration.GetSection("boss_spell_cast_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (vfxRegister.TryLookupId(bossSpellCastVFXId, out var boss_cast_vfx, out var _))
            {
                AccessTools
                    .Field(typeof(CharacterData), "bossSpellCastVFX")
                    .SetValue(data, boss_cast_vfx);
            }

            var bossRoomSpellCastVFXId = configuration.GetSection("boss_room_cast_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (vfxRegister.TryLookupId(bossRoomSpellCastVFXId, out var boss_room_cast_vfx, out var _))
            {
                AccessTools
                    .Field(typeof(CharacterData), "bossRoomSpellCastVFX")
                    .SetValue(data, boss_room_cast_vfx);
            }

            var checkOverride = configuration.GetSection("override").ParseBool() ?? false;

            //handle triggers
            var triggerDatas = data.GetTriggers().ToList() ?? [];
            if (checkOverride)
            {
                triggerDatas.Clear();
            }
            var triggerReferences = configuration
                .GetSection("triggers")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var reference in triggerReferences)
            {
                if (
                    triggerRegister.TryLookupId(
                        reference.ToId(key, TemplateConstants.CharacterTrigger),
                        out var trigger,
                        out var _
                    )
                )
                {
                    triggerDatas.Add(trigger);
                }
            }
            AccessTools.Field(typeof(CharacterData), "triggers").SetValue(data, triggerDatas);

            //status effect immunities
            var statusEffectImmunities = data.GetStatusEffectImmunities()?.ToList() ?? [];
            if (checkOverride)
            {
                statusEffectImmunities.Clear();
            }
            var statusImmunityReferences = configuration.GetSection("status_effect_immunities")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var reference in statusImmunityReferences)
            {
                var statusEffectId = reference.ToId(key, TemplateConstants.StatusEffect);
                if (statusRegister.TryLookupId(statusEffectId, out var statusEffectData, out var _))
                {
                    statusEffectImmunities.Add(statusEffectData.GetStatusId());
                }
            }
            AccessTools
                .Field(typeof(CharacterData), "statusEffectImmunities")
                .SetValue(data, statusEffectImmunities.ToArray());

            //status
            var startingStatusEffects = new List<StatusEffectStackData>();
            if (!checkOverride)
            {
                var startingStatusEffects2 = (StatusEffectStackData[])
                    AccessTools
                        .Field(typeof(CharacterData), "startingStatusEffects")
                        .GetValue(data);
                if (startingStatusEffects2 != null)
                    startingStatusEffects.AddRange(startingStatusEffects2);
            }
            foreach (var child in configuration.GetSection("starting_status_effects").GetChildren())
            {
                var reference = child.GetSection("status").ParseReference();
                if (reference == null)
                    continue;
                var statusEffectId = reference.ToId(key, TemplateConstants.StatusEffect);
                if (statusRegister.TryLookupId(statusEffectId, out var statusEffectData, out var _))
                {
                    startingStatusEffects.Add(new StatusEffectStackData()
                    {
                        statusId = statusEffectData.GetStatusId(),
                        count = child.GetSection("count").ParseInt() ?? 0,
                    });
                }
            }
            AccessTools
                .Field(typeof(CharacterData), "startingStatusEffects")
                .SetValue(data, startingStatusEffects.ToArray());

            // TODO checkOverride is not honored, should allow merging the existing data.
            var chatterReference = configuration.GetSection("chatter").ParseReference();
            if (chatterReference != null)
            {
                if (chatterRegister.TryLookupId(chatterReference.ToId(key, TemplateConstants.Chatter), out var lookup, out var _))
                {
                    AccessTools.Field(typeof(CharacterData), "characterChatterData").SetValue(data, lookup);
                }
            }

            //subtypes
            var subtypes =
                (List<string>)
                    AccessTools.Field(typeof(CharacterData), "subtypeKeys").GetValue(data) ?? [];
            if (checkOverride)
            {
                subtypes.Clear();
            }
            var subtypeReferences = configuration.GetSection("subtypes")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var reference in subtypeReferences)
            {
                if (subtypeRegister.TryLookupId(reference.ToId(key, TemplateConstants.Subtype), out var lookup, out var _))
                {
                    subtypes.Add(lookup.Key);
                }
            }
            AccessTools.Field(typeof(CharacterData), "subtypeKeys").SetValue(data, subtypes);

            var roomModifiers = new List<RoomModifierData>();
            if (!checkOverride)
            {
                var roomModifiers2 = (List<RoomModifierData>)
                    AccessTools
                        .Field(typeof(CharacterData), "roomModifiers")
                        .GetValue(data);
                if (roomModifiers2 != null)
                    roomModifiers.AddRange(roomModifiers2);
            }
            var roomModifierReferences = configuration.GetSection("room_modifiers")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var reference in roomModifierReferences)
            {
                if (roomModifierRegister.TryLookupId(reference.ToId(key, TemplateConstants.RoomModifier), out var roomModifierData, out var _))
                {
                    roomModifiers.Add(roomModifierData);
                }
            }
            AccessTools
                .Field(typeof(CharacterData), "roomModifiers")
                .SetValue(data, roomModifiers);

            var relicReference = configuration.GetSection("enemy_relic").ParseReference();
            if (relicReference != null)
            {
                relicRegister.TryLookupId(relicReference.ToId(key, TemplateConstants.RelicData), out var relic, out var _);
                AccessTools.Field(typeof(RelicEffectData), "enemyRelicData").SetValue(data, relic);
            }

            AccessTools
                .Field(typeof(CharacterData), "fallbackData")
                .SetValue(data, dataProvider.FallbackData);
        }
    }
}
