using Microsoft.Extensions.Configuration;

namespace TrainworksReloaded.Core.Interfaces
{
    public interface IDefinition<T>
    {
        public string Key { get; }
        public string Id { get; }
        public T Data { get; }
        public IConfiguration Configuration { get; }
        public bool IsModded { get; }
    }
}
