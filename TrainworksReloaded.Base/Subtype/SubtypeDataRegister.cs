using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using Malee;
using TrainworksReloaded.Core.Enum;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Subtype
{
    public class SubtypeDataRegister : Dictionary<string, SubtypeData>, IRegister<SubtypeData>
    {
        private readonly IModLogger<SubtypeDataRegister> logger;
        private readonly Lazy<SaveManager> SaveManager;

        public SubtypeDataRegister(GameDataClient client, IModLogger<SubtypeDataRegister> logger)
        {
            // TODO update this to the new method of getting SaveManager.
            SaveManager = new Lazy<SaveManager>(() =>
            {
                if (client.TryGetValue(typeof(SaveManager).Name, out var details))
                {
                    return (SaveManager)details.Provider;
                }
                else
                {
                    return new SaveManager();
                }
            });
            this.logger = logger;
        }

        public void Register(string key, SubtypeData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Register Subtype {key}... ");
            Add(key, item);

            //var manager = SaveManager.Value.GetAllGameData().GetBalanceData().GetSubtypesData();
            //manager.AllData.Add(item);
            SubtypeManager.AllData.Add(item);
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

        public bool TryLookupIdentifier(string identifier, RegisterIdentifierType identifierType, [NotNullWhen(true)] out SubtypeData? lookup, [NotNullWhen(true)] out bool? IsModded)
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
