using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Trait
{
    public class CardTraitDataRegister : Dictionary<string, CardTraitData>, IRegister<CardTraitData>
    {
        private readonly IModLogger<CardTraitDataRegister> logger;

        public CardTraitDataRegister(IModLogger<CardTraitDataRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, CardTraitData item)
        {
            logger.Log(LogLevel.Info, $"Register Trait ({key})");
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out CardTraitData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out CardTraitData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
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
