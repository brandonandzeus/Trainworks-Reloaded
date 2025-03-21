using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Core.Interfaces;
using static RimLight;

namespace TrainworksReloaded.Base.Map
{
    public class MapNodeRegister : Dictionary<string, MapNodeData>, IRegister<MapNodeData>
    {
        private readonly IModLogger<MapNodeRegister> logger;
        private readonly Lazy<SaveManager> SaveManager;

        public MapNodeRegister(GameDataClient client, IModLogger<MapNodeRegister> logger)
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

        public void Register(string key, MapNodeData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Register Map Node {key}... ");
            var gamedata = SaveManager.Value.GetAllGameData();
            var MapNodeDatas =
                (List<MapNodeData>)
                    AccessTools.Field(typeof(AllGameData), "mapNodeDatas").GetValue(gamedata);
            MapNodeDatas.Add(item);
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out MapNodeData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            var data = SaveManager.Value.GetAllGameData().FindMapNodeData(id);
            if (data != null)
            {
                lookup = data;
                IsModded = this.ContainsKey(data.name);
                return true;
            }
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out MapNodeData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            var data = SaveManager.Value.GetAllGameData();
            var mapData =
                (List<MapNodeData>)
                    AccessTools.Field(typeof(AllGameData), "mapNodeDatas").GetValue(data);
            if (data != null)
            {
                foreach (var map in mapData)
                {
                    if (map.name == name)
                    {
                        lookup = map;
                        IsModded = this.ContainsKey(data.name);
                        return true;
                    }
                }
            }
            return this.TryGetValue(name, out lookup);
        }
    }
}
