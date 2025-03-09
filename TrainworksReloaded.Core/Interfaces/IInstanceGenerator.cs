using System;
using System.Collections.Generic;
using System.Text;

namespace TrainworksReloaded.Core.Interfaces
{
    public interface IInstanceGenerator<T>
        where T : new()
    {
        T CreateInstance();
    }
}
