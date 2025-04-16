using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using HarmonyLib;
using Malee;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Core.Enum;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.CardUpgrade
{
    public class CardUpgradeMaskRegister
        : Dictionary<string, CardUpgradeMaskData>,
            IRegister<CardUpgradeMaskData>
    {
        private readonly IModLogger<CardUpgradeMaskRegister> logger;

        public CardUpgradeMaskRegister(IModLogger<CardUpgradeMaskRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, CardUpgradeMaskData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Registering Upgrade Mask {key}... ");
            Add(key, item);
        }

        public List<string> GetAllIdentifiers(RegisterIdentifierType identifierType)
        {
            return identifierType switch
            {
                RegisterIdentifierType.ReadableID => [.. this.Keys],
                RegisterIdentifierType.GUID => [.. this.Values.Select(mask => mask.name)],
                _ => [],
            };
        }


        public bool TryLookupIdentifier(string identifier, RegisterIdentifierType identifierType, [NotNullWhen(true)] out CardUpgradeMaskData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            lookup = null;
            IsModded = false;
            switch (identifierType)
            {
                case RegisterIdentifierType.ReadableID:
                    foreach (var mask in this.Values)
                    {
                        if (mask.name.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                        {
                            lookup = mask;
                            IsModded = true;
                            return true;
                        }
                    }
                    return false;
                case RegisterIdentifierType.GUID:
                    bool ret = TryGetValue(identifier, out lookup);
                    IsModded = ret;
                    return ret;
            }
            return false;
        }

    }
}
