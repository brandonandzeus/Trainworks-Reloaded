namespace TrainworksReloaded.Core.Interfaces
{
    /// <summary>
    /// A Data Pipeline Setup is used by a Data Pipeline to Setup the reference Data after it has been registered
    /// This is designed to handle extensions to configuration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataPipelineSetup<T>
    {
        public void Setup(IDefinition<T> definition);
    }
}
