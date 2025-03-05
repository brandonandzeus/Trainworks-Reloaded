using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Base.Trait;
using TrainworksReloaded.Core.Interfaces;
using static RimLight;

namespace TrainworksReloaded.Base.Room
{
    public class RoomModifierRegister
        : Dictionary<string, RoomModifierData>,
            IRegister<RoomModifierData>
    {
        private readonly IModLogger<RoomModifierRegister> logger;

        public RoomModifierRegister(IModLogger<RoomModifierRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, RoomModifierData item)
        {
            logger.Log(LogLevel.Info, $"Register Trait ({key})");
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out RoomModifierData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out RoomModifierData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(name, out lookup);
        }
    }
}
