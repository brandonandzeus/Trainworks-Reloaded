using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace TrainworksReloaded.Base.Prefab
{
    public class GameObjectRegister
        : Dictionary<string, GameObject>,
            IRegister<GameObject>,
            IResourceLocator
    {
        private readonly IModLogger<GameObjectRegister> logger;

        public GameObjectRegister(IModLogger<GameObjectRegister> logger)
        {
            this.logger = logger;
        }

        public Dictionary<Hash128, (string, GameObject)> HashToObjectMap { get; set; } = [];
        IEnumerable<object> IResourceLocator.Keys
        {
            get { return HashToObjectMap.Keys.Cast<object>(); }
        }

        public bool Locate(object key, out IList<IResourceLocation> locations)
        {
            locations = [];
            if (key is Hash128 hash && HashToObjectMap.TryGetValue(hash, out var value))
            {
                var location = new ResourceLocationBase(
                    value.Item1,
                    value.Item1,
                    typeof(GameObjectRegister).FullName,
                    []
                )
                {
                    Data = value.Item2,
                };
                locations.Add(location);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Register(string key, GameObject item)
        {
            logger.Log(LogLevel.Info, $"Register GameObject ({key})");
            HashToObjectMap.Add(Hash128.Compute(key), (key, item));
            this.Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out GameObject? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out GameObject? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = true;
            foreach (var gameobject in this.Values)
            {
                if (gameobject.name == name)
                {
                    lookup = gameobject;
                    IsModded = true;
                    return true;
                }
            }
            return false;
        }
    }
}
