using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Character
{
    public class CharacterChatterDefinition(
        string key,
        CharacterChatterData data,
        IConfiguration configuration
    ) : IDefinition<CharacterChatterData>
    {
        public string Key { get; set; } = key;
        public CharacterChatterData Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
        public string Id { get; set; } = "";
        public bool IsModded => true;
    }
}
