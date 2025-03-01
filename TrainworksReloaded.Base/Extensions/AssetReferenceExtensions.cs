using HarmonyLib;
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
        public static void SetAssetAndId(
            this AssetReference assetReference,
            string registeredGUID,
            UnityEngine.Object @object
        )
        {
            assetReference.SetAsset(@object);
            AccessTools
                .Field(typeof(AssetReference), "m_AssetGUID")
                .SetValue(assetReference, registeredGUID);
            AccessTools
                .Field(typeof(AssetReference), "m_debugName")
                .SetValue(assetReference, $"TR-{registeredGUID}");
        }

        public static void SetId(this AssetReference assetReference, string registeredGUID)
        {
            AccessTools
                .Field(typeof(AssetReference), "m_AssetGUID")
                .SetValue(assetReference, registeredGUID);
            AccessTools
                .Field(typeof(AssetReference), "m_debugName")
                .SetValue(assetReference, $"TR-{registeredGUID}");
        }
    }
}
