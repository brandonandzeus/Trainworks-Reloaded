using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.CardUpgrade
{
    public class CardUpgradeFinalizer : IDataFinalizer
    {
        public void FinalizeData() { //
        }

        private void FinalizeCardUpgradeData(
            IRegister<CardUpgradeData> service,
            CardUpgradeDefinition definition
        ) { }
    }
}
