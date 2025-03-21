using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Reward
{
    public class RewardDataRegister : Dictionary<string, RewardData>, IRegister<RewardData>
    {
        private readonly IModLogger<CardpoolRegister> logger;

        public RewardDataRegister(GameDataClient client, IModLogger<CardpoolRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, RewardData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Register Reward {key}... ");
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out RewardData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out RewardData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(name, out lookup);
        }
    }
}
