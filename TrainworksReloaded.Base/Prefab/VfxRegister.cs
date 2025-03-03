using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using static RimLight;

namespace TrainworksReloaded.Base.Prefab
{
    public class VfxRegister : Dictionary<string, VfxAtLoc>, IRegister<VfxAtLoc>
    {
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
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out VfxAtLoc? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(name, out lookup);
        }
    }
}
