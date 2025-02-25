using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace TrainworksReloaded.Base.Prefab
{
    public class GameobjectRegister : Dictionary<string, GameObject>, IRegister<GameObject>, IResourceLocator
    {
        public Dictionary<Hash128, (string,GameObject)> HashToObjectMap { get; set; } = new Dictionary<Hash128, (string, GameObject)>();
        IEnumerable<object> IResourceLocator.Keys
        {
            get
            {
                return HashToObjectMap.Keys.Cast<object>();
            }
        }

        public bool Locate(object key, out IList<IResourceLocation> locations)
        {
            locations = [];
            if (key is Hash128 hash && HashToObjectMap.TryGetValue(hash, out var value))
            {
                var location = new ResourceLocationBase(value.Item1, value.Item1, typeof(GameobjectRegister).FullName, [])
                {
                    Data = value.Item2
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
            HashToObjectMap.Add(Hash128.Compute(key), (key, item));
            this.Add(key, item);
        }

        public bool TryLookupId(string id, [NotNullWhen(true)] out GameObject? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(string name, [NotNullWhen(true)] out GameObject? lookup, [NotNullWhen(true)] out bool? IsModded)
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
