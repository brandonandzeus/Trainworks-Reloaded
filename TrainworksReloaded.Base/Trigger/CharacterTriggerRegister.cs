using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Trigger
{
    public class CharacterTriggerRegister
        : Dictionary<string, CharacterTriggerData>,
            IRegister<CharacterTriggerData>
    {
        private readonly IModLogger<CharacterTriggerRegister> logger;

        public CharacterTriggerRegister(IModLogger<CharacterTriggerRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, CharacterTriggerData item)
        {
            logger.Log(LogLevel.Info, $"Register Trigger ({key})");
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out CharacterTriggerData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out CharacterTriggerData? lookup,
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
