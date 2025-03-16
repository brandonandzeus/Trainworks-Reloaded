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
    public class AtlasIconRegister(IModLogger<AtlasIconRegister> logger) : Dictionary<string, Texture2D>, IRegister<Texture2D>
    {
        private readonly IModLogger<AtlasIconRegister> logger = logger;

        public void Register(string key, Texture2D item)
        {
            logger.Log(LogLevel.Info, $"Register Texture2D ({key})");
            this.Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out Texture2D? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out Texture2D? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = true;
            foreach (var icon in this.Values)
            {
                if (icon.name == name)
                {
                    lookup = icon;
                    IsModded = true;
                    return true;
                }
            }
            return false;
        }
    }
}
