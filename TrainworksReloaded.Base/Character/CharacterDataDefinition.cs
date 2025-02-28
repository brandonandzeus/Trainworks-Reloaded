using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Character
{
    public class CharacterDataDefinition(
        string key,
        CharacterData data,
        IConfiguration configuration,
        bool isOverride
    ) : IDefinition<CharacterData>
    {
        public string Key { get; set; } = key;
        public CharacterData Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
        public string Id { get; set; } = "";
        public bool IsModded { get; set; } = !isOverride;
    }
}
