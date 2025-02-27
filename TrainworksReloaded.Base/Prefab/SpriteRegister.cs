using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace TrainworksReloaded.Base.Prefab
{
    public class SpriteRegister : Dictionary<string, Sprite>, IRegister<Sprite>
    {
        private readonly IModLogger<SpriteRegister> logger;

        public SpriteRegister(IModLogger<SpriteRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, Sprite item)
        {
            logger.Log(LogLevel.Info, $"Register Sprite ({key})");
            this.Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out Sprite? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out Sprite? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = true;
            foreach (var sprite in this.Values)
            {
                if (sprite.name == name)
                {
                    lookup = sprite;
                    IsModded = true;
                    return true;
                }
            }
            return false;
        }
    }
}
