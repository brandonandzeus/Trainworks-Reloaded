namespace TrainworksReloaded.Core.Interfaces
{
    public interface IDataPipelineSetup<T>
    {
        public void Setup(IDefinition<T> definition);
    }
}
