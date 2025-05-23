﻿using HarmonyLib;
using Malee;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Core.Enum;
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
            StatusEffectManager.StatusIdToLocalizationExpression.Add(item.GetStatusId(), "StatusEffect_" + item.GetStatusId());
            Add(key, item);
        }

        public List<string> GetAllIdentifiers(RegisterIdentifierType identifierType)
        {
            return identifierType switch
            {
                RegisterIdentifierType.ReadableID => [.. this.Values.Select(effect => effect.GetStatusId())],
                RegisterIdentifierType.GUID => [.. this.Keys],
                _ => [],
            };
        }


        public bool TryLookupIdentifier(string identifier, RegisterIdentifierType identifierType, [NotNullWhen(true)] out StatusEffectData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            lookup = null;
            IsModded = true;
            switch (identifierType)
            {
                case RegisterIdentifierType.ReadableID:
                    foreach (var effect in this.Values)
                    {
                        if (effect.GetStatusId() == identifier)
                        {
                            lookup = effect;
                            return true;
                        }
                    }
                    return false;
                case RegisterIdentifierType.GUID:
                    return this.TryGetValue(identifier, out lookup);
                default:
                    return false;
            }
        }
    }
}
