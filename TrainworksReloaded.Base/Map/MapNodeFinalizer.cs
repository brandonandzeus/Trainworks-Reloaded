using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Room;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Reward
{
    public class MapNodeFinalizer : IDataFinalizer
    {
        private readonly IModLogger<MapNodeFinalizer> logger;
        private readonly ICache<IDefinition<MapNodeData>> cache;
        private readonly IRegister<Sprite> spriteRegister;
        private readonly IRegister<GameObject> gameObjectRegister;
        private readonly IRegister<MapNodeData> mapNodeRegister;

        public MapNodeFinalizer(
            IModLogger<MapNodeFinalizer> logger,
            ICache<IDefinition<MapNodeData>> cache,
            IRegister<Sprite> spriteRegister,
            IRegister<GameObject> gameObjectRegister,
            IRegister<MapNodeData> mapNodeRegister
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.spriteRegister = spriteRegister;
            this.gameObjectRegister = gameObjectRegister;
            this.mapNodeRegister = mapNodeRegister;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeMapData(definition);
            }
            cache.Clear();
        }

        /// <summary>
        /// Finalize Card Definitions
        /// Handles Data to avoid lookup looks for names and ids
        /// </summary>
        /// <param name="definition"></param>
        private void FinalizeMapData(IDefinition<MapNodeData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(
                Core.Interfaces.LogLevel.Info,
                $"Finalizing Map Node Data {definition.Id.ToId(key, TemplateConstants.MapNode)}... "
            );

            var sprite = configuration.GetSection("map_icon").ParseString();
            if (
                sprite != null
                && spriteRegister.TryLookupId(
                    sprite.ToId(key, TemplateConstants.Sprite),
                    out var spriteLookup,
                    out var _
                )
            )
            {
                AccessTools.Field(typeof(MapNodeData), "mapIcon").SetValue(data, spriteLookup);
            }

            var sprite2 = configuration.GetSection("minimap_icon").ParseString();
            if (
                sprite2 != null
                && spriteRegister.TryLookupId(
                    sprite2.ToId(key, TemplateConstants.Sprite),
                    out var spriteLookup2,
                    out var _
                )
            )
            {
                AccessTools.Field(typeof(MapNodeData), "minimapIcon").SetValue(data, spriteLookup2);
            }

            var gameobject = configuration.GetSection("prefab").ParseString();
            if (
                gameobject != null
                && gameObjectRegister.TryLookupId(
                    gameobject.ToId(key, TemplateConstants.GameObject),
                    out var objectLookup,
                    out var _
                )
            )
            {
                var icon = objectLookup.GetComponent<MapNodeIcon>();
                AccessTools.Field(typeof(MapNodeData), "mapIconPrefab").SetValue(data, icon);
            }

            var mapNodes = new List<MapNodeData>();
            foreach (var config in configuration.GetSection("ignore_if_present").GetChildren())
            {
                var mapNode = config.ParseString();
                if (
                    mapNode != null
                    && mapNodeRegister.TryLookupId(
                        mapNode.ToId(key, TemplateConstants.MapNode),
                        out var mapLookup,
                        out var _
                    )
                )
                {
                    mapNodes.Add(mapLookup);
                }
            }
            AccessTools.Field(typeof(MapNodeData), "ignoreIfNodesPresent").SetValue(data, mapNodes);
        }
    }
}
