using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RimLight;

namespace TrainworksReloaded.Base.Prefab
{
    public class VfxRegister : Dictionary<string, VfxAtLoc>, IRegister<VfxAtLoc>
    {
        private static VfxAtLoc Default { get; set; }

        static VfxRegister()
        {
            Default = (VfxAtLoc)FormatterServices.GetUninitializedObject(typeof(VfxAtLoc));
            AccessTools.Field(typeof(VfxAtLoc), "vfxPrefabLeft").SetValue(Default, null);
            AccessTools
                .Field(typeof(VfxAtLoc), "vfxPrefabRefLeft")
                .SetValue(Default, new AssetReferenceGameObject());
            AccessTools.Field(typeof(VfxAtLoc), "vfxPrefabRight").SetValue(Default, null);
            AccessTools
                .Field(typeof(VfxAtLoc), "vfxPrefabRefRight")
                .SetValue(Default, new AssetReferenceGameObject());
            AccessTools
                .Field(typeof(VfxAtLoc), "spawnLocation")
                .SetValue(Default, VfxAtLoc.Location.None);
            AccessTools.Field(typeof(VfxAtLoc), "facing").SetValue(Default, VfxAtLoc.Facing.None);
        }

        private readonly IModLogger<VfxRegister> logger;

        public VfxRegister(IModLogger<VfxRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, VfxAtLoc item)
        {
            logger.Log(LogLevel.Info, $"Register VFX ({key})");
            this.Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out VfxAtLoc? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            var result = this.TryGetValue(id, out lookup);
            if (result == false)
            {
                lookup = Default;
                result = true;
            }
            return result;
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out VfxAtLoc? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            var result = this.TryGetValue(name, out lookup);
            if (result == false)
            {
                lookup = Default;
                result = true;
            }
            return result;
        }
    }
}
