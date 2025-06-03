using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Character;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Prefab;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using static TrainworksReloaded.Base.Extensions.ParseReferenceExtensions;

namespace TrainworksReloaded.Base.CardUpgrade
{
    public class CardUpgradeFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CardUpgradeFinalizer> logger;
        private readonly ICache<IDefinition<CardUpgradeData>> cache;
        private readonly IRegister<CardData> cardRegister;
        private readonly IRegister<CharacterTriggerData> triggerRegister;
        private readonly IRegister<RoomModifierData> roomModifierRegister;
        private readonly IRegister<CardTraitData> traitRegister;
        private readonly IRegister<CardTriggerEffectData> cardTriggerRegister;
        private readonly IRegister<CardUpgradeData> upgradeRegister;
        private readonly IRegister<VfxAtLoc> vfxRegister;
        private readonly IRegister<StatusEffectData> statusRegister;
        private readonly IRegister<Sprite> spriteRegister;
        private readonly IRegister<CardUpgradeMaskData> upgradeMaskRegister;

        public CardUpgradeFinalizer(
            IModLogger<CardUpgradeFinalizer> logger,
            ICache<IDefinition<CardUpgradeData>> cache,
            IRegister<CardData> cardRegister,
            IRegister<CharacterTriggerData> triggerRegister,
            IRegister<RoomModifierData> roomModifierRegister,
            IRegister<CardTraitData> traitRegister,
            IRegister<CardTriggerEffectData> cardTriggerRegister,
            IRegister<CardUpgradeData> upgradeRegister,
            IRegister<VfxAtLoc> vfxRegister,
            IRegister<StatusEffectData> statusRegister,
            IRegister<Sprite> spriteRegister,
            IRegister<CardUpgradeMaskData> upgradeMaskRegister
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.cardRegister = cardRegister;
            this.triggerRegister = triggerRegister;
            this.roomModifierRegister = roomModifierRegister;
            this.traitRegister = traitRegister;
            this.cardTriggerRegister = cardTriggerRegister;
            this.upgradeRegister = upgradeRegister;
            this.vfxRegister = vfxRegister;
            this.statusRegister = statusRegister;
            this.spriteRegister = spriteRegister;
            this.upgradeMaskRegister = upgradeMaskRegister;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeCardUpgradeData(definition);
            }
            cache.Clear();
        }

        private void FinalizeCardUpgradeData(IDefinition<CardUpgradeData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(Core.Interfaces.LogLevel.Info, $"Finalizing Upgrade {data.name}... ");

            //handle traits
            //traits do not default properly, if null, set to empty
            var traitDataUpgradesVal =
                (List<CardTraitData>)
                    AccessTools.Field(typeof(CardUpgradeData), "traitDataUpgrades").GetValue(data);
            if (traitDataUpgradesVal == null)
                AccessTools
                    .Field(typeof(CardUpgradeData), "traitDataUpgrades")
                    .SetValue(data, new List<CardTraitData>());

            var traitDataUpgrades = new List<CardTraitData>();
            var traitDataUpgradesReference = configuration.GetSection("trait_upgrades")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var traitReference in traitDataUpgradesReference)
            {
                var id = traitReference.ToId(key, TemplateConstants.Trait);
                if (traitRegister.TryLookupId(id, out var trait, out var _))
                {
                    traitDataUpgrades.Add(trait);
                }
            }
            if (traitDataUpgrades.Count != 0)
                AccessTools
                    .Field(typeof(CardUpgradeData), "traitDataUpgrades")
                    .SetValue(data, traitDataUpgrades);

            //handle triggers
            var triggerUpgrades = new List<CharacterTriggerData>();
            var triggerUpgradeReferences = configuration.GetSection("trigger_upgrades")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var triggerReference in triggerUpgradeReferences)
            {
                var id = triggerReference.ToId(key, TemplateConstants.CharacterTrigger);
                if (triggerRegister.TryLookupId(id, out var trigger, out var _))
                {
                    triggerUpgrades.Add(trigger);
                }
            }
            if (triggerUpgrades.Count != 0)
                AccessTools
                    .Field(typeof(CardUpgradeData), "triggerUpgrades")
                    .SetValue(data, triggerUpgrades);

            //handle card triggers
            var cardTriggerUpgrades = new List<CardTriggerEffectData>();
            var cardTriggerUpgradeReferences = configuration
                .GetSection("card_trigger_upgrades")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var cardTriggerReference in cardTriggerUpgradeReferences)
            {
                var id = cardTriggerReference.ToId(key, TemplateConstants.CardTrigger);
                if (cardTriggerRegister.TryLookupId(id, out var trigger, out var _))
                {
                    cardTriggerUpgrades.Add(trigger);
                }
            }
            if (cardTriggerUpgrades.Count != 0)
                AccessTools
                    .Field(typeof(CardUpgradeData), "cardTriggerUpgrades")
                    .SetValue(data, cardTriggerUpgrades);

            //handle roomModifiers
            var roomModifierUpgrades = new List<RoomModifierData>();
            var roomModifierUpgradeReferences = configuration
                .GetSection("room_modifier_upgrades")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var roomModifierReference in roomModifierUpgradeReferences)
            {
                var id = roomModifierReference.ToId(key, TemplateConstants.RoomModifier);
                if (roomModifierRegister.TryLookupId(id, out var roomModifier, out var _))
                {
                    roomModifierUpgrades.Add(roomModifier);
                }
            }
            if (roomModifierUpgrades.Count != 0)
                AccessTools
                    .Field(typeof(CardUpgradeData), "roomModifierUpgrades")
                    .SetValue(data, roomModifierUpgrades);

            //status
            var statusEffectUpgrades = new List<StatusEffectStackData>();
            var statusEffectUpgradesConfig = configuration.GetSection("status_effect_upgrades").GetChildren();
            foreach (var child in statusEffectUpgradesConfig)
            {
                var statusReference = child.GetSection("status").ParseReference();
                if (statusReference == null)
                    continue;

                var statusEffectId = statusReference.ToId(key, TemplateConstants.StatusEffect);
                // Make sure the status effect exists. If using @ notation.
                // else it is a vanilla status effect.
                if (statusRegister.TryLookupId(statusEffectId, out var statusEffectData, out var _))
                {
                    statusEffectUpgrades.Add(new StatusEffectStackData
                    {
                        statusId = statusEffectData.GetStatusId(),
                        count = child.GetSection("count").ParseInt() ?? 0,
                    });
                }
            }
            AccessTools
                .Field(typeof(CardUpgradeData), "statusEffectUpgrades")
                .SetValue(data, statusEffectUpgrades);

            //handle upgrades to remove
            var upgradesToRemove = new List<CardUpgradeData>();
            var upgradesToRemoveReferences = configuration.GetSection("remove_upgrades")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var removeUpgradeReference in upgradesToRemoveReferences)
            {
                var id = removeUpgradeReference.ToId(key, TemplateConstants.Upgrade);
                if (upgradeRegister.TryLookupId(id, out var upgrade, out var _))
                {
                    upgradesToRemove.Add(upgrade);
                }
            }
            if (upgradesToRemove.Count != 0)
                AccessTools
                    .Field(typeof(CardUpgradeData), "upgradesToRemove")
                    .SetValue(data, upgradesToRemove);

            var appliedVFXId = configuration.GetSection("applied_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (vfxRegister.TryLookupId(appliedVFXId, out var applied_vfx, out var _))
            {
                AccessTools
                    .Field(typeof(CardUpgradeData), "appliedVFX")
                    .SetValue(data, applied_vfx);
            }

            var removedVFXId = configuration.GetSection("removed_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (vfxRegister.TryLookupId(removedVFXId, out var removed_vfx, out var _))
            {
                AccessTools
                    .Field(typeof(CardUpgradeData), "removedVFX")
                    .SetValue(data, removed_vfx);
            }

            var persistentVFXId = configuration.GetSection("persistent_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (vfxRegister.TryLookupId(persistentVFXId, out var persistent_vfx, out var _))
            {
                AccessTools
                    .Field(typeof(CardUpgradeData), "persistentVFX")
                    .SetValue(data, persistent_vfx);
            }

            List<CardUpgradeMaskData> filters = [];
            var filterReferences = configuration.GetSection("filters")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var reference in filterReferences)
            {
                var id = reference.ToId(key, TemplateConstants.UpgradeMask);
                if (upgradeMaskRegister.TryLookupId(id, out var lookup, out var _))
                {
                    filters.Add(lookup);
                }
            }
            AccessTools.Field(typeof(CardUpgradeData), "filters").SetValue(data, filters);

            var abilityReference = configuration.GetSection("ability_upgrade").ParseReference();
            if (abilityReference != null)
            {
                cardRegister.TryLookupName(abilityReference.ToId(key, TemplateConstants.Card), out var abilityCard, out var _);
                AccessTools.Field(typeof(CardUpgradeData), "unitAbilityUpgrade").SetValue(data, abilityCard);
            }

            var spriteReference = configuration.GetSection("icon").ParseReference();
            if (spriteReference != null)
            {
                spriteRegister.TryLookupId(spriteReference.ToId(key, TemplateConstants.Sprite), out var spriteLookup, out var _);
                AccessTools.Field(typeof(CardUpgradeData), "upgradeIcon").SetValue(data, spriteLookup);
            }
        }
    }
}
