using System.Linq;
using HarmonyLib;
using Malee;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Impl;

namespace TrainworksReloaded.Plugin.Patches
{
    [HarmonyPatch(typeof(AssetLoadingManager), "Start")]
    public class InitializationPatch
    {
        public static void Postfix(AssetLoadingData ____assetLoadingData)
        {
            var container = Railend.GetContainer();
            var register = container.GetInstance<CardDataRegister>();
            var delegator = container.GetInstance<CardPoolDelegator>();
            foreach (
                var cardpool in ____assetLoadingData.CardPoolsAll.Union(
                    ____assetLoadingData.CardPoolsAlwaysLoad
                )
            )
            {
                if (cardpool != null && delegator.CardPoolToData.ContainsKey(cardpool.name))
                {
                    var cardsToAdd = delegator.CardPoolToData[cardpool.name];
                    var dataList =
                        (ReorderableArray<CardData>)
                            AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(cardpool);
                    foreach (var card in cardsToAdd)
                    {
                        dataList.Add(card);
                    }
                }
            }
            delegator.CardPoolToData.Clear();
            ____assetLoadingData.CardPoolsAll.Add(register.CustomCardPool);

            var classRegister = container.GetInstance<ClassDataRegister>();
            var classDatas =
                (List<ClassData>)
                    AccessTools
                        .Field(typeof(BalanceData), "classDatas")
                        .GetValue(____assetLoadingData.BalanceData);
            classDatas.AddRange(classRegister.Values);

            var localization = container.GetInstance<CustomLocalizationTermRegistry>();
            localization.LoadData();

            var finalizer = container.GetInstance<Finalizer>();
            finalizer.FinalizeData();
        }
    }
}
