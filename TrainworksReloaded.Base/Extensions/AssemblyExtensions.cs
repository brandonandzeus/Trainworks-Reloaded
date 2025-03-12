using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TrainworksReloaded.Base.Extensions
{
    public static class AssemblyExtensions
    {
        public static Type? FindTypeByClassName(this Assembly assembly, string class_name)
        {
            return assembly.DefinedTypes.FirstOrDefault(t => t.DeclaringType == null && t.Name == class_name);
        }
    }
}
