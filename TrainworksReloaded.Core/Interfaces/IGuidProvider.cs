using System;
using System.Collections.Generic;
using System.Text;

namespace TrainworksReloaded.Core.Interfaces
{
    public interface IGuidProvider
    {
        Guid GetGuidDeterministic(string key);
    }
}
