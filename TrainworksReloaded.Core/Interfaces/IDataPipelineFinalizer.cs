namespace TrainworksReloaded.Core.Interfaces
{
    /// <summary>
    /// A Data Pipeline Finalizer is used by a Data Pipeline to Finalize the reference Data using additional services.
    /// This is run after all of the same type has been registered via pipeline
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataPipelineFinalizer<T>
    {
        public void Finalize(IDefinition<T> definition);
    }
}
