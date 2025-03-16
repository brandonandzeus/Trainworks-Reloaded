using HarmonyLib;
using Malee;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.StatusEffects
{
    public class StatusEffectDataRegister : Dictionary<string, StatusEffectData>, IRegister<StatusEffectData>
    {
        private readonly IModLogger<StatusEffectDataRegister> logger;

        public StatusEffectDataRegister(IModLogger<StatusEffectDataRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, StatusEffectData item)
        {
            logger.Log(LogLevel.Info, $"Register Status Effect ({key})");
            StatusEffectManager.Instance.GetAllStatusEffectsData().GetStatusEffectData().Add(item);
            var baseKey = "StatusEffect_" + item.GetStatusId();
            StatusEffectManager.StatusIdToLocalizationExpression.Add(item.GetStatusId(), baseKey);
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out StatusEffectData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out StatusEffectData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = true;
            foreach (var effect in this.Values)
            {
                if (effect.GetStatusId() == name)
                {
                    lookup = effect;
                    IsModded = true;
                    return true;
                }
            }
            return false;
        }
    }
}
