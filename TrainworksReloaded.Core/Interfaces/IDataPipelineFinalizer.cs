namespace TrainworksReloaded.Core.Interfaces
{
    public interface IDataPipelineFinalizer<T>
    {
        public void Finalize(IDefinition<T> definition);
    }
}
