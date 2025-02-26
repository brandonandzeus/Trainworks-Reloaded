using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Effect
{
    public class CardEffectDataRegister
        : Dictionary<string, CardEffectData>,
            IRegister<CardEffectData>
    {
        private readonly IModLogger<CardEffectDataRegister> logger;

        public CardEffectDataRegister(IModLogger<CardEffectDataRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, CardEffectData item)
        {
            logger.Log(LogLevel.Info, $"Register Effect ({key})");
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out CardEffectData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out CardEffectData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = true;
            foreach (var effect in this.Values)
            {
                if (effect.GetEffectStateName() == name)
                {
                    lookup = effect;
                    IsModded = true;
                    return true;
                }
            }
            return false;
        }
    }
}
