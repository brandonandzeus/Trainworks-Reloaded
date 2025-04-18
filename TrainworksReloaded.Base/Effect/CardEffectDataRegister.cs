using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TrainworksReloaded.Core.Enum;
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

        public List<string> GetAllIdentifiers(RegisterIdentifierType identifierType)
        {
            return identifierType switch
            {
                RegisterIdentifierType.ReadableID => [.. this.Values.Select(effect => effect.GetEffectStateName())],
                RegisterIdentifierType.GUID => [.. this.Keys],
                _ => []
            };
        }
        public bool TryLookupIdentifier(string identifier, RegisterIdentifierType identifierType, [NotNullWhen(true)] out CardEffectData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            lookup = null;
            IsModded = true;
            switch (identifierType)
            {
                case RegisterIdentifierType.ReadableID:
                    foreach (var effect in this.Values)
                    {
                        if (effect.GetEffectStateName() == identifier)
                        {
                            lookup = effect;
                            IsModded = true;
                            return true;
                        }
                    }
                    return false;
                case RegisterIdentifierType.GUID:
                    IsModded = true;
                    return this.TryGetValue(identifier, out lookup);
            }
            return false;
        }
    }
}
