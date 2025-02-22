using System;
using System.Collections.Generic;
using System.Text;

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
