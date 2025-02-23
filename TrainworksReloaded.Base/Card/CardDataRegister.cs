using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Card
{
    public class CardDataRegister : IRegister<CardData>
    {
        private readonly Lazy<SaveManager> SaveManager;
        private readonly ICustomRegister<CardData> customRegister;

        public CardDataRegister(GameDataClient client, ICustomRegister<CardData> customRegister)
        {
            SaveManager = new Lazy<SaveManager>(() =>
            {
                if (client.TryGetValue(typeof(SaveManager).Name, out var details))
                {
                    return (SaveManager)details.Provider;
                }
                else
                {
                    return new SaveManager();
                }
            });
            this.customRegister = customRegister;
        }

        public bool TryLookupId(string id, [NotNullWhen(true)] out CardData? lookup, [NotNullWhen(true)] out bool? isModded)
        {
            lookup = null;
            isModded = null;
            foreach (var card in SaveManager.Value.GetAllGameData().GetAllCardData())
            {
                if (card.GetID().Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = card;
                    isModded = customRegister.ContainsKey(card.name);
                    return true;
                }
            }
            return false;
        }

        public bool TryLookupName(string name, [NotNullWhen(true)] out CardData? lookup, [NotNullWhen(true)] out bool? isModded)
        {
            lookup = null;
            isModded = null;
            foreach (var card in SaveManager.Value.GetAllGameData().GetAllCardData())
            {
                if (card.GetAssetKey().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = card;
                    isModded = customRegister.ContainsKey(card.name);
                    return true;
                }
            }
            return false;
        }
    }
}
