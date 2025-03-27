using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Base.Trigger;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Enums
{
    public class CharacterTriggerTypeRegister
        : Dictionary<string, CharacterTriggerData.Trigger>,
            IRegister<CharacterTriggerData.Trigger>
    {
        private readonly IModLogger<CharacterTriggerRegister> logger;

        public CharacterTriggerTypeRegister(IModLogger<CharacterTriggerRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, CharacterTriggerData.Trigger item)
        {
            logger.Log(LogLevel.Info, $"Register Character Trigger Enum ({key})");
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out CharacterTriggerData.Trigger lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out CharacterTriggerData.Trigger lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(name, out lookup);
        }
    }
}
