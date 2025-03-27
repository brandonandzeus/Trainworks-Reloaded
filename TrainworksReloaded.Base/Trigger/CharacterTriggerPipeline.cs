﻿using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Trigger
{
    public class CharacterTriggerPipeline
        : IDataPipeline<IRegister<CharacterTriggerData>, CharacterTriggerData>
    {
        private readonly PluginAtlas atlas;
        private readonly IRegister<LocalizationTerm> termRegister;

        public CharacterTriggerPipeline(PluginAtlas atlas, IRegister<LocalizationTerm> termRegister)
        {
            this.atlas = atlas;
            this.termRegister = termRegister;
        }

        public List<IDefinition<CharacterTriggerData>> Run(IRegister<CharacterTriggerData> service)
        {
            var processList = new List<IDefinition<CharacterTriggerData>>();
            foreach (var config in atlas.PluginDefinitions)
            {
                processList.AddRange(LoadTriggers(service, config.Key, config.Value.Configuration));
            }
            return processList;
        }

        private List<CharacterTriggerDefinition> LoadTriggers(
            IRegister<CharacterTriggerData> service,
            string key,
            IConfiguration pluginConfig
        )
        {
            var processList = new List<CharacterTriggerDefinition>();
            foreach (var child in pluginConfig.GetSection("character_triggers").GetChildren())
            {
                var data = LoadTriggerConfiguration(service, key, child);
                if (data != null)
                {
                    processList.Add(data);
                }
            }
            return processList;
        }

        private CharacterTriggerDefinition? LoadTriggerConfiguration(
            IRegister<CharacterTriggerData> service,
            string key,
            IConfiguration configuration
        )
        {
            var id = configuration.GetSection("id").ParseString();
            if (id == null)
            {
                return null;
            }
            var name = key.GetId("CTrigger", id);
            var descriptionKey = $"CharacterTriggerData_descriptionKey-{name}";
            var additionalTextOnTriggerKey = $"CharacterTriggerData_textOnTriggerKey-{name}";
            var data = new CharacterTriggerData(CharacterTriggerData.Trigger.OnDeath, null);

            //handle descriptions
            var localizationDescription = configuration
                .GetSection("descriptions")
                .ParseLocalizationTerm();
            if (localizationDescription != null)
            {
                AccessTools
                    .Field(typeof(CharacterTriggerData), "descriptionKey")
                    .SetValue(data, descriptionKey);
                localizationDescription.Key = descriptionKey;
                termRegister.Register(descriptionKey, localizationDescription);
            }

            //handle descriptions
            var localizationTrigger = configuration
                .GetSection("text_on_trigger")
                .ParseLocalizationTerm();
            if (localizationTrigger != null)
            {
                AccessTools
                    .Field(typeof(CharacterTriggerData), "additionalTextOnTriggerKey")
                    .SetValue(data, additionalTextOnTriggerKey);
                localizationTrigger.Key = additionalTextOnTriggerKey;
                termRegister.Register(additionalTextOnTriggerKey, localizationTrigger);
            }

            //handle bools
            var showAdditionalTriggerTextOnSuccessOnly = false;
            AccessTools
                .Field(typeof(CharacterTriggerData), "showAdditionalTriggerTextOnSuccessOnly")
                .SetValue(
                    data,
                    configuration.GetSection("show_text_on_trigger_success_only").ParseBool()
                        ?? showAdditionalTriggerTextOnSuccessOnly
                );

            var displayEffectHintText = false;
            AccessTools
                .Field(typeof(CharacterTriggerData), "displayEffectHintText")
                .SetValue(
                    data,
                    configuration.GetSection("display_hint_text").ParseBool()
                        ?? displayEffectHintText
                );

            var hideVisualAndIgnoreSilence = false;
            AccessTools
                .Field(typeof(CharacterTriggerData), "hideVisualAndIgnoreSilence")
                .SetValue(
                    data,
                    configuration.GetSection("hide_tooltip").ParseBool()
                        ?? hideVisualAndIgnoreSilence
                );

            var allowAdditionalTooltipsWhenVisualIsHidden = false;
            AccessTools
                .Field(typeof(CharacterTriggerData), "allowAdditionalTooltipsWhenVisualIsHidden")
                .SetValue(
                    data,
                    configuration.GetSection("allow_tooltips_when_hidden").ParseBool()
                        ?? allowAdditionalTooltipsWhenVisualIsHidden
                );

            var suppressTriggerNotification = false;
            AccessTools
                .Field(typeof(CharacterTriggerData), "suppressTriggerNotification")
                .SetValue(
                    data,
                    configuration.GetSection("suppress_notifications").ParseBool()
                        ?? suppressTriggerNotification
                );

            var triggerOnce = false;
            AccessTools
                .Field(typeof(CharacterTriggerData), "triggerOnce")
                .SetValue(
                    data,
                    configuration.GetSection("trigger_once").ParseBool() ?? triggerOnce
                );

            var onlyTriggerIfEquipped = false;
            AccessTools
                .Field(typeof(CharacterTriggerData), "onlyTriggerIfEquipped")
                .SetValue(
                    data,
                    configuration.GetSection("only_trigger_if_equipped").ParseBool()
                        ?? onlyTriggerIfEquipped
                );

            service.Register(name, data);
            return new CharacterTriggerDefinition(key, data, configuration) { Id = id };
        }
    }
}
