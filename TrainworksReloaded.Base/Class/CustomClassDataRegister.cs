using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Class
{
    public class CustomClassDataRegister : Dictionary<string, ClassData>, IRegister<ClassData>
    {
        public void Register(string key, ClassData item)
        {
            this.Add(key, item);
        }

        public bool TryLookupId(string id, [NotNullWhen(true)] out ClassData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            throw new NotImplementedException();
        }

        public bool TryLookupName(string name, [NotNullWhen(true)] out ClassData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            throw new NotImplementedException();
        }
    }
}
