using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Room
{
    public class RoomModifierPipeline : IDataPipeline<IRegister<RoomModifierData>, RoomModifierData>
    {
        private readonly PluginAtlas atlas;
        private readonly IModLogger<RoomModifierPipeline> logger;
        private readonly IRegister<LocalizationTerm> termRegister;

        public RoomModifierPipeline(
            PluginAtlas atlas,
            IModLogger<RoomModifierPipeline> logger,
            IRegister<LocalizationTerm> termRegister
        )
        {
            this.atlas = atlas;
            this.logger = logger;
            this.termRegister = termRegister;
        }

        public List<IDefinition<RoomModifierData>> Run(IRegister<RoomModifierData> service)
        {
            var processList = new List<IDefinition<RoomModifierData>>();
            foreach (var config in atlas.PluginDefinitions)
            {
                processList.AddRange(
                    LoadRoomModifiers(service, config.Key, config.Value.Configuration)
                );
            }
            return processList;
        }

        private List<RoomModifierDefinition> LoadRoomModifiers(
            IRegister<RoomModifierData> service,
            string key,
            IConfiguration pluginConfig
        )
        {
            var processList = new List<RoomModifierDefinition>();
            foreach (var child in pluginConfig.GetSection("room_modifiers").GetChildren())
            {
                var data = LoadRoomModifier(service, key, child);
                if (data != null)
                {
                    processList.Add(data);
                }
            }
            return processList;
        }

        private RoomModifierDefinition? LoadRoomModifier(
            IRegister<RoomModifierData> service,
            string key,
            IConfiguration configuration
        )
        {
            var id = configuration.GetSection("id").ParseString();
            if (id == null)
            {
                return null;
            }
            var name = key.GetId("RoomModifier", id);
            var namekey = $"RoomModifierData_nameKey-{name}";
            var descriptionKey = $"RoomModifierData_descriptionKey-{name}";
            var descriptionKeyInPlay = $"RoomModifierData_descriptionKeyInPlay-{name}";
            var extraTooltipTitleKey = $"RoomModifierData_extraTooltipTitleKey-{name}";
            var extraTooltipBodyKey = $"RoomModifierData_extraTooltipBodyKey-{name}";
            var data = new RoomModifierData();

            //handle names
            var nameClass =
                configuration.GetSection("name").ParseString() ?? "RoomStateBuffModifier";
            AccessTools
                .Field(typeof(RoomModifierData), "roomStateModifierClassName")
                .SetValue(data, nameClass);
            var nameTerm = configuration.GetSection("names").ParseLocalizationTerm();
            if (nameTerm != null)
            {
                nameTerm.Key = nameClass;
                termRegister.Register(nameClass, nameTerm);
            }

            //handle descriptions
            var descriptionKeyTerm = configuration
                .GetSection("descriptions")
                .ParseLocalizationTerm();
            if (descriptionKeyTerm != null)
            {
                AccessTools
                    .Field(typeof(RoomModifierData), "descriptionKey")
                    .SetValue(data, descriptionKey);
                descriptionKeyTerm.Key = descriptionKey;
                termRegister.Register(descriptionKey, descriptionKeyTerm);
            }

            //handle descriptions
            var descriptionKeyInPlayTerm = configuration
                .GetSection("play_descriptions")
                .ParseLocalizationTerm();
            if (descriptionKeyInPlayTerm != null)
            {
                AccessTools
                    .Field(typeof(RoomModifierData), "descriptionKeyInPlay")
                    .SetValue(data, descriptionKeyInPlay);
                descriptionKeyInPlayTerm.Key = descriptionKeyInPlay;
                termRegister.Register(descriptionKeyInPlay, descriptionKeyInPlayTerm);
            }

            //handle descriptions
            var extraTooltipTitleKeyTerm = configuration
                .GetSection("extra_title_tooltips")
                .ParseLocalizationTerm();
            if (extraTooltipTitleKeyTerm != null)
            {
                AccessTools
                    .Field(typeof(RoomModifierData), "extraTooltipTitleKey")
                    .SetValue(data, extraTooltipTitleKey);
                extraTooltipTitleKeyTerm.Key = extraTooltipTitleKey;
                termRegister.Register(extraTooltipTitleKey, extraTooltipTitleKeyTerm);
            }

            //handle descriptions
            var extraTooltipBodyKeyTerm = configuration
                .GetSection("extra_body_tooltips")
                .ParseLocalizationTerm();
            if (extraTooltipBodyKeyTerm != null)
            {
                AccessTools
                    .Field(typeof(RoomModifierData), "extraTooltipBodyKey")
                    .SetValue(data, extraTooltipBodyKey);
                extraTooltipBodyKeyTerm.Key = extraTooltipBodyKey;
                termRegister.Register(extraTooltipBodyKey, extraTooltipBodyKeyTerm);
            }

            //string
            var roomStateModifierClassName = "";
            AccessTools
                .Field(typeof(RoomModifierData), "roomStateModifierClassName")
                .SetValue(
                    data,
                    configuration.GetSection("name").ParseString() ?? roomStateModifierClassName
                );

            var paramSubtype = "";
            AccessTools
                .Field(typeof(RoomModifierData), "paramSubtype")
                .SetValue(
                    data,
                    configuration.GetSection("param_subtype").ParseString() ?? paramSubtype
                );

            //bool
            var useTitleForCardDescription = false;
            AccessTools
                .Field(typeof(RoomModifierData), "useTitleForCardDescription")
                .SetValue(
                    data,
                    configuration.GetSection("use_name_as_description").ParseBool()
                        ?? useTitleForCardDescription
                );

            //int
            var paramInt = 0;
            AccessTools
                .Field(typeof(RoomModifierData), "paramInt")
                .SetValue(data, configuration.GetSection("param_int").ParseInt() ?? paramInt);
            var paramInt2 = 0;
            AccessTools
                .Field(typeof(RoomModifierData), "paramInt2")
                .SetValue(data, configuration.GetSection("param_int_2").ParseInt() ?? paramInt2);

            //trigger
            var paramTrigger = CharacterTriggerData.Trigger.OnDeath;
            AccessTools
                .Field(typeof(RoomModifierData), "paramTrigger")
                .SetValue(
                    data,
                    configuration.GetSection("param_trigger").ParseTrigger() ?? paramTrigger
                );

            var additionalTooltips = new List<AdditionalTooltipData>();
            int configCount = 0;
            foreach (var config in configuration.GetSection("additional_tooltips").GetChildren())
            {
                var tooltipData = new AdditionalTooltipData();

                var titleKey = $"RoomModifierDataTooltip_titleKey_{configCount}-{name}";
                var descriptionTKey = $"RoomModifierDataTooltip_titleKey_{configCount}-{name}";

                var titleKeyTerm = configuration.GetSection("titles").ParseLocalizationTerm();
                if (titleKeyTerm != null)
                {
                    tooltipData.titleKey = titleKey;
                    titleKeyTerm.Key = titleKey;
                    termRegister.Register(titleKey, titleKeyTerm);
                }
                var descriptionTKeyTerm = configuration
                    .GetSection("descriptions")
                    .ParseLocalizationTerm();
                if (descriptionTKeyTerm != null)
                {
                    tooltipData.descriptionKey = descriptionTKey;
                    descriptionTKeyTerm.Key = descriptionTKey;
                    termRegister.Register(descriptionTKey, descriptionTKeyTerm);
                }

                tooltipData.style =
                    configuration.GetSection("param_trigger").ParseTooltipDesignType()
                    ?? TooltipDesigner.TooltipDesignType.Default;
                tooltipData.isStatusTooltip =
                    configuration.GetSection("is_status").ParseBool() ?? false;
                tooltipData.hideInTrainRoomUI =
                    configuration.GetSection("hide_in_train_room").ParseBool() ?? false;
                tooltipData.allowSecondaryPlacement =
                    configuration.GetSection("allow_secondary").ParseBool() ?? false;
                tooltipData.isTriggerTooltip =
                    configuration.GetSection("hide_in_train_room").ParseBool() ?? false;

                configCount++;
                additionalTooltips.Add(tooltipData);
            }
            AccessTools
                .Field(typeof(RoomModifierData), "additionalTooltips")
                .SetValue(data, additionalTooltips.ToArray());

            var paramStatusEffects = configuration
                .GetSection("param_status_effects")
                .GetChildren()
                .Select(xs => new StatusEffectStackData()
                {
                    statusId = xs.GetSection("status").ParseString() ?? "",
                    count = xs.GetSection("count").ParseInt() ?? 0,
                })
                .ToList();
            AccessTools
                .Field(typeof(RoomModifierData), "paramStatusEffects")
                .SetValue(data, paramStatusEffects.ToArray());

            service.Register(name, data);
            return new RoomModifierDefinition(key, data, configuration) { Id = id };
        }
    }
}
