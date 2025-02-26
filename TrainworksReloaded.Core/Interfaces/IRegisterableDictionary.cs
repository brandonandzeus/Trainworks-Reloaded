namespace TrainworksReloaded.Core.Interfaces
{
    public interface IRegisterableDictionary<T> : IDictionary<string, T>
    {
        public void Register(string key, T item);
    }
}
