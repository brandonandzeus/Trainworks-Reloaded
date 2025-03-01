using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Card;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using static Unity.Properties.TypeUtility;

namespace TrainworksReloaded.Base.Class
{
    public class ClassDataFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CardDataFinalizer> logger;
        private readonly ICache<IDefinition<ClassData>> cache;
        private readonly IRegister<Sprite> spriteRegister;
        private readonly IRegister<CardData> cardDataRegister;
        private readonly IRegister<CardUpgradeData> upgradeDataRegister;

        public ClassDataFinalizer(
            IModLogger<CardDataFinalizer> logger,
            ICache<IDefinition<ClassData>> cache,
            IRegister<Sprite> spriteRegister,
            IRegister<CardData> cardDataRegister,
            IRegister<CardUpgradeData> upgradeDataRegister
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.spriteRegister = spriteRegister;
            this.cardDataRegister = cardDataRegister;
            this.upgradeDataRegister = upgradeDataRegister;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeClassData(definition);
            }
            cache.Clear();
        }

        /// <summary>
        /// Finalize Card Definitions
        /// Handles Data to avoid lookup looks for names and ids
        /// </summary>
        /// <param name="definition"></param>
        private void FinalizeClassData(IDefinition<ClassData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(Core.Interfaces.LogLevel.Info, $"Finalizing Class {data.name}... ");

            var iconSet = AccessTools.Field(typeof(ClassData), "icons").GetValue(data);
            var iconSetType = typeof(ClassData).GetNestedType(
                "IconSet",
                System.Reflection.BindingFlags.NonPublic
            );
            if (iconSet == null)
            {
                iconSet = iconSetType.GetConstructor([]).Invoke([]);
                AccessTools.Field(typeof(ClassData), "icons").SetValue(data, iconSet);
            }
            var iconField = configuration.GetSection("icons");

            var smallIcon = iconField.GetSection("small").ParseString();
            if (
                smallIcon != null
                && spriteRegister.TryLookupName(
                    smallIcon.ToId(key, "Sprite"),
                    out var lookup,
                    out var _
                )
            )
            {
                AccessTools.Field(iconSetType, "small").SetValue(iconSet, lookup);
            }

            var mediumIcon = iconField.GetSection("medium").ParseString();
            if (
                mediumIcon != null
                && spriteRegister.TryLookupName(
                    mediumIcon.ToId(key, "Sprite"),
                    out var lookup2,
                    out var _
                )
            )
            {
                AccessTools.Field(iconSetType, "medium").SetValue(iconSet, lookup2);
            }

            var largeIcon = iconField.GetSection("large").ParseString();
            if (
                largeIcon != null
                && spriteRegister.TryLookupName(
                    largeIcon.ToId(key, "Sprite"),
                    out var lookup3,
                    out var _
                )
            )
            {
                AccessTools.Field(iconSetType, "large").SetValue(iconSet, lookup3);
            }

            var silhouette = iconField.GetSection("silhouette").ParseString();
            if (
                silhouette != null
                && spriteRegister.TryLookupName(
                    silhouette.ToId(key, "Sprite"),
                    out var lookup4,
                    out var _
                )
            )
            {
                AccessTools.Field(iconSetType, "silhouette").SetValue(iconSet, lookup4);
            }

            //handle starter relics
            var starterRelics =
                (List<RelicData>)
                    AccessTools.Field(typeof(ClassData), "starterRelics").GetValue(data);
            if (starterRelics == null)
            {
                starterRelics = new List<RelicData>();
                AccessTools.Field(typeof(ClassData), "starterRelics").SetValue(data, starterRelics);
            }

            //handle champion data
            var champions = configuration.GetSection("champions").GetChildren().ToList();
            var championDatas =
                (List<ChampionData>)
                    AccessTools.Field(typeof(ClassData), "champions").GetValue(data);
            if (championDatas != null)
            {
                championDatas.Clear();
            }

            if (championDatas == null)
            {
                championDatas = new List<ChampionData>();
                AccessTools.Field(typeof(ClassData), "champions").SetValue(data, championDatas);
            }
            foreach (var champion in champions)
            {
                var championData = ScriptableObject.CreateInstance<ChampionData>();

                //string
                championData.championSelectedCue =
                    configuration.GetSection("selected_cue").ParseString() ?? "";

                //card data
                var championCardData = champion.GetSection("card_data").ParseString();
                if (
                    championCardData != null
                    && cardDataRegister.TryLookupName(
                        championCardData.ToId(key, "Card"),
                        out var championCard,
                        out var _
                    )
                )
                {
                    championData.championCardData = championCard;
                }

                var starterCardData = champion.GetSection("starter_card").ParseString();
                if (
                    starterCardData != null
                    && cardDataRegister.TryLookupName(
                        starterCardData.ToId(key, "Card"),
                        out var starterCard,
                        out var _
                    )
                )
                {
                    championData.starterCardData = starterCard;
                }

                //sprites
                var championIcon = champion.GetSection("icon").ParseString();
                if (
                    championIcon != null
                    && spriteRegister.TryLookupName(
                        championIcon.ToId(key, "Sprite"),
                        out var icon1,
                        out var _
                    )
                )
                {
                    championData.championIcon = icon1;
                }

                var championLockedIcon = champion.GetSection("locked_icon").ParseString();
                if (
                    championLockedIcon != null
                    && spriteRegister.TryLookupName(
                        championLockedIcon.ToId(key, "Sprite"),
                        out var icon2,
                        out var _
                    )
                )
                {
                    championData.championLockedIcon = icon2;
                }

                var championPortrait = champion.GetSection("portrait").ParseString();
                if (
                    championPortrait != null
                    && spriteRegister.TryLookupName(
                        championPortrait.ToId(key, "Sprite"),
                        out var icon3,
                        out var _
                    )
                )
                {
                    championData.championPortrait = icon3;
                }

                //upgrade upgrade tree
                var upgradeTreeData = ScriptableObject.CreateInstance<CardUpgradeTreeData>();
                if (championData.championCardData != null)
                {
                    var championSpawn = championData.championCardData.GetSpawnCharacterData();
                    if (championSpawn != null)
                    {
                        AccessTools
                            .Field(typeof(CardUpgradeTreeData), "champion")
                            .SetValue(upgradeTreeData, championSpawn);
                    }
                }
                var upgradeTrees = new List<CardUpgradeTreeData.UpgradeTree>();
                AccessTools
                    .Field(typeof(CardUpgradeTreeData), "upgradeTrees")
                    .SetValue(upgradeTreeData, upgradeTrees);
                var upgradeTreeConfig = champion.GetSection("upgrade_tree").GetChildren();
                foreach (var config in upgradeTreeConfig)
                {
                    var upgradeTree = new CardUpgradeTreeData.UpgradeTree();
                    var upgrades = new List<CardUpgradeData>();
                    AccessTools
                        .Field(typeof(CardUpgradeTreeData.UpgradeTree), "cardUpgrades")
                        .SetValue(upgradeTree, upgrades);
                    foreach (var upgrade in config.GetChildren())
                    {
                        var upgradeString = upgrade.ParseString();
                        if (
                            upgradeString != null
                            && upgradeDataRegister.TryLookupName(
                                upgradeString.ToId(key, "Upgrade"),
                                out var upgradeObj,
                                out var _
                            )
                        )
                        {
                            upgrades.Add(upgradeObj);
                        }
                    }
                    upgradeTrees.Add(upgradeTree);
                }

                AccessTools
                    .Field(typeof(ChampionData), "upgradeTree")
                    .SetValue(championData, upgradeTreeData);

                championDatas.Add(championData);
            }
        }
    }
}
