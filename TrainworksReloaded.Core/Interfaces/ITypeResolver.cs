using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace TrainworksReloaded.Core.Interfaces
{
    public interface ITypeResolver
    {
        bool TryResolveType(
            string effectClass,
            string modReference,
            [NotNullWhen(true)] out Type? type,
            [NotNullWhen(true)] out bool? baseGameType
        );
    }
}
