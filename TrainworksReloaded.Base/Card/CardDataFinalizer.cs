using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HarmonyLib;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Prefab;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine.AddressableAssets;
using static TrainworksReloaded.Base.Extensions.ParseReferenceExtensions;

namespace TrainworksReloaded.Base.Card
{
    public class CardDataFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CardDataFinalizer> logger;
        private readonly ICache<IDefinition<CardData>> cache;
        private readonly IRegister<ClassData> classRegister;
        private readonly IRegister<CardData> cardRegister;
        private readonly IRegister<CardTraitData> traitRegister;
        private readonly IRegister<CardEffectData> effectRegister;
        private readonly IRegister<AssetReferenceGameObject> assetReferenceRegister;
        private readonly IRegister<CardUpgradeData> upgradeRegister;
        private readonly IRegister<CardTriggerEffectData> triggerEffectRegister;
        private readonly IRegister<CharacterTriggerData> triggerDataRegister;
        private readonly IRegister<VfxAtLoc> vfxRegister;
        private readonly FallbackDataProvider fallbackDataProvider;

        public CardDataFinalizer(
            IModLogger<CardDataFinalizer> logger,
            ICache<IDefinition<CardData>> cache,
            IRegister<ClassData> classRegister,
            IRegister<CardData> cardRegister,
            IRegister<CardTraitData> traitRegister,
            IRegister<CardEffectData> effectRegister,
            IRegister<AssetReferenceGameObject> assetReferenceRegister,
            IRegister<CardUpgradeData> upgradeRegister,
            IRegister<CardTriggerEffectData> triggerEffectRegister,
            IRegister<CharacterTriggerData> triggerDataRegister,
            IRegister<VfxAtLoc> vfxRegister,
            FallbackDataProvider fallbackDataProvider
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.classRegister = classRegister;
            this.cardRegister = cardRegister;
            this.traitRegister = traitRegister;
            this.effectRegister = effectRegister;
            this.assetReferenceRegister = assetReferenceRegister;
            this.upgradeRegister = upgradeRegister;
            this.triggerEffectRegister = triggerEffectRegister;
            this.triggerDataRegister = triggerDataRegister;
            this.vfxRegister = vfxRegister;
            this.fallbackDataProvider = fallbackDataProvider;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeCardData(definition);
            }
            cache.Clear();
        }

        /// <summary>
        /// Finalize Card Definitions
        /// Handles Data to avoid lookup looks for names and ids
        /// </summary>
        /// <param name="definition"></param>
        private void FinalizeCardData(IDefinition<CardData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(Core.Interfaces.LogLevel.Info, $"Finalizing Card {data.name}... ");

            //handle linked class
            var classfield = configuration.GetSection("class").ParseString();
            if (
                classfield != null
                && classRegister.TryLookupName(
                    classfield.ToId(key, TemplateConstants.Class),
                    out var lookup,
                    out var _
                )
            )
            {
                AccessTools.Field(typeof(CardData), "linkedClass").SetValue(data, lookup);
            }

            //handle discovery cards
            var SharedDiscoveryCards = new List<CardData>();
            var SharedDiscoveryCardConfig = configuration
                .GetSection("shared_discovery_cards")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var cardReference in SharedDiscoveryCardConfig)
            {
                if (
                    cardRegister.TryLookupName(
                        cardReference.ToId(key, TemplateConstants.Card),
                        out var card,
                        out var _
                    )
                )
                {
                    SharedDiscoveryCards.Add(card);
                }
            }
            AccessTools
                .Field(typeof(CardData), "sharedDiscoveryCards")
                .SetValue(data, SharedDiscoveryCards);

            //handle mastery cards
            var SharedMasteryCards = new List<CardData>();
            var SharedMasteryCardConfig = configuration
                .GetSection("shared_mastery_cards")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var cardReference in SharedMasteryCardConfig)
            {
                if (
                    cardRegister.TryLookupName(
                        cardReference.ToId(key, TemplateConstants.Card),
                        out var card,
                        out var _
                    )
                )
                {
                    SharedMasteryCards.Add(card);
                }
            }
            AccessTools
                .Field(typeof(CardData), "sharedMasteryCards")
                .SetValue(data, SharedMasteryCards);

            //handle linked mastery card
            var MasteryCardRef = configuration.GetSection("linked_mastery_card").ParseReference();
            if (
                MasteryCardRef != null
                && cardRegister.TryLookupName(
                    MasteryCardRef.ToId(key, TemplateConstants.Card),
                    out var MasteryCard,
                    out var _
                )
            )
            {
                AccessTools
                    .Field(typeof(CardData), "linkedMasteryCard")
                    .SetValue(data, MasteryCard);
            }

            //handle art
            var cardArtReference = configuration.GetSection("card_art").ParseReference();
            if (cardArtReference != null)
            {
                if (
                    assetReferenceRegister.TryLookupId(
                        cardArtReference.ToId(key, TemplateConstants.GameObject),
                        out var gameObject,
                        out var _
                    )
                )
                {
                    AccessTools
                        .Field(typeof(CardData), "cardArtPrefabVariantRef")
                        .SetValue(data, gameObject);
                }
            }

            //handle traits
            var cardTraitDatas = new List<CardTraitData>();
            var cardTraitReferences = configuration.GetSection("traits")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var traitReference in cardTraitReferences)
            {
                var id = traitReference.ToId(key, TemplateConstants.Trait);
                if (traitRegister.TryLookupId(id, out var card, out var _))
                {
                    cardTraitDatas.Add(card);
                }
            }
            if (cardTraitDatas.Count != 0)
                AccessTools.Field(typeof(CardData), "traits").SetValue(data, cardTraitDatas);

            var cardEffectDatas = new List<CardEffectData>();
            var cardEffectDatasConfig = configuration.GetSection("effects")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var effectReference in cardEffectDatasConfig)
            {
                if (effectRegister.TryLookupId(effectReference.ToId(key, TemplateConstants.Effect), out var effect, out var _))
                {
                    cardEffectDatas.Add(effect);
                }
            }
            if (cardEffectDatas.Count != 0)
                AccessTools.Field(typeof(CardData), "effects").SetValue(data, cardEffectDatas);

            var cardTriggers = new List<CardTriggerEffectData>();
            var cardTriggerEffectDataConfig = configuration.GetSection("triggers")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var triggerReference in cardTriggerEffectDataConfig)
            {   
                var id = triggerReference.ToId(key, TemplateConstants.CardTrigger);
                if (triggerEffectRegister.TryLookupId(id, out var trigger, out var _))
                {
                    cardTriggers.Add(trigger);
                }
            }
            if (cardTriggers.Count != 0)
                AccessTools.Field(typeof(CardData), "triggers").SetValue(data, cardTriggers);

            var initialUpgrades = new List<CardUpgradeData>();
            var initialUpgradesConfig = configuration.GetSection("initial_upgrades")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var upgradeReference in initialUpgradesConfig)
            {
                var id = upgradeReference.ToId(key, TemplateConstants.Upgrade);
                if (upgradeRegister.TryLookupId(id, out var upgrade, out var _))
                {
                    initialUpgrades.Add(upgrade);
                }
            }
            if (initialUpgrades.Count != 0)
                AccessTools
                    .Field(typeof(CardData), "startingUpgrades")
                    .SetValue(data, initialUpgrades);

            var effectTriggers = new List<CharacterTriggerData>();
            var effectTriggersConfig = configuration.GetSection("effect_triggers")
                .GetChildren()
                .Select(x => x.ParseReference())
                .Where(x => x != null)
                .Cast<ReferencedObject>();
            foreach (var characterTriggerReference in cardEffectDatasConfig)
            {
                var id = characterTriggerReference.ToId(key, TemplateConstants.CharacterTrigger);
                if (triggerDataRegister.TryLookupId(id, out var trigger, out var _))
                {
                    effectTriggers.Add(trigger);
                }
            }
            if (effectTriggers.Count != 0)
                AccessTools
                    .Field(typeof(CardData), "effectTriggers")
                    .SetValue(data, effectTriggers);

            var offCooldownVFXId = configuration.GetSection("off_cooldown_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (
                vfxRegister.TryLookupId(
                    offCooldownVFXId,
                    out var offCooldownVfx,
                    out var _
                )
            )
            {
                AccessTools.Field(typeof(CardData), "offCooldownVFX").SetValue(data, offCooldownVfx);
            }

            var specialEdgeVFXId = configuration.GetSection("special_edge_vfx").ParseReference()?.ToId(key, TemplateConstants.Vfx) ?? "";
            if (
                vfxRegister.TryLookupId(
                    specialEdgeVFXId,
                    out var specialEdgeVfx,
                    out var _
                )
            )
            {
                AccessTools.Field(typeof(CardData), "specialEdgeVFX").SetValue(data, specialEdgeVfx);
            }

            AccessTools
                .Field(typeof(CardData), "fallbackData")
                .SetValue(data, fallbackDataProvider.FallbackData);
        }
    }
}
