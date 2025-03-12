using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using TrainworksReloaded.Base.Extensions;

namespace TrainworksReloaded.Base.Util
{
    class EffectClassUtils
    {
        public static readonly Assembly MT2Assembly = typeof(CardEffectDamage).Assembly;

        public static bool GetFullyQualifiedName(string className, Assembly? assembly, Type baseClass, [NotNullWhen(true)] out string? fullyQualifiedName)
        {
            Type? foundType = null;
            bool baseGameType = false;
            fullyQualifiedName = null;
            if (assembly != null)
            {
                foundType = assembly.FindTypeByClassName(className);
            }
            if (foundType == null)
            {
                baseGameType = true;
                foundType = MT2Assembly.FindTypeByClassName(className);
            }
            if (foundType != null && baseClass.IsAssignableFrom(foundType))
            {
                fullyQualifiedName = baseGameType ? className : foundType.AssemblyQualifiedName;
                return true;
            }
            return false;
        }
    }
}
