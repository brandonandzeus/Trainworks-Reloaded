using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceProviders.Experimental;
using UnityEngine.ResourceManagement.Util;
using static RimLight;

namespace TrainworksReloaded.Base.Prefab
{

    public class TransformOnStart : MonoBehaviour{
        void Start(){
            this.transform.position = new Vector3(10000, 10000, 0);
        }
    }
    /// <summary>
    /// Provide Game Object Prefabs
    /// </summary>
    public class GameObjectRegister
        : Dictionary<string, GameObject>,
            IRegister<GameObject>,
            IResourceLocator,
            IResourceProvider
    {
        private string? m_ProviderId;
        private readonly IModLogger<GameObjectRegister> logger;
        public readonly GameObject hiddenRoot;

        public GameObjectRegister(IModLogger<GameObjectRegister> logger)
        {
            hiddenRoot = new GameObject { name = "Prefabs" };
            GameObject.DontDestroyOnLoad(hiddenRoot);
            hiddenRoot.AddComponent<TransformOnStart>();
            // hiddenRoot.transform.localScale = new Vector3(0.000001f, 0.000001f, 0.000001f);
            this.logger = logger;
        }

        public Dictionary<Hash128, (string, GameObject)> HashToObjectMap { get; set; } = [];

        public virtual string ProviderId
        {
            get
            {
                if (string.IsNullOrEmpty(this.m_ProviderId))
                {
                    this.m_ProviderId = typeof(GameObjectRegister).FullName;
                }
                return this.m_ProviderId;
            }
        }

        public ProviderBehaviourFlags BehaviourFlags => ProviderBehaviourFlags.None;

        IEnumerable<object> IResourceLocator.Keys
        {
            get { return HashToObjectMap.Keys.Cast<object>(); }
        }

        public bool CanProvide<TObject>(IResourceLocation location)
            where TObject : class
        {
            if (location == null)
            {
                throw new ArgumentException("IResourceLocation location cannot be null.");
            }
            return this.ProviderId.Equals(location.ProviderId, StringComparison.Ordinal);
        }

        public bool Initialize(string id, string data)
        {
            this.m_ProviderId = id;
            return !string.IsNullOrEmpty(this.m_ProviderId);
        }

        public bool Locate(object key, out IList<IResourceLocation> locations)
        {
            locations = [];
            if (key is Hash128 hash && HashToObjectMap.TryGetValue(hash, out var value))
            {
                var location = new ResourceLocationBase(
                    value.Item1,
                    value.Item1,
                    typeof(GameObjectRegister).FullName,
                    []
                )
                {
                    Data = value.Item2,
                };
                locations.Add(location);
                return true;
            }
            else
            {
                return false;
            }
        }

        public IAsyncOperation<TObject> Provide<TObject>(
            IResourceLocation location,
            IList<object> dependencies
        )
            where TObject : class
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }

            if (!typeof(TObject).IsAssignableFrom(typeof(GameObject)))
            {
                throw new ArgumentNullException("gameobject");
            }

            logger.Log(LogLevel.Info, $"Providing for {location.InternalId}");
            var obj = this[location.InternalId];
            // obj.SetActive(true);
            if (obj is TObject @object)
            {

                return new CompletedOperation<TObject>().Start(
                    location,
                    location.InternalId,
                    @object
                );
            }
            else
            {
                logger.Log(LogLevel.Info, $"Did not Find for {location.InternalId}");
                return new CompletedOperation<TObject>().Start(
                    location,
                    location.InternalId,
                    default!
                );
            }
        }

        public bool Release(IResourceLocation location, object asset)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            return true;
        }

        public void Register(string key, GameObject item)
        {
            var hash = Hash128.Compute(key);
            logger.Log(LogLevel.Info, $"Register GameObject ({key}) -- ({hash})");
            item.name = key;
            HashToObjectMap.Add(hash, (key, item));
            item.transform.SetParent(hiddenRoot.transform, false);
            this.Add(key, item);
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out GameObject? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            IsModded = true;
            return this.TryGetValue(id, out lookup);
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out GameObject? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = true;
            foreach (var gameobject in this.Values)
            {
                if (gameobject.name == name)
                {
                    lookup = gameobject;
                    IsModded = true;
                    return true;
                }
            }

            return false;
        }
    }
}
