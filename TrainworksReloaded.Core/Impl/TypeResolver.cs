using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Core.Impl
{
    public class TypeResolver : ITypeResolver
    {
        private readonly ITypeProvider MT2Assembly;
        private readonly IDictionary<string, ITypeProvider> modGuidToAssembly;

        public TypeResolver(ITypeProvider mT2Assembly, IDictionary<string, ITypeProvider> assemblies)
        {
            MT2Assembly = mT2Assembly;
            this.modGuidToAssembly = assemblies;
        }

        /// <summary>
        /// Attempts to resolve the fully qualified type name given a class name.
        /// 
        /// The search starts with the Assembly pointed to by modReference. If it is not present within that assembly
        /// it searchs the MT2 Assembly for the type. If it is not present in either then null is returned via type.
        /// </summary>
        /// <param name="effectClass"></param>
        /// <param name="modReference"></param>
        /// <param name="type"></param>
        /// <param name="baseGameType"></param>
        /// <returns></returns>
        public bool TryResolveType(
            string effectClass,
            string modReference,
            [NotNullWhen(true)] out Type? type,
            [NotNullWhen(true)] out bool? baseGameType
        )
        {

            ITypeProvider assembly;
            type = null;
            baseGameType = false;
            bool found = false;
            if (modGuidToAssembly.TryGetValue(modReference, out assembly))
            {
                found = assembly.TryLookupType(effectClass, out type);
            }
            if (!found)
            {
                found = MT2Assembly.TryLookupType(effectClass, out type);
                baseGameType = found;
            }
            return found;
        }
    }
}
