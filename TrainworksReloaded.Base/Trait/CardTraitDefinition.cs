using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Trait
{
    public class CardTraitDefinition(string key, CardTraitData data, IConfiguration configuration)
        : IDefinition<CardTraitData>
    {
        public string Key { get; set; } = key;
        public CardTraitData Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
    }
}
