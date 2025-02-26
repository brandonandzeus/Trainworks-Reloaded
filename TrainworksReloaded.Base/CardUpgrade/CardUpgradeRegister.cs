using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using Malee;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.CardUpgrade
{
    public class CardUpgradeRegister
        : Dictionary<string, CardUpgradeData>,
            IRegister<CardUpgradeData>
    {
        private readonly Lazy<SaveManager> SaveManager;
        private readonly IModLogger<CardUpgradeRegister> logger;

        public CardUpgradeRegister(GameDataClient client, IModLogger<CardUpgradeRegister> logger)
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
            this.logger = logger;
        }

        public void Register(string key, CardUpgradeData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Registering Upgrade {key}... ");
            var gamedata = SaveManager.Value.GetAllGameData();
            var CardUpgradeDatas =
                (List<CardUpgradeData>)
                    AccessTools.Field(typeof(AllGameData), "cardUpgradeDatas").GetValue(gamedata);
            CardUpgradeDatas.Add(item);
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out CardUpgradeData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = null;
            foreach (var card in SaveManager.Value.GetAllGameData().GetAllCardUpgradeData())
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
            [NotNullWhen(true)] out CardUpgradeData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = null;
            foreach (var card in SaveManager.Value.GetAllGameData().GetAllCardUpgradeData())
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
