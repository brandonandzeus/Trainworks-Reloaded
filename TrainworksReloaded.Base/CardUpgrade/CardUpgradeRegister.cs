using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using HarmonyLib;
using Malee;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Core.Enum;
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

        public List<string> GetAllIdentifiers(RegisterIdentifierType identifierType)
        {
            return identifierType switch
            {
                RegisterIdentifierType.ReadableID => [.. SaveManager.Value.GetAllGameData().GetAllCardUpgradeData().Select(upgrade => upgrade.GetAssetKey())],
                RegisterIdentifierType.GUID => [.. SaveManager.Value.GetAllGameData().GetAllCardUpgradeData().Select(upgrade => upgrade.GetID())],
                _ => [],
            };
        }

        public bool TryLookupIdentifier(string identifier, RegisterIdentifierType identifierType, [NotNullWhen(true)] out CardUpgradeData? lookup, [NotNullWhen(true)] out bool? IsModded)
        {
            lookup = null;
            IsModded = null;
            switch (identifierType)
            {
                case RegisterIdentifierType.ReadableID:
                    foreach (var card in SaveManager.Value.GetAllGameData().GetAllCardUpgradeData())
                    {
                        if (card.GetAssetKey().Equals(identifier, StringComparison.OrdinalIgnoreCase))
                        {
                            lookup = card;
                            IsModded = this.ContainsKey(card.name);
                            return true;
                        }
                    }
                    return false;
                case RegisterIdentifierType.GUID:
                    foreach (var card in SaveManager.Value.GetAllGameData().GetAllCardUpgradeData())
                    {
                        if (card.GetID().Equals(identifier, StringComparison.OrdinalIgnoreCase))
                        {
                            lookup = card;
                            IsModded = this.ContainsKey(card.name);
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
        }

    }
}
