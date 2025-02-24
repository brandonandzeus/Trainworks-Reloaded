using HarmonyLib;
using Malee;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using static RotaryHeart.Lib.DataBaseExample;

namespace TrainworksReloaded.Base.Card
{
    public class CustomCardDataRegister : Dictionary<string, CardData>, IRegister<CardData>
    {
        private readonly Lazy<SaveManager> SaveManager;
        public CardPool CustomCardPool;
        public ReorderableArray<CardData> CardPoolBacking;
        public CustomCardDataRegister(GameDataClient client)
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
            CustomCardPool = ScriptableObject.CreateInstance<CardPool>();
            CardPoolBacking = (ReorderableArray<CardData>)AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(CustomCardPool);
        }
        public void Register(string key, CardData item)
        {
            CardPoolBacking.Add(item);
            var gamedata = SaveManager.Value.GetAllGameData();
            var CardDatas = (List<CardData>)AccessTools.Field(typeof(AllGameData), "cardDatas").GetValue(gamedata);
            CardDatas.Add(item);
            Add(key, item);
        }

        public bool TryLookupId(string id, [NotNullWhen(true)] out CardData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            lookup = null;
            IsModded = null;
            foreach (var card in SaveManager.Value.GetAllGameData().GetAllCardData())
            {
                if (card.GetID().Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = card;
                    IsModded = this.ContainsKey(card.name);
                    return true;
                }
            }
            return false;
        }

        public bool TryLookupName(string name, [NotNullWhen(true)] out CardData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            lookup = null;
            IsModded = null;
            foreach (var card in SaveManager.Value.GetAllGameData().GetAllCardData())
            {
                if (card.GetAssetKey().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = card;
                    IsModded = this.ContainsKey(card.name);
                    return true;
                }
            }
            return false;
        }
    }
}
