using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace TrainworksReloaded.Base.Prefab
{
    public class FallbackDataProvider
    {
        private readonly Lazy<SaveManager> SaveManager;

        public FallbackDataProvider(GameDataClient client)
        {
            SaveManager = new Lazy<SaveManager>(() =>
            {
                if (client.TryGetProvider<SaveManager>(out var provider))
                {
                    return provider;
                }
                else
                {
                    return new SaveManager();
                }
            });
        }

        public FallbackData FallbackData =>
            (FallbackData)
                AccessTools
                    .Field(typeof(CardData), "fallbackData")
                    .GetValue(SaveManager.Value.GetAllGameData().GetAllCardData().First());
    }
}
