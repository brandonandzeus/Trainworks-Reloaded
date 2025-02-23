using Microsoft.Extensions.Configuration;
using ShinyShoe;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainworksReloaded.Base.Extensions
{
    public static class ParseEnumExtensions
    {
        public static CardData.CostType? ParseCostType(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            return val.ToLower() switch
            {
                "default" => CardData.CostType.Default,
                "x" => CardData.CostType.ConsumeRemainingEnergy,
                "unplayable" => CardData.CostType.NonPlayable,
                _ => null,
            };
        }

        public static CardType? ParseCardType(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            return val.ToLower() switch
            {
                "invalid" => CardType.Invalid,
                "spell" => CardType.Spell,
                "monster" => CardType.Monster,
                "blight" => CardType.Blight,
                "junk" => CardType.Junk,
                "equipment" => CardType.Equipment,
                "room" => CardType.TrainRoomAttachment,
                _ => null,
            };
        }
        public static CollectableRarity? ParseRarity(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            return val.ToLower() switch
            {
                "common" => CollectableRarity.Common,
                "uncommon" => CollectableRarity.Uncommon,
                "starter" => CollectableRarity.Starter,
                "rare" => CollectableRarity.Rare,
                "champion" => CollectableRarity.Champion,
                _ => null,
            };

        }
        public static DLC? ParseDLC(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            return val.ToLower() switch
            {
                "none" => DLC.None,
                _ => null,
            };
        }
    }
}
