using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TrainworksReloaded.Base.Character;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Prefab;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TrainworksReloaded.Base.StatusEffects
{
    public class StatusEffectDataFinalizer : IDataFinalizer
    {
        private readonly IModLogger<StatusEffectDataFinalizer> logger;
        private readonly ICache<IDefinition<StatusEffectData>> cache;
        private readonly IRegister<VfxAtLoc> vfxRegister;
        private readonly IRegister<Sprite> spriteRegister;

        public StatusEffectDataFinalizer(IModLogger<StatusEffectDataFinalizer> logger, ICache<IDefinition<StatusEffectData>> cache, IRegister<VfxAtLoc> vfxRegister, IRegister<Sprite> spriteRegister)
        {
            this.logger = logger;
            this.cache = cache;
            this.vfxRegister = vfxRegister;
            this.spriteRegister = spriteRegister;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeStatusEffect(definition);
            }
            cache.Clear();
        }

        public void FinalizeStatusEffect(IDefinition<StatusEffectData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(Core.Interfaces.LogLevel.Info, $"Finalizing StatusEffect {data.GetStatusId()}... ");

            var icon = configuration.GetSection("icon").ParseString();
            if (
                icon != null
                && spriteRegister.TryLookupName(
                    icon.ToId(key, "Sprite"),
                    out var lookup,
            out var _
                )
            )
            {
                AccessTools.Field(typeof(StatusEffectData), "icon").SetValue(data, lookup);
            }

            var addedVFX = configuration.GetSection("added_vfx").ParseString() ?? "";
            if (vfxRegister.TryLookupId(addedVFX.ToId(key, "Vfx"), out var added_vfx, out var _))
            {
                AccessTools.Field(typeof(StatusEffectData), "addedVFX").SetValue(data, added_vfx);
            }

            var moreAddedVFX = new VfxAtLocList();
            var moreAddedVFXList = moreAddedVFX.GetVfxList();
            foreach (var child in configuration.GetSection("more_added_vfx").GetChildren())
            {
                var vfxName = child.ParseString() ?? "";
                if (vfxRegister.TryLookupId(vfxName.ToId(key, "Vfx"), out var vfx, out var _))
                {
                    moreAddedVFXList.Add(vfx);
                }
            }
            AccessTools.Field(typeof(StatusEffectData), "moreAddedVFX").SetValue(data, moreAddedVFX);

            var persistentVFX = configuration.GetSection("persistent_vfx").ParseString() ?? "";
            if (vfxRegister.TryLookupId(persistentVFX.ToId(key, "Vfx"), out var persistent_vfx, out var _))
            {
                AccessTools.Field(typeof(StatusEffectData), "persistentVFX").SetValue(data, persistent_vfx);
            }

            var morePersistentVFX = new VfxAtLocList();
            var morePersistentVFXList = morePersistentVFX.GetVfxList();
            foreach (var child in configuration.GetSection("more_persistent_vfx").GetChildren())
            {
                var vfxName = child.ParseString() ?? "";
                if (vfxRegister.TryLookupId(vfxName.ToId(key, "Vfx"), out var vfx, out var _))
                {
                    morePersistentVFXList.Add(vfx);
                }
            }
            AccessTools.Field(typeof(StatusEffectData), "morePersistentVFX").SetValue(data, morePersistentVFX);

            var triggeredVFX = configuration.GetSection("triggered_vfx").ParseString() ?? "";
            if (vfxRegister.TryLookupId(triggeredVFX.ToId(key, "Vfx"), out var triggered_vfx, out var _))
            {
                AccessTools.Field(typeof(StatusEffectData), "triggeredVFX").SetValue(data, triggered_vfx);
            }

            var moreTriggeredVFX = new VfxAtLocList();
            var moreTriggeredVFXList = moreTriggeredVFX.GetVfxList();
            foreach (var child in configuration.GetSection("more_triggered_vfx").GetChildren())
            {
                var vfxName = child.ParseString() ?? "";
                if (vfxRegister.TryLookupId(vfxName.ToId(key, "Vfx"), out var vfx, out var _))
                {
                    moreTriggeredVFXList.Add(vfx);
                }
            }
            AccessTools.Field(typeof(StatusEffectData), "moreTriggeredVFX").SetValue(data, moreTriggeredVFX);

            var removedVFX = configuration.GetSection("removed_vfx").ParseString() ?? "";
            if (vfxRegister.TryLookupId(removedVFX.ToId(key, "Vfx"), out var removed_vfx, out var _))
            {
                AccessTools.Field(typeof(StatusEffectData), "removedVFX").SetValue(data, removed_vfx);
            }

            var moreRemovedVFX = new VfxAtLocList();
            var moreRemovedVFXList = moreRemovedVFX.GetVfxList();
            foreach (var child in configuration.GetSection("more_removed_vfx").GetChildren())
            {
                var vfxName = child.ParseString() ?? "";
                if (vfxRegister.TryLookupId(vfxName.ToId(key, "Vfx"), out var vfx, out var _))
                {
                    moreRemovedVFXList.Add(vfx);
                }
            }
            AccessTools.Field(typeof(StatusEffectData), "moreRemovedVFX").SetValue(data, moreRemovedVFX);

            var affectedVFX = configuration.GetSection("affected_vfx").ParseString() ?? "";
            if (vfxRegister.TryLookupId(affectedVFX.ToId(key, "Vfx"), out var affected_vfx, out var _))
            {
                AccessTools.Field(typeof(StatusEffectData), "affectedVFX").SetValue(data, affected_vfx);
            }
        }
    }
}
