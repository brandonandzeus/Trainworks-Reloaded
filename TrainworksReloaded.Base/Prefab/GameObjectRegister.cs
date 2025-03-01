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
using static RimLight;

namespace TrainworksReloaded.Base.Prefab
{
    public class GameObjectRegister
        : Dictionary<string, GameObject>,
            IRegister<GameObject>,
            IResourceLocator,
            IResourceProvider
    {
        private string? m_ProviderId;
        private readonly IModLogger<GameObjectRegister> logger;

        public GameObjectRegister(IModLogger<GameObjectRegister> logger)
        {
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
            logger.Log(LogLevel.Info, key);
            if (key is Hash128 hash && HashToObjectMap.TryGetValue(hash, out var value))
            {
                logger.Log(LogLevel.Info, hash);
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
            return AsyncOperationCache
                .Instance.Acquire<InternalOp<TObject>>()
                .Start(location, dependencies, this[location.InternalId]);
        }

        internal class InternalOp<TObject> : AsyncOperationBase<TObject>
            where TObject : class
        {
            public IAsyncOperation<TObject> Start(
                IResourceLocation location,
                IList<object> deps,
                object result
            )
            {
                base.Validate();
                base.Context = location;
                if (result is TObject @object)
                {
                    this.SetResult(@object);
                }
                else
                {
                    this.SetResult(default!);
                }

                base.InvokeCompletionEvent();
                return this;
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
