using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TrainworksReloaded.Core.Interfaces
{
    public interface ITypeProvider
    {
        bool TryLookupType(string name, [NotNullWhen(true)] out Type? type);
    }
}
