using Microsoft.Extensions.Configuration;

namespace TrainworksReloaded.Core.Interfaces
{
    public interface IDefinition<T>
    {
        public string Key { get; set; }
        public T Data { get; set; }
        public IConfiguration Configuration { get; set; }
    }
}
