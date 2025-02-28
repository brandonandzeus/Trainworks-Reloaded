using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Trait
{
    public class CardTraitDataFinalizer : IDataFinalizer
    {
        public void FinalizeData() { }

        private void FinalizeCardTraitData(
            IRegister<CardTraitData> service,
            CardTraitDefinition definition
        )
        {
            var configuration = definition.Configuration;
            var data = definition.Data;

            //AccessTools.Field(typeof(CardTraitData), "paramCardTraitData").SetValue(data, configuration.GetSection("cost").ParseInt() ?? defaultCost);
            //AccessTools.Field(typeof(CardTraitData), "paramCardUpgradeData").SetValue(data, configuration.GetSection("cost").ParseInt() ?? defaultCost);
            //AccessTools.Field(typeof(CardTraitData), "paramStatusEffects").SetValue(data, configuration.GetSection("cost").ParseInt() ?? defaultCost);
        }
    }
}
