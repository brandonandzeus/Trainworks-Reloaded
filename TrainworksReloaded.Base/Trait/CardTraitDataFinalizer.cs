﻿using HarmonyLib;
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

            logger.Log(LogLevel.Debug, 
                $"Finalizing Card Trait {definition.Id.ToId(key, TemplateConstants.Trait)}... "
            );

            // Card
            var cardReference = configuration.GetDeprecatedSection("param_card_data", "param_card").ParseReference();
            CardData? card = null;
            if (cardReference != null)
            {
                cardRegister.TryLookupId(cardReference.ToId(key, TemplateConstants.Card), out card, out var _);
            }
            AccessTools
                .Field(typeof(CardTraitData), "paramCardData")
                .SetValue(data, card);

            // CardUpgrade
            var cardUpgradeReference = configuration.GetSection("param_upgrade").ParseReference();
            CardUpgradeData? cardUpgrade = null;
            if (cardUpgradeReference != null)
            {
                var cardUpgradeId = cardUpgradeReference.ToId(key, TemplateConstants.Upgrade);
                upgradeRegister.TryLookupId(cardUpgradeId, out cardUpgrade, out var _);
            }
            AccessTools
                .Field(typeof(CardTraitData), "paramCardUpgradeData")
                .SetValue(data, cardUpgrade);

            // Status Effects
            List<StatusEffectStackData> paramStatusEffects = [];
            foreach (var child in configuration.GetSection("param_status_effects").GetChildren())
            {
                var statusReference = child.GetSection("status").ParseReference();
                if (statusReference == null)
                    continue;
                var statusEffectId = statusReference.ToId(key, TemplateConstants.StatusEffect);
                if (statusRegister.TryLookupId(statusEffectId, out var statusEffectData, out var _))
                {
                    paramStatusEffects.Add(new StatusEffectStackData
                    {
                        statusId = statusEffectData.GetStatusId(),
                        count = child.GetSection("count").ParseInt() ?? 0,
                    });
                }
            }
            AccessTools
                .Field(typeof(CardTraitData), "paramStatusEffects")
                .SetValue(data, paramStatusEffects.ToArray());

            var paramSubtype = "SubtypesData_None";
            var paramSubtypeReference = configuration.GetSection("param_subtype").ParseReference();
            if (paramSubtypeReference != null)
            {
                if (subtypeRegister.TryLookupId(
                    paramSubtypeReference.ToId(key, TemplateConstants.Subtype),
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
