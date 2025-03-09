using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Trigger
{
    public class CardTriggerEffectRegister
        : Dictionary<string, CardTriggerEffectData>,
            IRegister<CardTriggerEffectData>
    {
        private readonly IModLogger<CardTriggerEffectRegister> logger;

        public CardTriggerEffectRegister(IModLogger<CardTriggerEffectRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, CardTriggerEffectData item)
        {
            logger.Log(LogLevel.Info, $"Register Card Trigger Effect ({key})");
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out CardTriggerEffectData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out CardTriggerEffectData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = true;
            foreach (var trigger in this.Values)
            {
                if (trigger.GetDebugName() == name)
                {
                    lookup = trigger;
                    IsModded = true;
                    return true;
                }
            }
            return false;
        }
    }
}
