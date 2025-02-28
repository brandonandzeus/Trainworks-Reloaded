namespace TrainworksReloaded.Core.Interfaces
{
    public interface IDataPipeline<T, U>
        where T : IRegister<U>
    {
        public List<IDefinition<U>> Run(T service);
    }
}
