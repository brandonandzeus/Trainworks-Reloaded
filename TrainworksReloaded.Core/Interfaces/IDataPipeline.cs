namespace TrainworksReloaded.Core.Interfaces
{
    /// <summary>
    /// Marks
    /// </summary>
    public interface IDataPipeline<T>
    {
        public void Run(T service);
    }
}
