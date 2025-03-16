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

namespace TrainworksReloaded.Base.Reward
{
    public class CardPoolRewardDataFinalizerDecorator : IDataFinalizer
    {
        private readonly IModLogger<CardPoolRewardDataFinalizerDecorator> logger;
        private readonly ICache<IDefinition<RewardData>> cache;
        private readonly IRegister<CardPool> cardPoolRegister;
        private readonly IDataFinalizer decoratee;

        public CardPoolRewardDataFinalizerDecorator(
            IModLogger<CardPoolRewardDataFinalizerDecorator> logger,
            ICache<IDefinition<RewardData>> cache,
            IRegister<CardPool> cardPoolRegister,
            IDataFinalizer decoratee
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.cardPoolRegister = cardPoolRegister;
            this.decoratee = decoratee;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeRewardData(definition);
            }
            decoratee.FinalizeData();
            cache.Clear();
        }

        /// <summary>
        /// Finalize Card Definitions
        /// Handles Data to avoid lookup looks for names and ids
        /// </summary>
        /// <param name="definition"></param>
        private void FinalizeRewardData(IDefinition<RewardData> definition)
        {
            var configuration1 = definition.Configuration;
            var data1 = definition.Data;
            var key = definition.Key;
            if (data1 is not CardPoolRewardData data)
                return;

            logger.Log(
                Core.Interfaces.LogLevel.Info,
                $"Finalizing Card Pool Reward Data {definition.Id.ToId(key, TemplateConstants.RewardData)}... "
            );

            var configuration = configuration1
                .GetSection("extensions")
                .GetChildren()
                .Select(xs => xs.GetSection("cardpool"))
                .First(xs => xs.GetSection("cardpool").Value != null);
            if (configuration == null)
                return;

            //cardpool
            var cardpool = configuration.GetSection("cardpool").ParseString();
            if (
                cardpool != null
                && cardPoolRegister.TryLookupId(
                    cardpool.ToId(key, TemplateConstants.CardPool),
                    out var cardpoolData,
                    out var _
                )
            )
            {
                AccessTools
                    .Field(typeof(CardPoolRewardData), "cardPool")
                    .SetValue(data, cardpoolData);
            }

            //card costs
            var cardCosts = configuration
                .GetSection("cost_overrides")
                .GetChildren()
                .Select(xs => new CardPoolRewardData.CardCosts()
                {
                    rarity = xs.GetSection("rarity").ParseRarity() ?? CollectableRarity.Common,
                    costs =
                    [
                        .. xs.GetSection("costs").GetChildren().Select(xs => xs.ParseInt() ?? 0),
                    ],
                })
                .ToList();
            AccessTools
                .Field(typeof(CardPoolRewardData), "cardCostsOverride")
                .SetValue(data, cardCosts);
        }
    }
}
