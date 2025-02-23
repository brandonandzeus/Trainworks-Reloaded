using System;
using System.Collections.Generic;
using System.Text;

namespace TrainworksReloaded.Core.Interfaces
{
    /// <summary>
    /// A custom register is a dictionary containing registered modded data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICustomRegister<T> : IDictionary<string, T>
    {
        public void Register(string key, T item);
    }
}
