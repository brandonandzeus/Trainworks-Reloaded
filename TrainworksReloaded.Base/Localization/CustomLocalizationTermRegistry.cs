﻿using I2.Loc;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Localization
{
    public class CustomLocalizationTermRegistry : Dictionary<string, LocalizationTerm>, ICustomRegister<LocalizationTerm>
    {
        private readonly IModLogger<CustomLocalizationTermRegistry> logger;

        public CustomLocalizationTermRegistry(IModLogger<CustomLocalizationTermRegistry> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, LocalizationTerm item)
        {
            this.Add(key, item);
        }

        public void LoadData()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Key,Type,Desc,Group,Descriptions,English [en-US],French [fr-FR],German [de-DE],Russian,Portuguese (Brazil),Chinese,Spanish,Chinese (Traditional),Korean,Japanese");
            foreach (var term in this.Values)
            {
                logger.Log(LogLevel.Info, $"Adding Term ({term.Key}) -- ({term.English})");
                builder.AppendLine($"{term.Key},{term.Type},{term.Desc},{term.Group},{term.Descriptions},{term.English},{term.French},{term.German},{term.Russian},{term.Portuguese},{term.Chinese},{term.Spanish},{term.ChineseTraditional},{term.Korean},{term.Japanese}");
            }

            LocalizationManager.InitializeIfNeeded();
            List<string> categories = LocalizationManager.Sources[0].GetCategories(true);
            foreach (string Category in categories)
                LocalizationManager.Sources[0].Import_CSV(Category, builder.ToString(), eSpreadsheetUpdateMode.Merge, ',');
        }
    }
}
