using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Core.Impl
{
    public class InstanceGenerator<T> : IInstanceGenerator<T>
        where T : new()
    {
        public T CreateInstance()
        {
            return new T();
        }
    }
}
