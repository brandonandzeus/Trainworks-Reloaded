using HarmonyLib;
using Microsoft.Extensions.Configuration;
using ShinyShoe;
using System;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Base.Localization;

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
        public static CardInitialKeyboardTarget? ParseKeyboardTarget(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            return val.ToLower() switch
            {
                "front_friendly" => CardInitialKeyboardTarget.FrontFriendly,
                "front_enemy" => CardInitialKeyboardTarget.FrontEnemy,
                "back_friendly" => CardInitialKeyboardTarget.BackFriendly,
                "back_enemy" => CardInitialKeyboardTarget.BackEnemy,
                _ => null,
            };

        }

        public static LocalizationTerm? ParseLocalizationTerm(this IConfigurationSection section)
        {
            var key = section.GetSection("key").Value;
            var type = section.GetSection("type").Value;
            var description = section.GetSection("description").Value;
            var group = section.GetSection("group").Value;
            var speaker_descriptions = section.GetSection("speaker_descriptions").Value;
            var english = section.GetSection("english").Value;
            var french = section.GetSection("french").Value;
            var german = section.GetSection("german").Value;
            var russian = section.GetSection("russian").Value;
            var portuguese = section.GetSection("portuguese").Value;
            var chinese = section.GetSection("chinese").Value;
            var spanish = section.GetSection("spanish").Value;
            var chinese_traditional = section.GetSection("chinese_traditional").Value;
            var korean = section.GetSection("korean").Value;
            var japanese = section.GetSection("japanese").Value;
            if (key == null &&
                type == null &&
                description == null &&
                group == null &&
                speaker_descriptions == null &&
                english == null &&
                french == null &&
                german == null &&
                russian == null &&
                portuguese == null &&
                chinese == null &&
                spanish == null &&
                chinese_traditional == null &&
                korean == null &&
                japanese == null)
            {
                return null;
            }

            return new LocalizationTerm()
            {
                Key = key ?? "",
                Type = type ?? "Text",
                Desc = description ?? "",
                Group = group ?? "",
                Descriptions = speaker_descriptions ?? "",
                English = english ?? "",
                French = french ?? "",
                German = german ?? "",
                Russian = russian ?? "",
                Portuguese = portuguese ?? "",
                Chinese = chinese ?? "",
                Spanish = spanish ?? "",
                ChineseTraditional = chinese_traditional ?? "",
                Korean = korean ?? "",
                Japanese = japanese ?? ""
            };

        }
    }
}
