using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Core.Interfaces;
using static RimLight;

namespace TrainworksReloaded.Base.Trait
{
    public class CardTraitDataRegister : Dictionary<string, CardTraitData>, IRegister<CardTraitData>
    {
        public void Register(string key, CardTraitData item)
        {
            Add(key, item);
        }

        public bool TryLookupId(string id, [NotNullWhen(true)] out CardTraitData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(string name, [NotNullWhen(true)] out CardTraitData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            lookup = null;
            IsModded = true;
            foreach (var trait in this.Values)
            {

                if (trait.traitStateName == name)
                {
                    lookup = trait;
                    IsModded = true;
                    return true;
                }
            }
            return false;
        }
    }
}
