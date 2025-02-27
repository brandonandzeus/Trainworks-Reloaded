using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Malee;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Character
{
    public class CharacterDataRegister : Dictionary<string, CharacterData>, IRegister<CharacterData>
    {
        private readonly Lazy<SaveManager> SaveManager;
        private readonly IModLogger<CharacterDataRegister> logger;

        public CharacterDataRegister(
            GameDataClient client,
            IModLogger<CharacterDataRegister> logger
        )
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

        public void Register(string key, CharacterData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Registering Character {key}... ");
            var gamedata = SaveManager.Value.GetAllGameData();
            var CharacterDatas =
                (List<CharacterData>)
                    AccessTools.Field(typeof(AllGameData), "characterDatas").GetValue(gamedata);
            CharacterDatas.Add(item);
            Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out CharacterData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = null;
            foreach (var card in SaveManager.Value.GetAllGameData().GetAllCharacterData())
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
            [NotNullWhen(true)] out CharacterData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = null;
            foreach (var card in SaveManager.Value.GetAllGameData().GetAllCharacterData())
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
