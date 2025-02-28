using Microsoft.Extensions.Configuration;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Class
{
    public class ClassDataDefinition(
        string key,
        ClassData data,
        IConfiguration configuration,
        bool isOverride
    ) : IDefinition<ClassData>
    {
        public string Key { get; set; } = key;
        public ClassData Data { get; set; } = data;
        public IConfiguration Configuration { get; set; } = configuration;
        public string Id { get; set; } = "";
        public bool IsModded { get; set; } = !isOverride;
    }
}
