using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TrainworksReloaded.Base.Character;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.Class
{
    public class ClassDataRegister : Dictionary<string, ClassData>, IRegister<ClassData>
    {
        private readonly Lazy<SaveManager> SaveManager;
        private readonly IModLogger<ClassDataRegister> logger;

        public ClassDataRegister(GameDataClient client, IModLogger<ClassDataRegister> logger)
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

        public void Register(string key, ClassData item)
        {
            logger.Log(Core.Interfaces.LogLevel.Info, $"Register Class {key}... ");
            var gamedata = SaveManager.Value.GetAllGameData();
            var ClassDatas =
                (List<ClassData>)
                    AccessTools.Field(typeof(AllGameData), "classDatas").GetValue(gamedata);
            ClassDatas.Add(item);
            this.Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out ClassData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = null;
            foreach (var @class in SaveManager.Value.GetAllGameData().GetAllClassDatas())
            {
                if (@class.GetID().Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = @class;
                    IsModded = ContainsKey(@class.name);
                    return true;
                }
            }
            return false;
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out ClassData? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = null;
            foreach (var @class in SaveManager.Value.GetAllGameData().GetAllClassDatas())
            {
                if (@class.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = @class;
                    IsModded = ContainsKey(@class.name);
                    return true;
                }
            }
            return false;
        }
    }
}
