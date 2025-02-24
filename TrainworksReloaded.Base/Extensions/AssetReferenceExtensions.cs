using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;

namespace TrainworksReloaded.Base.Extensions
{
    public static class AssetReferenceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetReference"></param>
        /// <param name="registeredGUID">A GUID to the Asset for Dynamic Loading, needs to be registered to an IResourceLocator</param>
        /// <param name="object"></param>
        public static void SetAssetAndId(this AssetReference assetReference, string registeredGUID, UnityEngine.Object @object)
        {
            assetReference.SetAsset(@object);
            AccessTools.Field(typeof(AssetReference), "m_AssetGUID").SetValue(assetReference, registeredGUID);
        }
    }
}
