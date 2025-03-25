using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Core.Interfaces;
using static CharacterTriggerData;
using static RimLight;

namespace TrainworksReloaded.Base.Enum
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

        public void Register(string key, CardTriggerType item)
        {
            logger.Log(LogLevel.Info, $"Register Card Trigger Enum ({key})");

            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out CardTriggerType lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out CardTriggerType lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(name, out lookup);
        }
    }
}
