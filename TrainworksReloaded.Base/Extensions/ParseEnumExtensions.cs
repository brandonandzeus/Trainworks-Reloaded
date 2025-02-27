using Microsoft.Extensions.Configuration;
using ShinyShoe;
using TrainworksReloaded.Base.Localization;
using static BossState;
using static CardEffectData;
using static CardStatistics;
using static CharacterTriggerData;
using static CharacterUI;

namespace TrainworksReloaded.Base.Extensions
{
    public static class ParseEnumExtensions
    {
        public static CardData.CostType? ParseCostType(this IConfigurationSection section)
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
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
            if (string.IsNullOrEmpty(val))
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
            if (string.IsNullOrEmpty(val))
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
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }
            return val.ToLower() switch
            {
                "none" => DLC.None,
                _ => null,
            };
        }

        public static CardInitialKeyboardTarget? ParseKeyboardTarget(
            this IConfigurationSection section
        )
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
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
            if (
                key == null
                && type == null
                && description == null
                && group == null
                && speaker_descriptions == null
                && english == null
                && french == null
                && german == null
                && russian == null
                && portuguese == null
                && chinese == null
                && spanish == null
                && chinese_traditional == null
                && korean == null
                && japanese == null
            )
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
                Japanese = japanese ?? "",
            };
        }

        public static CardStatistics.TrackedValueType? ParseTrackedValueType(
            this IConfigurationSection section
        )
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
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
                "status_effect_count_in_target_room" =>
                    TrackedValueType.StatusEffectCountInTargetRoom,
                "corruption_in_target_room" => TrackedValueType.CorruptionInTargetRoom,
                "turn_count" => TrackedValueType.TurnCount,
                "regal_count_in_target_room" => TrackedValueType.RegalCountInTargetRoom,
                "dragons_hoard_amount" => TrackedValueType.DragonsHoardAmount,
                "moon_phase" => TrackedValueType.MoonPhase,
                "magic_power_in_target_room" => TrackedValueType.MagicPowerInTargetRoom,
                "gold" => TrackedValueType.Gold,
                "status_effect_count_on_last_ability_activator" =>
                    TrackedValueType.StatusEffectCountOnLastAbilityActivator,
                "const_one" => TrackedValueType.ConstOne,
                "pyre_heart_resurrection" => TrackedValueType.PyreHeartResurrection,
                "num_specific_cards_in_deck" => TrackedValueType.NumSpecificCardsInDeck,
                "any_status_effect_stacks_added" => TrackedValueType.AnyStatusEffectStacksAdded,
                "any_status_effect_stacks_removed" => TrackedValueType.AnyStatusEffectStacksRemoved,
                _ => null,
            };
        }

        public static CardStatistics.CardTypeTarget? ParseCardTypeTarget(
            this IConfigurationSection section
        )
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
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

        public static CardStatistics.EntryDuration? ParseEntryDuration(
            this IConfigurationSection section
        )
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }
            return val.ToLower() switch
            {
                "this_turn" => EntryDuration.ThisTurn,
                "this_battle" => EntryDuration.ThisBattle,
                "previous_turn" => EntryDuration.PreviousTurn,
                _ => null,
            };
        }

        public static Team.Type? ParseTeamType(this IConfigurationSection section)
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }
            return val.ToLower() switch
            {
                "none" => Team.Type.None,
                "heroes" => Team.Type.Heroes,
                "monsters" => Team.Type.Monsters,
                "both" => Team.Type.Heroes | Team.Type.Monsters,
                _ => null,
            };
        }

        public static CardTraitData.StackMode? ParseStackMode(this IConfigurationSection section)
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.ToLower() switch
            {
                "none" => CardTraitData.StackMode.None,
                "param_int" => CardTraitData.StackMode.ParamInt,
                "param_int_largest" => CardTraitData.StackMode.ParamIntLargest,
                _ => null,
            };
        }

        public static TargetMode? ParseTargetMode(this IConfigurationSection section)
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.ToLower() switch
            {
                "room" => TargetMode.Room,
                "random_in_room" => TargetMode.RandomInRoom,
                "front_in_room" => TargetMode.FrontInRoom,
                "room_heal_targets" => TargetMode.RoomHealTargets,
                "self" => TargetMode.Self,
                "last_attacked_character" => TargetMode.LastAttackedCharacter,
                "front_with_status" => TargetMode.FrontWithStatus,
                "tower" => TargetMode.Tower,
                "back_in_room" => TargetMode.BackInRoom,
                "drop_target_character" => TargetMode.DropTargetCharacter,
                "draw_pile" => TargetMode.DrawPile,
                "discard" => TargetMode.Discard,
                "exhaust" => TargetMode.Exhaust,
                "eaten" => TargetMode.Eaten,
                "weakest" => TargetMode.Weakest,
                "last_attacker_character" => TargetMode.LastAttackerCharacter,
                "hand" => TargetMode.Hand,
                "last_drawn_card" => TargetMode.LastDrawnCard,
                "last_feeder_character" => TargetMode.LastFeederCharacter,
                "pyre" => TargetMode.Pyre,
                "last_targeted_characters" => TargetMode.LastTargetedCharacters,
                "last_damaged_characters" => TargetMode.LastDamagedCharacters,
                "last_spawned_morsel" => TargetMode.LastSpawnedMorsel,
                "front_in_all_rooms" => TargetMode.FrontInAllRooms,
                "front_in_room_and_room_above" => TargetMode.FrontInRoomAndRoomAbove,
                "weakest_all_rooms" => TargetMode.WeakestAllRooms,
                "strongest_all_rooms" => TargetMode.StrongestAllRooms,
                "last_equipped_character" => TargetMode.LastEquippedCharacter,
                _ => null,
            };
        }

        public static CardEffectData.HealthFilter? ParseHealthFilter(
            this IConfigurationSection section
        )
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.ToLower() switch
            {
                "both" => CardEffectData.HealthFilter.Both,
                "undamaged" => CardEffectData.HealthFilter.Undamaged,
                "damaged" => CardEffectData.HealthFilter.Damaged,
                _ => null,
            };
        }

        public static CardSelectionMode? ParseCardSelectionMode(this IConfigurationSection section)
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.ToLower() switch
            {
                "choose_to_hand" => CardSelectionMode.ChooseToHand,
                "random_to_hand" => CardSelectionMode.RandomToHand,
                "choose_to_deck" => CardSelectionMode.ChooseToDeck,
                "random_to_deck" => CardSelectionMode.RandomToDeck,
                "random_to_room" => CardSelectionMode.RandomToRoom,
                "random_to_hand_with_upgrades" => CardSelectionMode.RandomToHandWithUpgrades,
                "random_to_room_until_capacity_full" =>
                    CardSelectionMode.RandomToRoomUntilCapacityFull,
                _ => null,
            };
        }

        public static Anim? ParseAnim(this IConfigurationSection section)
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.ToLower() switch
            {
                "none" => Anim.None,
                "idle" => Anim.Idle,
                "attack" => Anim.Attack,
                "hit_react" => Anim.HitReact,
                "idle_relentless" => Anim.Idle_Relentless,
                "attack_spell" => Anim.Attack_Spell,
                "death" => Anim.Death,
                "talk" => Anim.Talk,
                "hover" => Anim.Hover,
                _ => null,
            };
        }

        public static Trigger? ParseTrigger(this IConfigurationSection section)
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.ToLower() switch
            {
                "on_death" => Trigger.OnDeath,
                "post_combat" => Trigger.PostCombat,
                "on_spawn" => Trigger.OnSpawn,
                "on_attacking" => Trigger.OnAttacking,
                "on_kill" => Trigger.OnKill,
                "on_any_hero_death_on_floor" => Trigger.OnAnyHeroDeathOnFloor,
                "on_any_monster_death_on_floor" => Trigger.OnAnyMonsterDeathOnFloor,
                "on_heal" => Trigger.OnHeal,
                "on_team_turn_begin" => Trigger.OnTeamTurnBegin,
                "pre_combat" => Trigger.PreCombat,
                "post_ascension" => Trigger.PostAscension,
                "post_combat_healing" => Trigger.PostCombatHealing,
                "on_hit" => Trigger.OnHit,
                "after_spawn_enchant" => Trigger.AfterSpawnEnchant,
                "post_descension" => Trigger.PostDescension,
                "on_any_unit_death_on_floor" => Trigger.OnAnyUnitDeathOnFloor,
                "card_spell_played" => Trigger.CardSpellPlayed,
                "card_monster_played" => Trigger.CardMonsterPlayed,
                "end_turn_pre_hand_discard" => Trigger.EndTurnPreHandDiscard,
                "on_feed" => Trigger.OnFeed,
                "on_eaten" => Trigger.OnEaten,
                "on_turn_begin" => Trigger.OnTurnBegin,
                "on_burnout" => Trigger.OnBurnout,
                "on_spawn_not_from_card" => Trigger.OnSpawnNotFromCard,
                "on_unscaled_spawn" => Trigger.OnUnscaledSpawn,
                "post_attempted_ascension" => Trigger.PostAttemptedAscension,
                "post_attempted_descension" => Trigger.PostAttemptedDescension,
                "on_hatched" => Trigger.OnHatched,
                "card_corrupt_played" => Trigger.CardCorruptPlayed,
                "corruption_added" => Trigger.CorruptionAdded,
                "on_armor_added" => Trigger.OnArmorAdded,
                "on_food_spawn" => Trigger.OnFoodSpawn,
                "card_exhausted" => Trigger.CardExhausted,
                "on_remove_hatch" => Trigger.OnRemoveHatch,
                "on_own_ability_activated" => Trigger.OnOwnAbilityActivated,
                "card_regal_played" => Trigger.CardRegalPlayed,
                "regal_count_added" => Trigger.RegalCountAdded,
                "on_unit_ability_available" => Trigger.OnUnitAbilityAvailable,
                "on_unit_ability_unavailable" => Trigger.OnUnitAbilityUnavailable,
                "on_shift" => Trigger.OnShift,
                "on_equipment_added" => Trigger.OnEquipmentAdded,
                "on_equipment_removed" => Trigger.OnEquipmentRemoved,
                "on_deathwish" => Trigger.OnDeathwish,
                "on_deathwish_lost" => Trigger.OnDeathwishLost,
                "on_valiant" => Trigger.OnValiant,
                "on_encounter_complete" => Trigger.OnEncounterComplete,
                "on_pyregel_added" => Trigger.OnPyregelAdded,
                "on_moon_phase_shift" => Trigger.OnMoonPhaseShift,
                "on_equipment_added_to_ally" => Trigger.OnEquipmentAddedToAlly,
                "on_timebomb" => Trigger.OnTimebomb,
                "on_reanimated" => Trigger.OnReanimated,
                "on_graft_equipment_added" => Trigger.OnGraftEquipmentAdded,
                "on_new_status_effect_added" => Trigger.OnNewStatusEffectAdded,
                "on_queued_status_effect_to_add" => Trigger.OnQueuedStatusEffectToAdd,
                "on_moon_lit" => Trigger.OnMoonLit,
                "on_moon_shade" => Trigger.OnMoonShade,
                "on_moonlit_lost" => Trigger.OnMoonlitLost,
                "on_moonshade_lost" => Trigger.OnMoonshadeLost,
                "on_troop_added" => Trigger.OnTroopAdded,
                "on_troop_removed" => Trigger.OnTroopRemoved,
                "on_train_room_loop" => Trigger.OnTrainRoomLoop,
                "on_status_effect_changed" => Trigger.OnStatusEffectChanged,
                "on_attacking_before_damage" => Trigger.OnAttackingBeforeDamage,
                _ => null,
            };
        }

        public static AttackPhase? ParseAttackPhase(this IConfigurationSection section)
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.ToLower() switch
            {
                "none" => 0, // No flags set
                "casting" => AttackPhase.Casting,
                "relentless" => AttackPhase.Relentless,
                "both" => AttackPhase.Casting | AttackPhase.Relentless, // Both flags set
                _ => null, // Invalid input
            };
        }

        public static CharacterDeathVFX.Type? ParseCharacterDeathType(
            this IConfigurationSection section
        )
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.ToLower() switch
            {
                "none" => CharacterDeathVFX.Type.NONE,
                "normal" => CharacterDeathVFX.Type.Normal,
                "large" => CharacterDeathVFX.Type.Large,
                "boss" => CharacterDeathVFX.Type.Boss,
                _ => null, // Invalid input
            };
        }

        public static TitanAffinity? ParseTitanAffinity(this IConfigurationSection section)
        {
            var val = section.Value;
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return val.ToLower() switch
            {
                "none" => TitanAffinity.None,
                "entropy" => TitanAffinity.Entropy,
                "savagery" => TitanAffinity.Savagery,
                "dominion" => TitanAffinity.Dominion,
                _ => null, // Invalid input
            };
        }
    }
}
