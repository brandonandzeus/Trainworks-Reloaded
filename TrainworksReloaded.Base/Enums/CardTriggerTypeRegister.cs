using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Core.Enum;
using TrainworksReloaded.Core.Interfaces;
using static CharacterTriggerData;
using static RimLight;

namespace TrainworksReloaded.Base.Enums
{
    public class CardTriggerTypeRegister
        : Dictionary<string, CardTriggerType>,
            IRegister<CardTriggerType>
    {
        private readonly IModLogger<CardTriggerTypeRegister> logger;

        public CardTriggerTypeRegister(IModLogger<CardTriggerTypeRegister> logger)
        {
            this.logger = logger;
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

        public void Register(string key, CardTriggerType item)
        {
            logger.Log(LogLevel.Info, $"Register Card Trigger Enum ({key})");
            Add(key, item);
        }

        public bool TryLookupIdentifier(string identifier, RegisterIdentifierType identifierType, [NotNullWhen(true)] out CardTriggerType lookup, [NotNullWhen(true)] out bool? IsModded)
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
