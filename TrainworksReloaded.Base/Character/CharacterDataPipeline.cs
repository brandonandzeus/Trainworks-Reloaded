using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using ShinyShoe;
using SimpleInjector;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Base.Localization;
using TrainworksReloaded.Core.Extensions;
using TrainworksReloaded.Core.Impl;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TrainworksReloaded.Base.Character
{
    public class CharacterDataPipeline : IDataPipeline<IRegister<CharacterData>, CharacterData>
    {
        private readonly PluginAtlas atlas;
        private readonly IModLogger<CharacterDataPipeline> logger;
        private readonly IRegister<LocalizationTerm> termRegister;
        private readonly IGuidProvider guidProvider;

        public CharacterDataPipeline(
            PluginAtlas atlas,
            IModLogger<CharacterDataPipeline> logger,
            IRegister<LocalizationTerm> termRegister,
            IGuidProvider guidProvider
        )
        {
            this.atlas = atlas;
            this.logger = logger;
            this.termRegister = termRegister;
            this.guidProvider = guidProvider;
        }

        public List<IDefinition<CharacterData>> Run(IRegister<CharacterData> service)
        {
            // We load all cards and then finalize them to avoid dependency loops
            var processList = new List<IDefinition<CharacterData>>();
            foreach (var config in atlas.PluginDefinitions)
            {
                processList.AddRange(
                    LoadCharacters(service, config.Key, config.Value.Configuration)
                );
            }
            return processList;
        }

        /// <summary>
        /// Loads the Card Definitions in
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <param name="pluginConfig"></param>
        /// <returns></returns>
        private List<CharacterDataDefinition> LoadCharacters(
            IRegister<CharacterData> service,
            string key,
            IConfiguration pluginConfig
        )
        {
            var processList = new List<CharacterDataDefinition>();
            foreach (var child in pluginConfig.GetSection("characters").GetChildren())
            {
                var data = LoadCharacterConfiguration(service, key, child);
                if (data != null)
                {
                    processList.Add(data);
                }
            }
            return processList;
        }

        private CharacterDataDefinition? LoadCharacterConfiguration(
            IRegister<CharacterData> service,
            string key,
            IConfiguration configuration
        )
        {
            var id = configuration.GetSection("id").ParseString();
            if (id == null)
            {
                return null;
            }

            var name = key.GetId("Character", id);
            var namekey = $"CharacterData_nameKey-{name}";
            var checkOverride = configuration.GetSection("override").ParseBool() ?? false;

            string guid;
            if (checkOverride && service.TryLookupName(id, out CharacterData? data, out var _))
            {
                logger.Log(Core.Interfaces.LogLevel.Info, $"Overriding Character {id}... ");
                namekey = data.GetNameKey();
                guid = data.GetID();
            }
            else
            {
                data = ScriptableObject.CreateInstance<CharacterData>();
                data.name = name;
                guid = guidProvider.GetGuidDeterministic(name).ToString();
            }

            //handle id
            AccessTools.Field(typeof(CharacterData), "id").SetValue(data, guid);

            //handle names
            var localizationNameTerm = configuration.GetSection("names").ParseLocalizationTerm();
            if (localizationNameTerm != null)
            {
                AccessTools.Field(typeof(CharacterData), "nameKey").SetValue(data, namekey);
                localizationNameTerm.Key = namekey;
                termRegister.Register(namekey, localizationNameTerm);
            }

            //bools
            var hideInLogbook =
                checkOverride
                && (bool)AccessTools.Field(typeof(CharacterData), "hideInLogbook").GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "hideInLogbook")
                .SetValue(
                    data,
                    configuration.GetSection("hide_in_logbook").ParseBool() ?? hideInLogbook
                );

            var blockVisualSizeIncrease =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CharacterData), "blockVisualSizeIncrease")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "blockVisualSizeIncrease")
                .SetValue(
                    data,
                    configuration.GetSection("block_size_increase").ParseBool()
                        ?? blockVisualSizeIncrease
                );

            var canBeHealed =
                checkOverride
                && (bool)AccessTools.Field(typeof(CharacterData), "canBeHealed").GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "canBeHealed")
                .SetValue(
                    data,
                    configuration.GetSection("can_be_healed").ParseBool() ?? canBeHealed
                );

            var isOuterTrainBoss =
                checkOverride
                && (bool)
                    AccessTools.Field(typeof(CharacterData), "isOuterTrainBoss").GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "isOuterTrainBoss")
                .SetValue(
                    data,
                    configuration.GetSection("is_outer_train_boss").ParseBool() ?? isOuterTrainBoss
                );

            var isMiniboss =
                checkOverride
                && (bool)AccessTools.Field(typeof(CharacterData), "isMiniboss").GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "isMiniboss")
                .SetValue(data, configuration.GetSection("is_mini_boss").ParseBool() ?? isMiniboss);

            var canAttack =
                checkOverride
                && (bool)AccessTools.Field(typeof(CharacterData), "canAttack").GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "canAttack")
                .SetValue(data, configuration.GetSection("can_attack").ParseBool() ?? canAttack);

            var ascendsTrainAutomatically =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CharacterData), "ascendsTrainAutomatically")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "ascendsTrainAutomatically")
                .SetValue(
                    data,
                    configuration.GetSection("ascends_train_automatically").ParseBool()
                        ?? ascendsTrainAutomatically
                );

            var loopsBetweenTrainFloors =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CharacterData), "loopsBetweenTrainFloors")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "loopsBetweenTrainFloors")
                .SetValue(
                    data,
                    configuration.GetSection("loops_between_floors").ParseBool()
                        ?? loopsBetweenTrainFloors
                );

            var attackTeleportsToDefender =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CharacterData), "attackTeleportsToDefender")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "attackTeleportsToDefender")
                .SetValue(
                    data,
                    configuration.GetSection("attack_teleports_to_defender").ParseBool()
                        ?? attackTeleportsToDefender
                );

            var deathSlidesBackwards =
                checkOverride
                && (bool)
                    AccessTools.Field(typeof(CharacterData), "deathSlidesBackwards").GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "deathSlidesBackwards")
                .SetValue(
                    data,
                    configuration.GetSection("death_slides_backwards").ParseBool()
                        ?? deathSlidesBackwards
                );

            var chosenVariant =
                checkOverride
                && (bool)AccessTools.Field(typeof(CharacterData), "chosenVariant").GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "chosenVariant")
                .SetValue(
                    data,
                    configuration.GetSection("chosen_variant").ParseBool() ?? chosenVariant
                );

            var removeTriggersOnRelentlessChange =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CharacterData), "removeTriggersOnRelentlessChange")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "removeTriggersOnRelentlessChange")
                .SetValue(
                    data,
                    configuration.GetSection("remove_triggers_on_relentless").ParseBool()
                        ?? removeTriggersOnRelentlessChange
                );

            var isPyreHeart =
                checkOverride
                && (bool)AccessTools.Field(typeof(CharacterData), "isPyreHeart").GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "isPyreHeart")
                .SetValue(data, configuration.GetSection("is_pyre").ParseBool() ?? isPyreHeart);

            var disableInDailyChallenges =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CharacterData), "disableInDailyChallenges")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "disableInDailyChallenges")
                .SetValue(
                    data,
                    configuration.GetSection("disable_in_daily_challenges").ParseBool()
                        ?? disableInDailyChallenges
                );

            var canOnlyEquipGraftedEquipment =
                checkOverride
                && (bool)
                    AccessTools
                        .Field(typeof(CharacterData), "canOnlyEquipGraftedEquipment")
                        .GetValue(data);
            AccessTools
                .Field(typeof(CharacterData), "canOnlyEquipGraftedEquipment")
                .SetValue(
                    data,
                    configuration.GetSection("can_equip_only_grafted").ParseBool()
                        ?? canOnlyEquipGraftedEquipment
                );

            //int
            var size = checkOverride
                ? (int)AccessTools.Field(typeof(CharacterData), "size").GetValue(data)
                : 2;
            AccessTools
                .Field(typeof(CharacterData), "size")
                .SetValue(data, configuration.GetSection("size").ParseInt() ?? size);

            var health = checkOverride
                ? (int)AccessTools.Field(typeof(CharacterData), "health").GetValue(data)
                : 0;
            AccessTools
                .Field(typeof(CharacterData), "health")
                .SetValue(data, configuration.GetSection("health").ParseInt() ?? health);

            var attackDamage = checkOverride
                ? (int)AccessTools.Field(typeof(CharacterData), "attackDamage").GetValue(data)
                : 0;
            AccessTools
                .Field(typeof(CharacterData), "attackDamage")
                .SetValue(
                    data,
                    configuration.GetSection("attack_damage").ParseInt() ?? attackDamage
                );

            var equipmentLimit = checkOverride
                ? (int)AccessTools.Field(typeof(CharacterData), "equipmentLimit").GetValue(data)
                : 1;
            AccessTools
                .Field(typeof(CharacterData), "equipmentLimit")
                .SetValue(
                    data,
                    configuration.GetSection("equip_limit").ParseInt() ?? equipmentLimit
                );

            //attack phase
            var validBossAttackPhase = checkOverride
                ? (BossState.AttackPhase)
                    AccessTools.Field(typeof(CharacterData), "validBossAttackPhase").GetValue(data)
                : BossState.AttackPhase.Relentless;
            AccessTools
                .Field(typeof(CharacterData), "validBossAttackPhase")
                .SetValue(
                    data,
                    configuration.GetSection("valid_attack_phase").ParseAttackPhase()
                        ?? validBossAttackPhase
                );

            //death type
            var deathType = checkOverride
                ? (CharacterDeathVFX.Type)
                    AccessTools.Field(typeof(CharacterData), "deathType").GetValue(data)
                : CharacterDeathVFX.Type.Normal;
            AccessTools
                .Field(typeof(CharacterData), "deathType")
                .SetValue(
                    data,
                    configuration.GetSection("death_type").ParseCharacterDeathType() ?? deathType
                );

            //death type
            var bossTitanAffinity = checkOverride
                ? (TitanAffinity)
                    AccessTools.Field(typeof(CharacterData), "bossTitanAffinity").GetValue(data)
                : TitanAffinity.None;
            AccessTools
                .Field(typeof(CharacterData), "bossTitanAffinity")
                .SetValue(
                    data,
                    configuration.GetSection("titan_affinity").ParseTitanAffinity()
                        ?? bossTitanAffinity
                );

            //subtypes
            var subtypes =
                (List<string>)
                    AccessTools.Field(typeof(CharacterData), "subtypeKeys").GetValue(data);
            if (subtypes == null)
            {
                subtypes = new List<string>();
                AccessTools.Field(typeof(CharacterData), "subtypeKeys").SetValue(data, subtypes);
            }

            if (checkOverride)
            {
                subtypes.Clear();
            }

            foreach (var config in configuration.GetSection("subtypes").GetChildren().ToList())
            {
                var str = config.ParseString();
                if (str != null)
                    subtypes.Add(str);
            }

            //status effect immunities
            var statusEffectImmunities = (string[])
                AccessTools.Field(typeof(CharacterData), "statusEffectImmunities").GetValue(data);
            if (statusEffectImmunities == null)
            {
                statusEffectImmunities = [];
            }
            var statusEffectImmunitiesList = statusEffectImmunities.ToList();

            if (checkOverride)
            {
                statusEffectImmunitiesList.Clear();
            }

            foreach (
                var config in configuration.GetSection("status_effect_immunities").GetChildren()
            )
            {
                var str = config.ParseString();
                if (str != null)
                    statusEffectImmunitiesList.Add(str);
            }
            AccessTools
                .Field(typeof(CharacterData), "statusEffectImmunities")
                .SetValue(data, statusEffectImmunitiesList.ToArray());

            //status
            var startingStatusEffects = new List<StatusEffectStackData>();
            if (!checkOverride)
            {
                var startingStatusEffects2 = (StatusEffectStackData[])
                    AccessTools
                        .Field(typeof(CharacterData), "startingStatusEffects")
                        .GetValue(data);
                if (startingStatusEffects2 != null)
                    startingStatusEffects.AddRange(startingStatusEffects2);
            }
            startingStatusEffects.AddRange(
                configuration
                    .GetSection("starting_status_effects")
                    .GetChildren()
                    .Select(xs => new StatusEffectStackData()
                    {
                        statusId = xs.GetSection("status").ParseString() ?? "",
                        count = xs.GetSection("count").ParseInt() ?? 0,
                    })
            );
            AccessTools
                .Field(typeof(CharacterData), "startingStatusEffects")
                .SetValue(data, startingStatusEffects.ToArray());

            //endless baseline stats

            var endlessBaselineStats = checkOverride
                ? (EndlessBaselineStats)
                    AccessTools.Field(typeof(CharacterData), "endlessBaselineStats").GetValue(data)
                : new EndlessBaselineStats();
            var endlessHealth = configuration
                .GetSection("endless_stats")
                .GetSection("health")
                .ParseInt();
            if (endlessHealth != null)
            {
                AccessTools
                    .Field(typeof(EndlessBaselineStats), "endlessHealth")
                    .SetValue(endlessBaselineStats, endlessHealth);
            }
            var endlessAttack = configuration
                .GetSection("endless_stats")
                .GetSection("attack")
                .ParseInt();
            if (endlessHealth != null)
            {
                AccessTools
                    .Field(typeof(EndlessBaselineStats), "endlessAttack")
                    .SetValue(endlessBaselineStats, endlessAttack);
            }
            AccessTools
                .Field(typeof(CharacterData), "endlessBaselineStats")
                .SetValue(data, endlessBaselineStats);

            //register before filling in data using
            if (!checkOverride)
                service.Register(name, data);

            return new CharacterDataDefinition(key, data, configuration, checkOverride);
        }
    }
}
