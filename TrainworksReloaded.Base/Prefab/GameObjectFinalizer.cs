


using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Prefab
{
    public class GameObjectFinalizer(
        IRegister<GameObject> gameObjectRegister,
        ICache<IDefinition<GameObject>> cache
        ) : IDataFinalizer
    {
        private readonly ICache<IDefinition<GameObject>> cache = cache;
        private readonly IRegister<GameObject> gameObjectRegister = gameObjectRegister;

        public void FinalizeData()
        {
            cache.Clear();
        }
    }
}