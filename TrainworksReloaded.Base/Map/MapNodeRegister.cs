using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Map
{
    public class MapNodeRegister : Dictionary<string, MapNodeData>, IRegister<MapNodeData>
    {
        private readonly IModLogger<MapNodeRegister> logger;

        public MapNodeRegister(GameDataClient client, IModLogger<MapNodeRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, MapNodeData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Register Map Node {key}... ");
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out MapNodeData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out MapNodeData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(name, out lookup);
        }
    }
}
