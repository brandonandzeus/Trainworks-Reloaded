using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Prefab
{
    public class VfxDefinition(string key, VfxAtLoc data, IConfiguration configuration)
        : IDefinition<VfxAtLoc>
    {
        public string Key { get; set; } = key;
        public VfxAtLoc Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
        public string Id { get; set; } = "";
        public bool IsModded { get; set; }
    }
}
