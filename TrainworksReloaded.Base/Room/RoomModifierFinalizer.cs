using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base.Room
{
    public class RoomModifierFinalizer : IDataFinalizer
    {
        private readonly IModLogger<RoomModifierFinalizer> logger;
        private readonly ICache<IDefinition<RoomModifierData>> cache;
        private readonly IRegister<Sprite> spriteRegister;
        private readonly IRegister<CardData> cardDataRegister;
        private readonly IRegister<CardUpgradeData> upgradeDataRegister;
        private readonly IRegister<CardEffectData> cardEffectDataRegister;
        private readonly IRegister<VfxAtLoc> vfxRegister;

        public RoomModifierFinalizer(
            IModLogger<RoomModifierFinalizer> logger,
            ICache<IDefinition<RoomModifierData>> cache,
            IRegister<Sprite> spriteRegister,
            IRegister<CardData> cardDataRegister,
            IRegister<CardUpgradeData> upgradeDataRegister,
            IRegister<CardEffectData> cardEffectDataRegister,
            IRegister<VfxAtLoc> vfxRegister
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.spriteRegister = spriteRegister;
            this.cardDataRegister = cardDataRegister;
            this.upgradeDataRegister = upgradeDataRegister;
            this.cardEffectDataRegister = cardEffectDataRegister;
            this.vfxRegister = vfxRegister;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeRoomModifiers(definition);
            }
            cache.Clear();
        }

        /// <summary>
        /// Finalize Card Definitions
        /// Handles Data to avoid lookup looks for names and ids
        /// </summary>
        /// <param name="definition"></param>
        private void FinalizeRoomModifiers(IDefinition<RoomModifierData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(
                Core.Interfaces.LogLevel.Info,
                $"Finalizing Room Modifier {definition.Id.ToId(key, "RoomModifier")}... "
            );

            var sprite = configuration.GetSection("sprite").ParseString();
            if (
                sprite != null
                && spriteRegister.TryLookupId(
                    sprite.ToId(key, "Sprite"),
                    out var spriteLookup,
                    out var _
                )
            )
            {
                AccessTools.Field(typeof(RoomModifierData), "icon").SetValue(data, spriteLookup);
            }

            var param_card_upgrade = configuration.GetSection("param_card_upgrade").ParseString();
            if (
                param_card_upgrade != null
                && upgradeDataRegister.TryLookupId(
                    param_card_upgrade.ToId(key, "Upgrade"),
                    out var upgradeLookup,
                    out var _
                )
            )
            {
                AccessTools
                    .Field(typeof(RoomModifierData), "paramCardUpgardeData")
                    .SetValue(data, upgradeLookup);
            }

            var trigered_vfx = configuration.GetSection("trigered_vfx").ParseString();
            if (
                trigered_vfx != null
                && vfxRegister.TryLookupId(
                    trigered_vfx.ToId(key, "Vfx"),
                    out var vfxLookup,
                    out var _
                )
            )
            {
                AccessTools
                    .Field(typeof(RoomModifierData), "triggeredVFX")
                    .SetValue(data, vfxLookup);
            }

            var cardEffectDatas = new List<CardEffectData>();
            var cardEffectDatasConfig = configuration.GetSection("effects").GetChildren();
            foreach (var configEffect in cardEffectDatasConfig)
            {
                if (configEffect == null)
                {
                    continue;
                }
                var idConfig = configEffect.GetSection("id").Value;
                if (idConfig == null)
                {
                    continue;
                }
                var id = idConfig.ToId(key, "Effect");
                if (cardEffectDataRegister.TryLookupId(id, out var card, out var _))
                {
                    cardEffectDatas.Add(card);
                }
            }

            if (cardEffectDatas.Count != 0)
                AccessTools
                    .Field(typeof(RoomModifierData), "paramCardEffects")
                    .SetValue(data, cardEffectDatas);
        }
    }
}
