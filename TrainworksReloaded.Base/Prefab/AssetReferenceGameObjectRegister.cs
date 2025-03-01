using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RimLight;

namespace TrainworksReloaded.Base.Prefab
{
    public class AssetReferenceGameObjectRegister
        : Dictionary<string, AssetReferenceGameObject>,
            IRegister<AssetReferenceGameObject>
    {
        private readonly IRegister<GameObject> objectRegister;

        public AssetReferenceGameObjectRegister(IRegister<GameObject> objectRegister)
        {
            this.objectRegister = objectRegister;
        }

        public void Register(string key, AssetReferenceGameObject item)
        {
            //
        }

        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out AssetReferenceGameObject? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = false;
            if (objectRegister.TryLookupId(id, out GameObject? gameObject, out bool? modded))
            {
                lookup = new AssetReferenceGameObject();
                IsModded = modded;
                lookup.SetAssetAndId(Hash128.Compute(id).ToString(), gameObject);
                return true;
            }

            lookup = new AssetReferenceGameObject();
            lookup.SetId(id);
            return true;
        }

        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out AssetReferenceGameObject? lookup,
            [NotNullWhen(true)] out bool? IsModded
        )
        {
            lookup = null;
            IsModded = false;
            if (objectRegister.TryLookupId(name, out GameObject? gameObject, out bool? modded))
            {
                lookup = new AssetReferenceGameObject();
                IsModded = modded;
                lookup.SetAssetAndId(Hash128.Compute(name).ToString(), gameObject);
                return true;
            }

            lookup = new AssetReferenceGameObject();
            lookup.SetId(name);
            return true;
        }
    }
}
