using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Prefab;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
                    classfield.ToId(key, "Class"),
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
                .Select(x => x.GetSection("id").ParseString());
            foreach (var ConfigCard in SharedDiscoveryCardConfig)
            {
                if (ConfigCard == null)
                {
                    continue;
                }
                if (
                    cardRegister.TryLookupName(
                        ConfigCard.ToId(key, "Card"),
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
                .Select(x => x.GetSection("id").ParseString());
            foreach (var ConfigCard in SharedMasteryCardConfig)
            {
                if (ConfigCard == null)
                {
                    continue;
                }

                if (
                    cardRegister.TryLookupName(
                        ConfigCard.ToId(key, "Card"),
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

            //handle mastery card
            var MasteryCardInfo = configuration.GetSection("mastery_card").ParseString();
            if (
                MasteryCardInfo != null
                && cardRegister.TryLookupName(
                    MasteryCardInfo.ToId(key, "Card"),
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
            var cardArtReference = configuration.GetSection("card_art_reference").ParseString();
            if (cardArtReference != null)
            {
                var gameObjectName = cardArtReference.ToId(key, "GameObject");
                if (
                    assetReferenceRegister.TryLookupId(
                        gameObjectName,
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
            var cardTraitDatasConfig = configuration.GetSection("traits").GetChildren();
            foreach (var configTrait in cardTraitDatasConfig)
            {
                if (configTrait == null)
                {
                    continue;
                }
                var idConfig = configTrait.GetSection("id").Value;
                if (idConfig == null)
                {
                    continue;
                }

                var id = idConfig.ToId(key, "Trait");
                if (traitRegister.TryLookupId(id, out var card, out var _))
                {
                    cardTraitDatas.Add(card);
                }
            }
            if (cardTraitDatas.Count != 0)
                AccessTools.Field(typeof(CardData), "traits").SetValue(data, cardTraitDatas);

            var cardEffectDatas = new List<CardEffectData>();
            var cardEffectDatasConfig = configuration.GetSection("effects").GetChildren();
            foreach (var configEffect in cardEffectDatasConfig)
            {
                if (configEffect == null)
                {
                    continue;
                }
                var idConfig = configEffect.GetSection("id").Value;
                if (idConfig == null)
                {
                    continue;
                }
                var id = idConfig.ToId(key, "Effect");
                if (effectRegister.TryLookupId(id, out var card, out var _))
                {
                    cardEffectDatas.Add(card);
                }
            }

            if (cardEffectDatas.Count != 0)
                AccessTools.Field(typeof(CardData), "effects").SetValue(data, cardEffectDatas);

            var cardTriggers = new List<CardTriggerEffectData>();
            var cardTriggerEffectDataConfig = configuration.GetSection("triggers").GetChildren();
            foreach (var configTrigger in cardTriggerEffectDataConfig)
            {
                if (configTrigger == null)
                    continue;
                
                var idConfig = configTrigger.GetSection("id").Value;
                if (idConfig == null)
                {
                    continue;
                }
                var id = idConfig.ToId(key, "Trigger");
                if (triggerEffectRegister.TryLookupId(id, out var card, out var _))
                {
                    cardTriggers.Add(card);
                }
            }

            if (cardTriggers.Count != 0)
                AccessTools.Field(typeof(CardData), "triggers").SetValue(data, cardTriggers);

            var initialUpgrades = new List<CardUpgradeData>();
            var initialUpgradesConfig = configuration.GetSection("initial_upgrades").GetChildren();
            foreach (var upgradeConfig in initialUpgradesConfig)
            {
                if (upgradeConfig == null)
                {
                    continue;
                }
                var idConfig = upgradeConfig.GetSection("id").Value;
                if (idConfig == null)
                {
                    continue;
                }

                var id = idConfig.ToId(key, "Upgrade");
                if (upgradeRegister.TryLookupId(id, out var card, out var _))
                {
                    initialUpgrades.Add(card);
                }
            }

            if (initialUpgrades.Count != 0)
                AccessTools
                    .Field(typeof(CardData), "startingUpgrades")
                    .SetValue(data, initialUpgrades);

            AccessTools
                .Field(typeof(CardData), "fallbackData")
                .SetValue(data, fallbackDataProvider.FallbackData);
        }
    }
}
