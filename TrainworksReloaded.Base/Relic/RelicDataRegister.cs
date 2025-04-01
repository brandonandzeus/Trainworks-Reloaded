using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
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

        public void Register(string key, RelicData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Register Relic {key}... ");
            var gamedata = SaveManager.Value.GetAllGameData();
            var RelicDatas =
                (List<CollectableRelicData>)
                    AccessTools.Field(typeof(AllGameData), "collectableRelicDatas").GetValue(gamedata);
            if (item is CollectableRelicData collectableRelic)
                RelicDatas.Add(collectableRelic);
            Add(key, item);
        }

        public bool TryLookupId(string id, [NotNullWhen(true)] out RelicData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(string name, [NotNullWhen(true)] out RelicData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            IsModded = true;
            return this.TryGetValue(name, out lookup);
        }
    }
}