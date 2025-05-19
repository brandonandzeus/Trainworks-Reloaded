using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Malee;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine.UIElements;

namespace TrainworksReloaded.Base.Card
{
    public class CardPoolFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CardPoolFinalizer> logger;
        private readonly ICache<IDefinition<CardPool>> cache;
        private readonly IRegister<CardData> cardRegister;

        public CardPoolFinalizer(
            IModLogger<CardPoolFinalizer> logger,
            ICache<IDefinition<CardPool>> cache,
            IRegister<CardData> cardRegister
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.cardRegister = cardRegister;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeCardPoolData(definition);
            }
            cache.Clear();
        }

        private void FinalizeCardPoolData(IDefinition<CardPool> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(Core.Interfaces.LogLevel.Info, $"Finalizing Card Pool {data.name}... ");

            //handle cards
            var cardDatas = new List<CardData>();
            var cardDatasConfig = configuration.GetSection("cards").GetChildren();
            foreach (var configData in cardDatasConfig)
            {
                if (configData == null)
                {
                    continue;
                }
                var idConfig = configData.ParseString();
                if (idConfig == null)
                {
                    continue;
                }

                var id = idConfig.ToId(key, TemplateConstants.Card);
                if (cardRegister.TryLookupName(id, out var card, out var _))
                {
                    cardDatas.Add(card);
                }
            }
            if (cardDatas.Count != 0)
            {
                var cardDataList =
                    (ReorderableArray<CardData>)
                        AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(data);
                cardDataList.Clear();
                foreach (var item in cardDatas)
                {
                    cardDataList.Add(item);
                }
                AccessTools.Field(typeof(CardPool), "cardDataList").SetValue(data, cardDataList);
            }
        }
    }
}
