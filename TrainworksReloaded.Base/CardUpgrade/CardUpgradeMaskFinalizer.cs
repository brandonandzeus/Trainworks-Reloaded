using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.CardUpgrade
{
    public class CardUpgradeMaskFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CardUpgradeMaskFinalizer> logger;
        private readonly ICache<IDefinition<CardUpgradeMaskData>> cache;
        private readonly IRegister<CardUpgradeData> upgradeRegister;
        private readonly IRegister<StatusEffectData> statusRegister;
        private readonly IRegister<ClassData> classRegister;
        private readonly IRegister<CardPool> poolRegister;

        public CardUpgradeMaskFinalizer(
            IModLogger<CardUpgradeMaskFinalizer> logger,
            ICache<IDefinition<CardUpgradeMaskData>> cache,
            IRegister<CardUpgradeData> upgradeRegister,
            IRegister<StatusEffectData> statusRegister,
            IRegister<ClassData> classRegister,
            IRegister<CardPool> poolRegister
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.upgradeRegister = upgradeRegister;
            this.statusRegister = statusRegister;
            this.classRegister = classRegister;
            this.poolRegister = poolRegister;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeCardUpgradeMask(definition);
            }
            cache.Clear();
        }

        private void FinalizeCardUpgradeMask(IDefinition<CardUpgradeMaskData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(Core.Interfaces.LogLevel.Info, $"Finalizing Upgrade Mask {data.name}... ");

            List<ClassData> requiredClasses = [];
            foreach (var child in configuration.GetSection("required_class").GetChildren())
            {
                var id = child?.ParseString();
                if (id == null || 
                    !classRegister.TryLookupName(
                        id.ToId(key, TemplateConstants.Class),
                        out var lookup,
                        out var _))
                {
                    continue;
                }
                requiredClasses.Add(lookup);
            }
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredLinkedClans").SetValue(data, requiredClasses);

            List<ClassData> excludedClasses = [];
            foreach (var child in configuration.GetSection("excluded_class").GetChildren())
            {
                var id = child?.ParseString();
                if (id == null ||
                    !classRegister.TryLookupName(
                        id.ToId(key, TemplateConstants.Class),
                        out var lookup,
                        out var _))
                {
                    continue;
                }
                excludedClasses.Add(lookup);
            }
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedLinkedClans").SetValue(data, excludedClasses);

            List<StatusEffectStackData> requiredStatus = [];
            foreach (var child in configuration.GetSection("required_status").GetChildren())
            {
                var idConfig = child?.GetSection("status").Value;
                if (idConfig == null)
                    continue;
                var statusEffectId = idConfig.ToId(key, TemplateConstants.StatusEffect);
                string statusId = idConfig;
                if (statusRegister.TryLookupId(statusEffectId, out var statusEffectData, out var _))
                {
                    statusId = statusEffectData.GetStatusId();
                }
                requiredStatus.Add(new StatusEffectStackData()
                {
                    statusId = statusId,
                    count = child?.GetSection("count").ParseInt() ?? 0,
                });
            }
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredStatusEffects").SetValue(data, requiredStatus);

            List<StatusEffectStackData> excludedStatus = [];
            foreach (var child in configuration.GetSection("excluded_status").GetChildren())
            {
                var idConfig = child?.GetSection("status").Value;
                if (idConfig == null)
                    continue;
                var statusEffectId = idConfig.ToId(key, TemplateConstants.StatusEffect);
                string statusId = idConfig;
                if (statusRegister.TryLookupId(statusEffectId, out var statusEffectData, out var _))
                {
                    statusId = statusEffectData.GetStatusId();
                }
                excludedStatus.Add(new StatusEffectStackData()
                {
                    statusId = statusId,
                    count = child?.GetSection("count").ParseInt() ?? 0,
                });
            }
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedStatusEffects").SetValue(data, excludedStatus);

            List<CardPool> allowedPools = [];
            foreach (var child in configuration.GetSection("allowed_pools").GetChildren())
            {
                var id = child?.ParseString();
                if (id == null ||
                    !poolRegister.TryLookupId(
                        id.ToId(key, TemplateConstants.CardPool),
                        out var lookup,
                        out var _))
                {
                    continue;
                }
                allowedPools.Add(lookup);
            }
            AccessTools.Field(typeof(CardUpgradeMaskData), "allowedCardPools").SetValue(data, allowedPools);

            List<CardPool> disallowedPools = [];
            foreach (var child in configuration.GetSection("disallowed_pools").GetChildren())
            {
                var id = child?.ParseString();
                if (id == null ||
                    !poolRegister.TryLookupId(
                        id.ToId(key, TemplateConstants.CardPool),
                        out var lookup,
                        out var _))
                {
                    continue;
                }
                disallowedPools.Add(lookup);
            }
            AccessTools.Field(typeof(CardUpgradeMaskData), "disallowedCardPools").SetValue(data, disallowedPools);

            List<CardUpgradeData> requiredUpgrades = [];
            foreach (var child in configuration.GetSection("required_upgrade").GetChildren())
            {
                var id = child?.ParseString();
                if (id == null ||
                    !upgradeRegister.TryLookupId(
                        id.ToId(key, TemplateConstants.Upgrade),
                        out var lookup,
                        out var _))
                {
                    continue;
                }
                requiredUpgrades.Add(lookup);
            }
            AccessTools.Field(typeof(CardUpgradeMaskData), "requiredCardUpgrades").SetValue(data, requiredUpgrades);
            
            List<CardUpgradeData> excludedUpgrades = [];
            foreach (var child in configuration.GetSection("excluded_upgrade").GetChildren())
            {
                var id = child?.ParseString();
                if (id == null ||
                    !upgradeRegister.TryLookupId(
                        id.ToId(key, TemplateConstants.Upgrade),
                        out var lookup,
                        out var _))
                {
                    continue;
                }
                excludedUpgrades.Add(lookup);
            }
            AccessTools.Field(typeof(CardUpgradeMaskData), "excludedCardUpgrades").SetValue(data, excludedUpgrades);
        }
    }
}
