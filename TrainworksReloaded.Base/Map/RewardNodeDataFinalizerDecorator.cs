using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Reward;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Map
{
    public class RewardNodeDataFinalizerDecorator : IDataFinalizer
    {
        private readonly IModLogger<RewardNodeDataFinalizerDecorator> logger;
        private readonly ICache<IDefinition<MapNodeData>> cache;
        private readonly IRegister<ClassData> classDataRegister;
        private readonly IRegister<RewardData> rewardDataRegister;
        private readonly IDataFinalizer decoratee;

        public RewardNodeDataFinalizerDecorator(
            IModLogger<RewardNodeDataFinalizerDecorator> logger,
            ICache<IDefinition<MapNodeData>> cache,
            IRegister<ClassData> classDataRegister,
            IRegister<RewardData> rewardDataRegister,
            IDataFinalizer decoratee
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.classDataRegister = classDataRegister;
            this.rewardDataRegister = rewardDataRegister;
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
        private void FinalizeRewardData(IDefinition<MapNodeData> definition)
        {
            var configuration1 = definition.Configuration;
            var data1 = definition.Data;
            var key = definition.Key;
            if (data1 is not RewardNodeData data)
                return;

            var configuration = configuration1
                .GetSection("extensions")
                .GetChildren()
                .Where(xs => xs.GetSection("reward").Exists())
                .Select(xs => xs.GetSection("reward"))
                .First();
            if (configuration == null)
                return;

            logger.Log(
                Core.Interfaces.LogLevel.Info,
                $"Finalizing Reward Node Data {definition.Id.ToId(key, TemplateConstants.RewardData)}... "
            );

            //class
            var required_class = configuration.GetSection("required_class").ParseString();
            if (
                required_class != null
                && classDataRegister.TryLookupId(
                    required_class.ToId(key, TemplateConstants.Class),
                    out var classData,
                    out var _
                )
            )
            {
                AccessTools
                    .Field(typeof(RewardNodeData), "requiredClass")
                    .SetValue(data, classData);
            }

            //rewards
            var rewards = new List<RewardData>();
            var rewardsConfigs = configuration.GetSection("rewards").GetChildren();
            foreach (var rewardConfig in rewardsConfigs)
            {
                var id = rewardConfig.GetSection("id").ParseString();
                if (
                    id != null
                    && rewardDataRegister.TryLookupId(
                        id.ToId(key, TemplateConstants.RewardData),
                        out var rewardData,
                        out var _
                    )
                )
                {
                    rewards.Add(rewardData);
                }
            }

            AccessTools.Field(typeof(RewardNodeData), "rewards").SetValue(data, rewards);
        }
    }
}
