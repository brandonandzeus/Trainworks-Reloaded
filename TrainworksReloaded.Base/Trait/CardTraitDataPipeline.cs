using System;
using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Trait
{
    public class CardTraitDataPipeline : IDataPipeline<IRegister<CardTraitData>, CardTraitData>
    {
        private readonly PluginAtlas atlas;
        private readonly IModLogger<CardTraitDataPipeline> logger;

        public CardTraitDataPipeline(PluginAtlas atlas, IModLogger<CardTraitDataPipeline> logger)
        {
            this.atlas = atlas;
            this.logger = logger;
        }

        public List<IDefinition<CardTraitData>> Run(IRegister<CardTraitData> service)
        {
            var processList = new List<IDefinition<CardTraitData>>();
            foreach (var config in atlas.PluginDefinitions)
            {
                processList.AddRange(LoadTraits(service, config.Key, config.Value.Configuration));
            }
            return processList;
        }

        private List<CardTraitDefinition> LoadTraits(
            IRegister<CardTraitData> service,
            string key,
            IConfiguration pluginConfig
        )
        {
            var processList = new List<CardTraitDefinition>();
            foreach (var child in pluginConfig.GetSection("traits").GetChildren())
            {
                var data = LoadTraitConfiguration(service, key, child);
                if (data != null)
                {
                    processList.Add(data);
                }
            }
            return processList;
        }

        private CardTraitDefinition? LoadTraitConfiguration(
            IRegister<CardTraitData> service,
            string key,
            IConfiguration configuration
        )
        {
            var id = configuration.GetSection("id").ParseString();
            if (id == null)
            {
                return null;
            }
            var name = key.GetId("Trait", id);
            var data = new CardTraitData();

            // TraitState
            var traitStateName = configuration.GetSection("name").Value;
            if (traitStateName == null)
                return null;

            var modReference = configuration.GetSection("mod_reference").Value ?? key;
            var assembly = atlas.PluginDefinitions.GetValueOrDefault(modReference)?.Assembly;
            if (
                !traitStateName.GetFullyQualifiedName<CardTraitState>(
                    assembly,
                    out string? fullyQualifiedName
                )
            )
            {
                return null;
            }
            AccessTools
                .Field(typeof(CardTraitData), "traitStateName")
                .SetValue(data, fullyQualifiedName);

            var paramTrackedValue = CardStatistics.TrackedValueType.SubtypeInDeck;
            AccessTools
                .Field(typeof(CardTraitData), "paramTrackedValue")
                .SetValue(
                    data,
                    configuration.GetSection("track_type").ParseTrackedValueType()
                        ?? paramTrackedValue
                );

            var paramCardType = CardStatistics.CardTypeTarget.Any;
            AccessTools
                .Field(typeof(CardTraitData), "paramCardType")
                .SetValue(
                    data,
                    configuration.GetSection("card_type").ParseCardTypeTarget() ?? paramCardType
                );

            var paramEntryDuration = CardStatistics.EntryDuration.ThisTurn;
            AccessTools
                .Field(typeof(CardTraitData), "paramEntryDuration")
                .SetValue(
                    data,
                    configuration.GetSection("entry_duration").ParseEntryDuration()
                        ?? paramEntryDuration
                );

            var paramStr = "";
            AccessTools
                .Field(typeof(CardTraitData), "paramStr")
                .SetValue(data, configuration.GetSection("param_str").ParseString() ?? paramStr);

            var paramDescription = "";
            AccessTools
                .Field(typeof(CardTraitData), "paramDescription")
                .SetValue(
                    data,
                    configuration.GetSection("param_description").ParseString() ?? paramDescription
                );

            var paramInt = 0;
            AccessTools
                .Field(typeof(CardTraitData), "paramInt")
                .SetValue(data, configuration.GetSection("param_int").ParseInt() ?? paramInt);

            var paramInt2 = 0;
            AccessTools
                .Field(typeof(CardTraitData), "paramInt2")
                .SetValue(data, configuration.GetSection("param_int_2").ParseInt() ?? paramInt2);

            var paramInt3 = 0;
            AccessTools
                .Field(typeof(CardTraitData), "paramInt3")
                .SetValue(data, configuration.GetSection("param_int_3").ParseInt() ?? paramInt3);

            var paramFloat = 0.0f;
            AccessTools
                .Field(typeof(CardTraitData), "paramFloat")
                .SetValue(data, configuration.GetSection("param_float").ParseFloat() ?? paramFloat);

            var subtype = "SubtypeData_None";
            AccessTools
                .Field(typeof(CardTraitData), "paramSubtype")
                .SetValue(data, configuration.GetSection("param_subtype").ParseString() ?? subtype);

            var paramUseScalingParams = false;
            AccessTools
                .Field(typeof(CardTraitData), "paramUseScalingParams")
                .SetValue(
                    data,
                    configuration.GetSection("param_use_scaling_params").ParseBool()
                        ?? paramUseScalingParams
                );

            var paramBool = false;
            AccessTools
                .Field(typeof(CardTraitData), "paramBool")
                .SetValue(data, configuration.GetSection("param_bool").ParseBool() ?? paramBool);

            var traitIsRemovable = false;
            AccessTools
                .Field(typeof(CardTraitData), "traitIsRemovable")
                .SetValue(
                    data,
                    configuration.GetSection("trait_is_removable").ParseBool() ?? traitIsRemovable
                );

            var tooltipSuppressed = false;
            AccessTools
                .Field(typeof(CardTraitData), "tooltipSuppressed")
                .SetValue(
                    data,
                    configuration.GetSection("tooltip_suppressed").ParseBool() ?? tooltipSuppressed
                );

            var effectTextSuppressed = false;
            AccessTools
                .Field(typeof(CardTraitData), "effectTextSuppressed")
                .SetValue(
                    data,
                    configuration.GetSection("effect_text_suppressed").ParseBool()
                        ?? effectTextSuppressed
                );

            var statusEffectTooltipsSuppressed = false;
            AccessTools
                .Field(typeof(CardTraitData), "statusEffectTooltipsSuppressed")
                .SetValue(
                    data,
                    configuration.GetSection("status_effect_tooltips_suppressed").ParseBool()
                        ?? statusEffectTooltipsSuppressed
                );

            var paramTeamType = Team.Type.None;
            AccessTools
                .Field(typeof(CardTraitData), "paramTeamType")
                .SetValue(
                    data,
                    configuration.GetSection("param_team").ParseTeamType() ?? paramTeamType
                );

            var stackMode = CardTraitData.StackMode.None;
            AccessTools
                .Field(typeof(CardTraitData), "stackMode")
                .SetValue(
                    data,
                    configuration.GetSection("stack_mode").ParseStackMode() ?? stackMode
                );

            var drawInDeploymentPhase = false;
            AccessTools
                .Field(typeof(CardTraitData), "drawInDeploymentPhase")
                .SetValue(
                    data,
                    configuration.GetSection("draw_in_deployment_phase").ParseBool()
                        ?? drawInDeploymentPhase
                );

            service.Register(name, data);
            return new CardTraitDefinition(key, data, configuration) { Id = id };
        }
    }
}
