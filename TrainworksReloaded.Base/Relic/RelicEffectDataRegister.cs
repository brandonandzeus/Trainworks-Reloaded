using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Relic
{
    public class RelicEffectDataRegister : Dictionary<string, RelicEffectData>, IRegister<RelicEffectData>
    {
        private readonly IModLogger<RelicEffectDataRegister> logger;

        public RelicEffectDataRegister(IModLogger<RelicEffectDataRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, RelicEffectData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Register RelicEffect {key}... ");
            Add(key, item);
        }

        public bool TryLookupId(string id, [NotNullWhen(true)] out RelicEffectData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(string name, [NotNullWhen(true)] out RelicEffectData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            IsModded = true;
            return this.TryGetValue(name, out lookup);
        }
    }
} 