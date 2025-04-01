using System;
using System.Linq;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using static ShinyShoe.DLC;

namespace TrainworksReloaded.Base.Relic
{
    public class CollectableRelicDataFinalizerDecorator : IDataFinalizer
    {
        private readonly IModLogger<CollectableRelicDataFinalizerDecorator> logger;
        private readonly ICache<IDefinition<RelicData>> cache;
        private readonly IRegister<ClassData> classRegister;
        private readonly IDataFinalizer decoratee;
        private readonly VanillaRelicPoolDelegator relicPoolDelegator;

        public CollectableRelicDataFinalizerDecorator(
            IModLogger<CollectableRelicDataFinalizerDecorator> logger,
            ICache<IDefinition<RelicData>> cache,
            IRegister<ClassData> classRegister,
            IDataFinalizer decoratee,
            VanillaRelicPoolDelegator relicPoolDelegator
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.classRegister = classRegister;
            this.decoratee = decoratee;
            this.relicPoolDelegator = relicPoolDelegator;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeRelicData(definition);
            }
            decoratee.FinalizeData();
            cache.Clear();
        }

        private void FinalizeRelicData(IDefinition<RelicData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;
            var relicId = definition.Id.ToId(key, TemplateConstants.RelicData);

            if (data is not CollectableRelicData collectableRelic)
                return;

            logger.Log(
                Core.Interfaces.LogLevel.Info,
                $"Finalizing Collectable Relic Data {relicId}... "
            );

            // Handle linked class
            var linkedClassId = configuration.GetSection("class").ParseString();
            if (linkedClassId != null && classRegister.TryLookupId(linkedClassId.ToId(key, TemplateConstants.Class), out var linkedClass, out var _))
            {
                AccessTools.Field(typeof(CollectableRelicData), "linkedClass").SetValue(collectableRelic, linkedClass);
            }

            // Handle rarity
            var rarity = configuration.GetSection("rarity").ParseRarity() ?? CollectableRarity.Common;
            AccessTools.Field(typeof(CollectableRelicData), "rarity").SetValue(collectableRelic, rarity);

            // Handle unlock level
            var unlockLevel = configuration.GetSection("unlock_level").ParseInt() ?? 0;
            AccessTools.Field(typeof(CollectableRelicData), "unlockLevel").SetValue(collectableRelic, unlockLevel);

            // Handle story event flag
            var fromStoryEvent = configuration.GetSection("from_story_event").ParseBool() ?? false;
            AccessTools.Field(typeof(CollectableRelicData), "fromStoryEvent").SetValue(collectableRelic, fromStoryEvent);

            // Handle boss given flag
            var isBossGivenRelic = configuration.GetSection("is_boss_given").ParseBool() ?? false;
            AccessTools.Field(typeof(CollectableRelicData), "isBossGivenRelic").SetValue(collectableRelic, isBossGivenRelic);

            // Handle dragon's hoard flag
            var isDragonsHoardRelic = configuration.GetSection("is_dragons_hoard").ParseBool() ?? false;
            AccessTools.Field(typeof(CollectableRelicData), "isDragonsHoardRelic").SetValue(collectableRelic, isDragonsHoardRelic);

            // Handle ignore for no relic achievement flag
            var ignoreForNoRelicAchievement = configuration.GetSection("ignore_for_no_relic_achievement").ParseBool() ?? false;
            AccessTools.Field(typeof(CollectableRelicData), "ignoreForNoRelicAchievement").SetValue(collectableRelic, ignoreForNoRelicAchievement);

            // Handle required DLC
            var requiredDLC = configuration.GetSection("required_dlc").ParseDLC() ?? ShinyShoe.DLC.None;
            AccessTools.Field(typeof(CollectableRelicData), "requiredDLC").SetValue(collectableRelic, requiredDLC);

            // Handle FTUE deprioritization flag
            var deprioritizeInFtueDrafts = configuration.GetSection("deprioritize_in_ftue_drafts").ParseBool() ?? false;
            AccessTools.Field(typeof(CollectableRelicData), "deprioritizeInFtueDrafts").SetValue(collectableRelic, deprioritizeInFtueDrafts);

            // Handle force update count label flag
            var forceUpdateCountLabel = configuration.GetSection("force_update_count_label").ParseBool() ?? false;
            AccessTools.Field(typeof(CollectableRelicData), "forceUpdateCountLabel").SetValue(collectableRelic, forceUpdateCountLabel);

            // Handle pool
            var pool = configuration.GetSection("pool").ParseString();
            if (pool != null)
            {
                if (!relicPoolDelegator.RelicPoolToData.ContainsKey(pool))
                {
                    relicPoolDelegator.RelicPoolToData[pool] = [];
                }
                relicPoolDelegator.RelicPoolToData[pool].Add(collectableRelic); 
            }
        }
    }
} 