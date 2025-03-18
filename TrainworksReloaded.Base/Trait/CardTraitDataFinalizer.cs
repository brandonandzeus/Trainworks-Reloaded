using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Room;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Trait
{
    public class CardTraitDataFinalizer(
        IModLogger<CardTraitDataFinalizer> logger,
        ICache<IDefinition<CardTraitData>> cache,
        IRegister<CardUpgradeData> upgradeRegister,
        IRegister<CardData> cardRegister,
        IRegister<StatusEffectData> statusRegister
        ) : IDataFinalizer
    {
        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeCardTrait(definition);
            }
            cache.Clear();
        }

        private void FinalizeCardTrait(IDefinition<CardTraitData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(
                Core.Interfaces.LogLevel.Info,
                $"Finalizing Card Trait {definition.Id.ToId(key, TemplateConstants.Trait)}... "
            );

            // Card
            var cardConfig = configuration.GetSection("param_card_data").Value;
            CardData? card = null;
            if (cardConfig != null)
            {
                var cardId = cardConfig.ToId(key, TemplateConstants.Card);
                cardRegister.TryLookupId(cardId, out card, out var _);
            }
            AccessTools
                .Field(typeof(CardTraitData), "paramCardData")
                .SetValue(data, card);

            // CardUpgrade
            var cardUpgradeConfig = configuration.GetSection("param_card_upgrade_data").Value;
            CardUpgradeData? cardUpgrade = null;
            if (cardUpgradeConfig != null)
            {
                var cardUpgradeId = cardUpgradeConfig.ToId(key, TemplateConstants.Upgrade);
                upgradeRegister.TryLookupId(cardUpgradeId, out cardUpgrade, out var _);
            }
            AccessTools
                .Field(typeof(CardTraitData), "paramCardUpgradeData")
                .SetValue(data, cardUpgrade);

            // Status Effects
            List<StatusEffectStackData> paramStatusEffects = [];
            foreach (var child in configuration.GetSection("param_status_effects").GetChildren())
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
                paramStatusEffects.Add(new StatusEffectStackData()
                {
                    statusId = statusId,
                    count = child?.GetSection("count").ParseInt() ?? 0,
                });
            }
            AccessTools
                .Field(typeof(CardTraitData), "paramStatusEffects")
                .SetValue(data, paramStatusEffects.ToArray());
        }
    }
}
