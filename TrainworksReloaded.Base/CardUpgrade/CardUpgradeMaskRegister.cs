using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using Malee;
using TrainworksReloaded.Base.Card;
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

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out CardUpgradeMaskData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            bool ret = TryGetValue(id, out lookup);
            IsModded = ret;
            return ret;
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out CardUpgradeMaskData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = false;
            foreach (var mask in this.Values)
            {
                if (mask.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = mask;
                    IsModded = true;
                    return true;
                }
            }
            return false;
        }
    }
}
