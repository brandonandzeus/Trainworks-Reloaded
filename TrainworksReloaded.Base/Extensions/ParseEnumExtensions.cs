using HarmonyLib;
using Microsoft.Extensions.Configuration;
using ShinyShoe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TrainworksReloaded.Base.Localization;
using static CardStatistics;
using static PlayerManager;

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
            var key = section.GetSection("id").Value;
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

        public static CardStatistics.TrackedValueType? ParseTrackedValueType(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            return val.ToLower() switch
            {
                "subtype_in_deck" => TrackedValueType.SubtypeInDeck,
                "subtype_in_discard_pile" => TrackedValueType.SubtypeInDiscardPile,
                "subtype_in_exhaust_pile" => TrackedValueType.SubtypeInExhaustPile,
                "subtype_in_draw_pile" => TrackedValueType.SubtypeInDrawPile,
                "subtype_in_eaten_pile" => TrackedValueType.SubtypeInEatenPile,
                "type_in_deck" => TrackedValueType.TypeInDeck,
                "type_in_discard_pile" => TrackedValueType.TypeInDiscardPile,
                "type_in_exhaust_pile" => TrackedValueType.TypeInExhaustPile,
                "type_in_draw_pile" => TrackedValueType.TypeInDrawPile,
                "type_in_eaten_pile" => TrackedValueType.TypeInEatenPile,
                "played_cost" => TrackedValueType.PlayedCost,
                "unmodified_played_cost" => TrackedValueType.UnmodifiedPlayedCost,
                "heroes_killed" => TrackedValueType.HeroesKilled,
                "spawned_monster_deaths" => TrackedValueType.SpawnedMonsterDeaths,
                "times_discarded" => TrackedValueType.TimesDiscarded,
                "times_played" => TrackedValueType.TimesPlayed,
                "times_drawn" => TrackedValueType.TimesDrawn,
                "times_exhausted" => TrackedValueType.TimesExhausted,
                "last_sacrificed_monster_stats" => TrackedValueType.LastSacrificedMonsterStats,
                "any_hero_killed" => TrackedValueType.AnyHeroKilled,
                "any_monster_death" => TrackedValueType.AnyMonsterDeath,
                "any_monster_spawned" => TrackedValueType.AnyMonsterSpawned,
                "any_discarded" => TrackedValueType.AnyDiscarded,
                "any_card_played" => TrackedValueType.AnyCardPlayed,
                "any_card_drawn" => TrackedValueType.AnyCardDrawn,
                "any_exhausted" => TrackedValueType.AnyExhausted,
                "any_monster_spawned_top_floor" => TrackedValueType.AnyMonsterSpawnedTopFloor,
                "monster_subtype_played" => TrackedValueType.MonsterSubtypePlayed,
                "status_effect_count_in_target_room" => TrackedValueType.StatusEffectCountInTargetRoom,
                "corruption_in_target_room" => TrackedValueType.CorruptionInTargetRoom,
                "turn_count" => TrackedValueType.TurnCount,
                "regal_count_in_target_room" => TrackedValueType.RegalCountInTargetRoom,
                "dragons_hoard_amount" => TrackedValueType.DragonsHoardAmount,
                "moon_phase" => TrackedValueType.MoonPhase,
                "magic_power_in_target_room" => TrackedValueType.MagicPowerInTargetRoom,
                "gold" => TrackedValueType.Gold,
                "status_effect_count_on_last_ability_activator" => TrackedValueType.StatusEffectCountOnLastAbilityActivator,
                "const_one" => TrackedValueType.ConstOne,
                "pyre_heart_resurrection" => TrackedValueType.PyreHeartResurrection,
                "num_specific_cards_in_deck" => TrackedValueType.NumSpecificCardsInDeck,
                "any_status_effect_stacks_added" => TrackedValueType.AnyStatusEffectStacksAdded,
                "any_status_effect_stacks_removed" => TrackedValueType.AnyStatusEffectStacksRemoved,
                _ => null,
            };
        }
        public static CardStatistics.CardTypeTarget? ParseCardTypeTarget(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            return val.ToLower() switch
            {
                "any" => CardTypeTarget.Any,
                "spell" => CardTypeTarget.Spell,
                "monster" => CardTypeTarget.Monster,
                "blight" => CardTypeTarget.Blight,
                "junk" => CardTypeTarget.Junk,
                "equipment" => CardTypeTarget.Equipment,
                "train_room_attachment" => CardTypeTarget.TrainRoomAttachment,
                _ => null,
            };
        }
        public static CardStatistics.EntryDuration? ParseEntryDuration(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            return val.ToLower() switch
            {
                "this_turn" => EntryDuration.ThisTurn,
                "this_battle" => EntryDuration.ThisBattle,
                "previous_turn" => EntryDuration.PreviousTurn,
                _ => null
            };
        }
        public static Team.Type? ParseTeamType(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }
            return val.ToLower() switch
            {
                "none" => Team.Type.None,
                "heroes" => Team.Type.Heroes,
                "monsters" => Team.Type.Monsters,
                _ => null
            };
        }
        public static CardTraitData.StackMode? ParseStackMode(this IConfigurationSection section)
        {
            var val = section.Value;
            if (val == null)
            {
                return null;
            }

            return val.ToLower() switch
            {
                "none" => CardTraitData.StackMode.None,
                "param_int" => CardTraitData.StackMode.ParamInt,
                "param_int_largest" => CardTraitData.StackMode.ParamIntLargest,
                 _ => null
            };
        }
    }
}
