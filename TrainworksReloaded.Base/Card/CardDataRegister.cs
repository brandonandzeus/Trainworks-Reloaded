﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Malee;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Card
{
    public class CardDataRegister : Dictionary<string, CardData>, IRegister<CardData>
    {
        private readonly Lazy<SaveManager> SaveManager;
        private readonly IModLogger<CardDataRegister> logger;
        public CardPool CustomCardPool;
        public ReorderableArray<CardData> CardPoolBacking;

        public CardDataRegister(GameDataClient client, IModLogger<CardDataRegister> logger)
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
            CardPoolBacking =
                (ReorderableArray<CardData>)
                    AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(CustomCardPool);
            this.logger = logger;
        }

        public void Register(string key, CardData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Registering Card {key}... ");
            CardPoolBacking.Add(item);
            var gamedata = SaveManager.Value.GetAllGameData();
            var CardDatas =
                (List<CardData>)
                    AccessTools.Field(typeof(AllGameData), "cardDatas").GetValue(gamedata);
            CardDatas.Add(item);
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out CardData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
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

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out CardData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
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
