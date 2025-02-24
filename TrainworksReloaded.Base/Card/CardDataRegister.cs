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

        public CardDataRegister(GameDataClient client)
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
        }

        public bool TryLookupId(string id, [NotNullWhen(true)] out CardData? lookup)
        {
            lookup = null;
            foreach (var card in SaveManager.Value.GetAllGameData().GetAllCardData())
            {
                if (card.GetID().Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = card;
                    return true;
                }
            }
            return false;
        }

        public bool TryLookupName(string name, [NotNullWhen(true)] out CardData? lookup)
        {
            lookup = null;
            foreach (var card in SaveManager.Value.GetAllGameData().GetAllCardData())
            {
                if (card.GetAssetKey().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = card;
                    return true;
                }
            }
            return false;
        }
    }
}
