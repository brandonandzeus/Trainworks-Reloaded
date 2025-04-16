using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Base.Trigger;
using TrainworksReloaded.Core.Enum;
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

        public List<string> GetAllIdentifiers(RegisterIdentifierType identifierType)
        {
            return identifierType switch
            {
                RegisterIdentifierType.ReadableID => [.. this.Keys],
                RegisterIdentifierType.GUID => [.. this.Keys],
                _ => []
            };
        }

        public bool TryLookupIdentifier(string identifier, RegisterIdentifierType identifierType, [NotNullWhen(true)] out CharacterTriggerData.Trigger lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            lookup = default;
            IsModded = true;
            switch (identifierType)
            {
                case RegisterIdentifierType.ReadableID:
                    return this.TryGetValue(identifier, out lookup);
                case RegisterIdentifierType.GUID:
                    return this.TryGetValue(identifier, out lookup);
                default:
                    return false;
            }
        }
    }
}
