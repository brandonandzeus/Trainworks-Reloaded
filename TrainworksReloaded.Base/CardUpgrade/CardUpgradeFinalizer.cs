using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Character;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.CardUpgrade
{
    public class CardUpgradeFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CardUpgradeFinalizer> logger;
        private readonly ICache<IDefinition<CardUpgradeData>> cache;

        public CardUpgradeFinalizer(
            IModLogger<CardUpgradeFinalizer> logger,
            ICache<IDefinition<CardUpgradeData>> cache
        )
        {
            this.logger = logger;
            this.cache = cache;
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

            //status effects
            var status_effects = new List<StatusEffectStackData>();
            AccessTools
                .Field(typeof(CardUpgradeData), "statusEffectUpgrades")
                .SetValue(data, status_effects);

            //remove trait
            var removeTraitUpgrades = new List<string>();
            AccessTools
                .Field(typeof(CardUpgradeData), "removeTraitUpgrades")
                .SetValue(data, removeTraitUpgrades);

            //trait data
            var traitDataUpgrades = new List<CardTraitData>();
            AccessTools
                .Field(typeof(CardUpgradeData), "traitDataUpgrades")
                .SetValue(data, traitDataUpgrades);
        }
    }
}
