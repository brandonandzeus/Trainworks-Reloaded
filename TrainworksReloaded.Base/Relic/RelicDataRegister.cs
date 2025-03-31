using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Relic
{
    public class RelicDataRegister : Dictionary<string, RelicData>, IRegister<RelicData>
    {
        private readonly IModLogger<RelicDataRegister> logger;

        public RelicDataRegister(IModLogger<RelicDataRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, RelicData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Register Relic {key}... ");
            Add(key, item);
        }

        public bool TryLookupId(string id, [NotNullWhen(true)] out RelicData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(string name, [NotNullWhen(true)] out RelicData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            IsModded = true;
            return this.TryGetValue(name, out lookup);
        }
    }
}