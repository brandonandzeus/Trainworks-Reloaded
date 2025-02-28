using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Card
{
    public class CardDataDefinition(
        string key,
        CardData data,
        IConfiguration configuration,
        bool isOverride
    ) : IDefinition<CardData>
    {
        public string Id { get; set; } = "";
        public string Key { get; set; } = key;
        public CardData Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
        public bool IsModded => !isOverride;
    }
}
