using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrainworksReloaded.Core.Interfaces;
using static RimLight;

namespace TrainworksReloaded.Base.Class
{
    public class ClassDataRegister : IRegister<ClassData>
    {
        private readonly Lazy<SaveManager> SaveManager;

        public ClassDataRegister(GameDataClient client)
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

        public bool TryLookupId(string id, [NotNullWhen(true)] out ClassData? lookup)
        {   
            lookup = null;
            foreach (var @class in SaveManager.Value.GetAllGameData().GetAllClassDatas())
            {
                if (@class.GetID().Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = @class;
                    return true;
                }
            }
            return false;
        }

        public bool TryLookupName(string name, [NotNullWhen(true)] out ClassData? lookup)
        {
            lookup = null;
            foreach (var @class in SaveManager.Value.GetAllGameData().GetAllClassDatas())
            {
                if (@class.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    lookup = @class;
                    return true;
                }
            }
            return false;
        }
    }
}
