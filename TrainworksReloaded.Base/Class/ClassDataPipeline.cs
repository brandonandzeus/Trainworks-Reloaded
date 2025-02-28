using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using TrainworksReloaded.Base.Class;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Class
{
    public class ClassDataPipeline : IDataPipeline<IRegister<ClassData>, ClassData>
    {
        private readonly PluginAtlas atlas;
        private readonly IModLogger<ClassDataPipeline> logger;
        private readonly IRegister<LocalizationTerm> termRegister;

        public ClassDataPipeline(
            PluginAtlas atlas,
            IModLogger<ClassDataPipeline> logger,
            IRegister<LocalizationTerm> termRegister
        )
        {
            this.atlas = atlas;
            this.logger = logger;
            this.termRegister = termRegister;
        }

        public List<IDefinition<ClassData>> Run(IRegister<ClassData> service)
        {
            // We load all cards and then finalize them to avoid dependency loops
            var processList = new List<IDefinition<ClassData>>();
            foreach (var config in atlas.PluginDefinitions)
            {
                processList.AddRange(LoadClasses(service, config.Key, config.Value.Configuration));
            }
            return processList;
        }

        /// <summary>
        /// Loads the Card Definitions in
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <param name="pluginConfig"></param>
        /// <returns></returns>
        private List<ClassDataDefinition> LoadClasses(
            IRegister<ClassData> service,
            string key,
            IConfiguration pluginConfig
        )
        {
            var processList = new List<ClassDataDefinition>();
            foreach (var child in pluginConfig.GetSection("classes").GetChildren())
            {
                var data = LoadClassConfiguration(service, key, child);
                if (data != null)
                {
                    processList.Add(data);
                }
            }
            return processList;
        }

        private ClassDataDefinition? LoadClassConfiguration(
            IRegister<ClassData> service,
            string key,
            IConfiguration configuration
        )
        {
            var id = configuration.GetSection("id").ParseString();
            if (id == null)
            {
                return null;
            }

            var name = key.GetId("Class", id);
            var titleKey = $"ClassData_titleKey-{name}";
            var descriptionKey = $"ClassData_descriptionKey-{name}";
            var subclassDescriptionKey = $"ClassData_subclassDescriptionKey-{name}";
            var checkOverride = configuration.GetSection("override").ParseBool() ?? false;

            string guid;
            if (checkOverride && service.TryLookupName(id, out ClassData? data, out var _))
            {
                logger.Log(Core.Interfaces.LogLevel.Info, $"Overriding Class {id}... ");
                titleKey = data.GetTitleKey();
                descriptionKey = data.GetDescriptionKey();
                subclassDescriptionKey = data.GetSubclassDescriptionKey();
                guid = data.GetID();
            }
            else
            {
                data = ScriptableObject.CreateInstance<ClassData>();
                data.name = name;
                guid = Guid.NewGuid().ToString();
            }

            //handle id
            AccessTools.Field(typeof(ClassData), "id").SetValue(data, guid);

            //handle titles
            var localizationTitleTerm = configuration.GetSection("titles").ParseLocalizationTerm();
            if (localizationTitleTerm != null)
            {
                AccessTools.Field(typeof(ClassData), "titleLoc").SetValue(data, titleKey);
                localizationTitleTerm.Key = titleKey;
                termRegister.Register(titleKey, localizationTitleTerm);
            }

            //handle desc
            var localizationDesriptionTerm = configuration
                .GetSection("descriptions")
                .ParseLocalizationTerm();
            if (localizationDesriptionTerm != null)
            {
                AccessTools
                    .Field(typeof(ClassData), "descriptionLoc")
                    .SetValue(data, descriptionKey);
                localizationDesriptionTerm.Key = descriptionKey;
                termRegister.Register(descriptionKey, localizationDesriptionTerm);
            }

            //handle desc
            var localizatioSubclassDescriptionTerm = configuration
                .GetSection("subclass_descriptions")
                .ParseLocalizationTerm();
            if (localizatioSubclassDescriptionTerm != null)
            {
                AccessTools
                    .Field(typeof(ClassData), "subclassDescriptionLoc")
                    .SetValue(data, subclassDescriptionKey);
                localizatioSubclassDescriptionTerm.Key = subclassDescriptionKey;
                termRegister.Register(subclassDescriptionKey, localizatioSubclassDescriptionTerm);
            }

            //handel bool
            var corruptionEnabled =
                checkOverride
                && (bool)AccessTools.Field(typeof(ClassData), "corruptionEnabled").GetValue(data);
            AccessTools
                .Field(typeof(ClassData), "corruptionEnabled")
                .SetValue(
                    data,
                    configuration.GetSection("corruption_enabled").ParseBool() ?? corruptionEnabled
                );

            var isCrew =
                checkOverride
                && (bool)AccessTools.Field(typeof(ClassData), "isCrew").GetValue(data);
            AccessTools
                .Field(typeof(ClassData), "isCrew")
                .SetValue(data, configuration.GetSection("is_crew").ParseBool() ?? isCrew);

            //string
            var clanSelectSfxCue = checkOverride
                ? (string)AccessTools.Field(typeof(ClassData), "clanSelectSfxCue").GetValue(data)
                : "";
            AccessTools
                .Field(typeof(ClassData), "clanSelectSfxCue")
                .SetValue(
                    data,
                    configuration.GetSection("sfx_cue").ParseString() ?? clanSelectSfxCue
                );

            //handle color
            var uiColor = checkOverride
                ? (Color)AccessTools.Field(typeof(ClassData), "uiColor").GetValue(data)
                : Color.white;
            AccessTools
                .Field(typeof(ClassData), "uiColor")
                .SetValue(data, configuration.GetSection("ui_color").ParseColor() ?? uiColor);

            //handle color
            var uiColorDark = checkOverride
                ? (Color)AccessTools.Field(typeof(ClassData), "uiColorDark").GetValue(data)
                : Color.black;
            AccessTools
                .Field(typeof(ClassData), "uiColorDark")
                .SetValue(
                    data,
                    configuration.GetSection("ui_color_dark").ParseColor() ?? uiColorDark
                );

            //parse gradient
            var gradient = checkOverride
                ? (Gradient)AccessTools.Field(typeof(ClassData), "uiColorGradient").GetValue(data)
                : new Gradient();
            var gradientKeysConfig = configuration.GetSection("ui_gradient").GetChildren();
            List<GradientColorKey> gradientColorKeys = [];
            foreach (var gradientKeyConfig in gradientKeysConfig)
            {
                var time = gradientKeyConfig.GetSection("time").ParseFloat();
                var color = gradientKeyConfig.GetSection("color").ParseColor() ?? Color.white;
                if (time == null)
                {
                    continue;
                }
                gradientColorKeys.Add(new GradientColorKey(color, (float)time));
            }
            gradient.SetKeys(gradientColorKeys.ToArray(), []);
            AccessTools.Field(typeof(ClassData), "uiColorGradient").SetValue(data, gradient);

            //List<String>
            var classUnlockPreviewTexts =
                (List<string>)
                    AccessTools.Field(typeof(ClassData), "classUnlockPreviewTexts").GetValue(data);
            var previews = configuration.GetSection("class_preview_texts").GetChildren();
            if (previews.Count() > 0)
            {
                classUnlockPreviewTexts.Clear();
            }
            int i = 0;
            foreach (var preview in previews)
            {
                var val = preview.ParseLocalizationTerm();
                if (val == null)
                {
                    continue;
                }
                val.Key = $"ClassData_previewKey-{name}-{i}";
                i++;
                termRegister.Register(val.Key, val);
                classUnlockPreviewTexts.Add(val.Key);
            }

            //card style
            var cardStyle = checkOverride
                ? (ClassCardStyle)AccessTools.Field(typeof(ClassData), "cardStyle").GetValue(data)
                : ClassCardStyle.None;
            AccessTools
                .Field(typeof(ClassData), "cardStyle")
                .SetValue(
                    data,
                    configuration.GetSection("card_style").ParseCardStyle() ?? cardStyle
                );

            //register before filling in data using
            if (!checkOverride)
                service.Register(name, data);

            return new ClassDataDefinition(key, data, configuration, checkOverride);
        }
    }
}
