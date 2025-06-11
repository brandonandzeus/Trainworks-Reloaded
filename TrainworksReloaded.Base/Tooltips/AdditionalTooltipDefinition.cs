using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Tooltips
{
    public class AdditionalTooltipDefinition(string key, AdditionalTooltipData data, IConfiguration configuration)
        : IDefinition<AdditionalTooltipData>
    {
        public string Key { get; set; } = key;
        public AdditionalTooltipData Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
        public string Id { get; set; } = "";
        public bool IsModded => true;
    }
}
