using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Enum
{
    public class CharacterTriggerTypeDefinition(
        string key,
        CharacterTriggerData.Trigger data,
        IConfiguration configuration
    ) : IDefinition<CharacterTriggerData.Trigger>
    {
        public string Key { get; set; } = key;
        public CharacterTriggerData.Trigger Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
        public string Id { get; set; } = "";
        public bool IsModded => true;
    }
}
