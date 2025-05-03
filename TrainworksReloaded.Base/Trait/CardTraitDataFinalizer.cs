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
    public class CardTraitDataFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CardTraitDataFinalizer> logger;
        private readonly ICache<IDefinition<CardTraitData>> cache;
        private readonly IRegister<CardUpgradeData> upgradeRegister;
        private readonly IRegister<CardData> cardRegister;
        private readonly IRegister<StatusEffectData> statusRegister;
        private readonly IRegister<SubtypeData> subtypeRegister;

        public CardTraitDataFinalizer(
            IModLogger<CardTraitDataFinalizer> logger,
            ICache<IDefinition<CardTraitData>> cache,
            IRegister<CardUpgradeData> upgradeRegister,
            IRegister<CardData> cardRegister,
            IRegister<StatusEffectData> statusRegister,
            IRegister<SubtypeData> subtypeRegister
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.upgradeRegister = upgradeRegister;
            this.cardRegister = cardRegister;
            this.statusRegister = statusRegister;
            this.subtypeRegister = subtypeRegister;
        }

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
            var cardUpgradeConfig = configuration.GetSection("param_upgrade").Value;
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

            var paramSubtype = "SubtypesData_None";
            var paramSubtypeId = configuration.GetSection("param_subtype").ParseString();
            if (paramSubtypeId != null)
            {
                if (subtypeRegister.TryLookupId(
                    paramSubtypeId.ToId(key, TemplateConstants.Subtype),
                    out var lookup,
                    out var _
                ))
                {
                    paramSubtype = lookup.Key;
                }
            }
            AccessTools
                .Field(typeof(CardTraitData), "paramSubtype")
                .SetValue(data, paramSubtype);
        }
    }
}
