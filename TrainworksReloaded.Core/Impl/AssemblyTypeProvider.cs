using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Core.Impl
{
    public class AssemblyTypeProvider : ITypeProvider
    {
        private Assembly Assembly;
        private readonly IDictionary<string, Type> DefinedTypes;

        public AssemblyTypeProvider(Assembly assembly)
        {
            this.Assembly = assembly;
            DefinedTypes = assembly.DefinedTypes.Where(t => t.DeclaringType == null).ToDictionary(k => k.Name, v => v.AsType());
        }

        public bool TryLookupType(string name, [NotNullWhen(true)] out Type? type)
        {
            return DefinedTypes.TryGetValue(name, out type);
        }
    }
}
