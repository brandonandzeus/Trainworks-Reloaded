using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Base.CardUpgrade;
using TrainworksReloaded.Core.Interfaces;
using TrainworksReloaded.Core.Enum;

namespace TrainworksReloaded.Base.Card
{
    public class CardpoolRegister : Dictionary<string, CardPool>, IRegister<CardPool>
    {
        private readonly IModLogger<CardpoolRegister> logger;

        public CardpoolRegister(GameDataClient client, IModLogger<CardpoolRegister> logger)
        {
            this.logger = logger;
        }

        public void Register(string key, CardPool item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Register Card Pool {key}... ");
            Add(key, item);
        }


        public List<string> GetAllIdentifiers(RegisterIdentifierType identifierType)
        {
            return [.. this.Keys];
        }

        public bool TryLookupIdentifier(
            string identifier,
            RegisterIdentifierType identifierType,
            [NotNullWhen(true)] out CardPool? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(identifier, out lookup);
        }
    }
}
