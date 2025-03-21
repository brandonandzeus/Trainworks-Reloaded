using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Map
{
    public record MapNodeKey
    {
        public MapNodeKey() { }

        public MapNodeKey(string runKey, string bucketKey)
        {
            RunKey = runKey;
            BucketKey = bucketKey;
        }

        public string RunKey { get; set; } = "";
        public string BucketKey { get; set; } = "";
    }

    public class MapNodeDelegator
    {
        public Dictionary<MapNodeKey, List<MapNodeData>> MapBucketToData = [];
    }
}
