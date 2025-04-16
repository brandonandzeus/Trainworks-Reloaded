using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TrainworksReloaded.Core.Enum;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Relic
{
    public class RelicDataRegister : Dictionary<string, RelicData>, IRegister<RelicData>
    {
        private readonly IModLogger<RelicDataRegister> logger;
        private readonly Lazy<SaveManager> SaveManager;

        public RelicDataRegister(GameDataClient client, IModLogger<RelicDataRegister> logger)
        {
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

        public List<string> GetAllIdentifiers(RegisterIdentifierType identifierType)
        {
            return identifierType switch
            {
                RegisterIdentifierType.ReadableID => [.. this.Keys],
                RegisterIdentifierType.GUID => [.. this.Keys],
                _ => [],
            };
        }

        public void Register(string key, RelicData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Register Relic {key}... ");
            if (item is CollectableRelicData collectableRelic)
            {
                var gamedata = SaveManager.Value.GetAllGameData();
                var RelicDatas =
                    (List<CollectableRelicData>)
                        AccessTools.Field(typeof(AllGameData), "collectableRelicDatas").GetValue(gamedata);
                RelicDatas.Add(collectableRelic);
            }
            Add(key, item);
        }

        public bool TryLookupIdentifier(string identifier, RegisterIdentifierType identifierType, [NotNullWhen(true)] out RelicData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            lookup = null;
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