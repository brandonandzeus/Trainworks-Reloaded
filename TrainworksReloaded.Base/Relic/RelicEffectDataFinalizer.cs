using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TrainworksReloaded.Core.Interfaces;
using TrainworksReloaded.Base.Extensions;
using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core.Extensions;

namespace TrainworksReloaded.Base.Relic
{
    public class RelicEffectDataFinalizer : IDataFinalizer
    {
        private readonly IModLogger<RelicEffectDataFinalizer> logger;
        private readonly ICache<IDefinition<RelicEffectData>> cache;
        private readonly IRegister<CardEffectData> cardEffectRegister;
        private readonly IRegister<CardPool> cardPoolRegister;
        private readonly IRegister<CharacterData> characterRegister;
        private readonly IRegister<StatusEffectData> statusEffectRegister;
        private readonly IRegister<CardTraitData> traitRegister;
        private readonly IRegister<CharacterTriggerData> triggerRegister;
        private readonly IRegister<CardData> cardRegister;
        private readonly IRegister<CardUpgradeData> upgradeRegister;
        private readonly IRegister<RewardData> rewardRegister;
        private readonly IRegister<CardUpgradeMaskData> cardUpgradeMaskRegister;
        private readonly IRegister<SubtypeData> subtypeRegister;

        public RelicEffectDataFinalizer(
            IModLogger<RelicEffectDataFinalizer> logger,
            ICache<IDefinition<RelicEffectData>> cache,
            IRegister<CardEffectData> cardEffectRegister,
            IRegister<CardPool> cardPoolRegister,
            IRegister<CharacterData> characterRegister,
            IRegister<CardTraitData> traitRegister,
            IRegister<CharacterTriggerData> triggerRegister,
            IRegister<CardData> cardRegister,
            IRegister<CardUpgradeData> upgradeRegister,
            IRegister<StatusEffectData> statusRegister,
            IRegister<RewardData> rewardRegister,
            IRegister<CardUpgradeMaskData> cardUpgradeMaskRegister,
            IRegister<SubtypeData> subtypeRegister
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.cardEffectRegister = cardEffectRegister;
            this.cardPoolRegister = cardPoolRegister;
            this.characterRegister = characterRegister;
            this.traitRegister = traitRegister;
            this.triggerRegister = triggerRegister;
            this.cardRegister = cardRegister;
            this.upgradeRegister = upgradeRegister;
            this.statusEffectRegister = statusRegister;
            this.rewardRegister = rewardRegister;
            this.cardUpgradeMaskRegister = cardUpgradeMaskRegister;
            this.subtypeRegister = subtypeRegister;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeRelicEffectData(definition);
            }
            cache.Clear();
        }

        private void FinalizeRelicEffectData(IDefinition<RelicEffectData> definition)
        {
            /*
            Types that need registers but don't have them yet:
            - AdditionalTooltipData
            - CharacterSubstitution
            - RelicEffectCondition
            - RarityTicketMultiplier
            - MerchantData
            - GrantableRewardData
            - CardSetBuilder
            - CollectibleRelicData
            - CovenantData
            - EnhancerPool
            - RandomChampionPool
            - RoomData
            */ 
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(Core.Interfaces.LogLevel.Info, $"Finalizing RelicEffect {key}... ");

            // Handle status effects
            var statusEffects = new List<StatusEffectStackData>();
            var statusEffectsConfig = configuration.GetSection("param_status_effects").GetChildren();
            foreach (var child in statusEffectsConfig)
            {
                var idConfig = child?.GetSection("status").Value;
                if (idConfig == null)
                    continue;
                var statusEffectId = idConfig.ToId(key, TemplateConstants.StatusEffect);
                string statusId = idConfig;
                if (statusEffectRegister.TryLookupId(statusEffectId, out var statusEffectData, out var _))
                {
                    statusId = statusEffectData.GetStatusId();
                }
                statusEffects.Add(new StatusEffectStackData()
                {
                    statusId = statusId,
                    count = child?.GetSection("count").ParseInt() ?? 0,
                });
            }
            AccessTools
                .Field(typeof(RelicEffectData), "paramStatusEffects")
                .SetValue(data, statusEffects.ToArray());

            // Handle card effects
            var cardEffects = new List<CardEffectData>();
            var cardEffectsConfig = configuration.GetSection("param_card_effects").GetChildren();
            foreach (var cardEffectConfig in cardEffectsConfig)
            {
                if (cardEffectConfig == null) continue;
                
                var idConfig = cardEffectConfig.GetSection("id").Value;
                if (idConfig == null) continue;

                var id = idConfig.ToId(key, TemplateConstants.Effect);
                if (cardEffectRegister.TryLookupId(id, out var cardEffect, out var _))
                {
                    cardEffects.Add(cardEffect);
                }
            }
            if (cardEffects.Count != 0)
            {
                AccessTools.Field(typeof(RelicEffectData), "paramCardEffects").SetValue(data, cardEffects);
            }

            // Handle card pool
            var cardPoolId = configuration.GetSection("param_card_pool").ParseString();
            if (cardPoolId != null && cardPoolRegister.TryLookupId(cardPoolId.ToId(key, TemplateConstants.CardPool), out var cardPool, out var _))
            {
                AccessTools.Field(typeof(RelicEffectData), "paramCardPool").SetValue(data, cardPool);
            }

            // Handle characters
            var characters = new List<CharacterData>();
            var charactersConfig = configuration.GetSection("param_characters").GetChildren();
            foreach (var characterConfig in charactersConfig)
            {
                if (characterConfig == null) continue;
                
                var idConfig = characterConfig.GetSection("id").Value;
                if (idConfig == null) continue;

                var id = idConfig.ToId(key, TemplateConstants.Character);
                if (characterRegister.TryLookupName(id, out var character, out var _))
                {
                    characters.Add(character);
                }
            }
            AccessTools.Field(typeof(RelicEffectData), "paramCharacters").SetValue(data, characters);

            // Handle traits
            var traits = new List<CardTraitData>();
            var traitsConfig = configuration.GetSection("traits").GetChildren();
            foreach (var traitConfig in traitsConfig)
            {
                if (traitConfig == null) continue;
                
                var idConfig = traitConfig.GetSection("id").Value;
                if (idConfig == null) continue;

                var id = idConfig.ToId(key, TemplateConstants.Trait);
                if (traitRegister.TryLookupId(id, out var trait, out var _))
                {
                    traits.Add(trait);
                }
            }
            if (traits.Count > 0)
            {
                AccessTools.Field(typeof(RelicEffectData), "traits").SetValue(data, traits);
            }

            // Handle excluded traits
            var excludedTraits = new List<CardTraitData>();
            var excludedTraitsConfig = configuration.GetSection("excluded_traits").GetChildren();
            foreach (var traitConfig in excludedTraitsConfig)
            {
                if (traitConfig == null) continue;
                
                var idConfig = traitConfig.GetSection("id").Value;
                if (idConfig == null) continue;

                var id = idConfig.ToId(key, TemplateConstants.Trait);
                if (traitRegister.TryLookupId(id, out var trait, out var _))
                {
                    excludedTraits.Add(trait);
                }
            }
            if (excludedTraits.Count > 0)
            {
                AccessTools.Field(typeof(RelicEffectData), "excludedTraits").SetValue(data, excludedTraits);
            }

            // Handle triggers
            var triggers = new List<CharacterTriggerData>();
            var triggersConfig = configuration.GetSection("triggers").GetChildren();
            foreach (var triggerConfig in triggersConfig)
            {
                if (triggerConfig == null) continue;
                
                var idConfig = triggerConfig.GetSection("id").Value;
                if (idConfig == null) continue;

                var id = idConfig.ToId(key, TemplateConstants.CharacterTrigger);
                if (triggerRegister.TryLookupId(id, out var trigger, out var _))
                {
                    triggers.Add(trigger);
                }
            }
            if (triggers.Count != 0)
            {
                AccessTools.Field(typeof(RelicEffectData), "triggers").SetValue(data, triggers);
            }

            // Handle card data
            var cardDataId = configuration.GetSection("param_card_data").ParseString();
            if (cardDataId != null && cardRegister.TryLookupId(cardDataId.ToId(key, TemplateConstants.Card), out var cardData, out var _))
            {
                AccessTools.Field(typeof(RelicEffectData), "paramCardData").SetValue(data, cardData);
            }

            // Handle card upgrade data
            var cardUpgradeDataId = configuration.GetSection("param_card_upgrade_data").ParseString();
            if (cardUpgradeDataId != null && upgradeRegister.TryLookupName(cardUpgradeDataId.ToId(key, TemplateConstants.Upgrade), out var cardUpgradeData, out var _))
            {
                AccessTools.Field(typeof(RelicEffectData), "paramCardUpgradeData").SetValue(data, cardUpgradeData);
            }

            //handle paramReward 
            var paramReward = configuration.GetSection("param_reward").ParseString();
            if (paramReward != null && rewardRegister.TryLookupId(paramReward.ToId(key, TemplateConstants.RewardData), out var reward, out var _))
            {
                AccessTools.Field(typeof(RelicEffectData), "paramReward").SetValue(data, reward);
            }

            //handle paramReward 2
            var paramReward2 = configuration.GetSection("param_reward_2").ParseString();
            if (paramReward2 != null && rewardRegister.TryLookupId(paramReward2.ToId(key, TemplateConstants.RewardData), out var reward2, out var _))
            {
                AccessTools.Field(typeof(RelicEffectData), "paramReward2").SetValue(data, reward2);
            }

            //handle paramCardFilter
            var paramCardFilter = configuration.GetSection("param_card_filter").ParseString();
            if (paramCardFilter != null && cardUpgradeMaskRegister.TryLookupId(paramCardFilter.ToId(key, TemplateConstants.UpgradeMask), out var cardFilter, out var _))
            {
                AccessTools.Field(typeof(RelicEffectData), "paramCardFilter").SetValue(data, cardFilter);
            }
            
            //handle paramCardFilterSecondary
            var paramCardFilterSecondary = configuration.GetSection("param_card_filter_secondary").ParseString();
            if (paramCardFilterSecondary != null && cardUpgradeMaskRegister.TryLookupId(paramCardFilterSecondary.ToId(key, TemplateConstants.UpgradeMask), out var cardFilterSecondary, out var _))
            {
                AccessTools.Field(typeof(RelicEffectData), "paramCardFilterSecondary").SetValue(data, cardFilterSecondary);
            }

            // Handle character subtype
            var characterSubtype = "SubtypesData_None";
            var characterSubtypeId = configuration.GetSection("character_subtype").ParseString();
            if (characterSubtypeId != null)
            {
                if (subtypeRegister.TryLookupId(
                    characterSubtypeId.ToId(key, TemplateConstants.Subtype),
                    out var lookup,
                    out var _))
                {
                    characterSubtype = lookup.Key;
                }
            }
            AccessTools.Field(typeof(RelicEffectData), "paramCharacterSubtype").SetValue(data, characterSubtype);


            // Handle excluded character subtypes
            List<string> excludedSubtypes = [];
            foreach (var id in configuration.GetSection("excluded_character_subtypes").GetChildren())
            {
                var idConfig = id?.ParseString();
                if (idConfig == null || !subtypeRegister.TryLookupId(
                    idConfig.ToId(key, TemplateConstants.Subtype),
                    out var lookup,
                    out var _))
                {
                    continue;
                }
                excludedSubtypes.Add(lookup.Key);
            }
            AccessTools.Field(typeof(RelicEffectData), "paramExcludeCharacterSubtypes").SetValue(data, excludedSubtypes.ToArray());
        }
    }
}
