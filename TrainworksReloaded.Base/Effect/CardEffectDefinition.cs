using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Effect
{
    public class CardEffectDefinition(string key, CardEffectData data, IConfiguration configuration)
        : IDefinition<CardEffectData>
    {
        public string Key { get; set; } = key;
        public CardEffectData Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
        public string Id { get; set; } = "";
        public bool IsModded { get; set; } = true;
    }
}
