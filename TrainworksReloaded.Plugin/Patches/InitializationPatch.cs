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
            //We inject data into AssetLoading Manager.
            var container = Railend.GetContainer();
            var register = container.GetInstance<CardDataRegister>();

            //add data to the existing main pools
            var delegator = container.GetInstance<VanillaCardPoolDelegator>();
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
            delegator.CardPoolToData.Clear(); //save memory
            //we add custom card pool so that the card data is loaded, even if it doesn't exist in any pool.
            ____assetLoadingData.CardPoolsAll.Add(register.CustomCardPool);

            var classRegister = container.GetInstance<ClassDataRegister>();
            var classDatas =
                (List<ClassData>)
                    AccessTools
                        .Field(typeof(BalanceData), "classDatas")
                        .GetValue(____assetLoadingData.BalanceData);
            classDatas.AddRange(classRegister.Values);

            //Load localization at this time
            var localization = container.GetInstance<CustomLocalizationTermRegistry>();
            localization.LoadData();

            //Run finalization steps to populate data that required all other data to be loaded first
            var finalizer = container.GetInstance<Finalizer>();
            finalizer.FinalizeData();
        }
    }
}
