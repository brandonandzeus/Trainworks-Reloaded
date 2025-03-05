using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TrainworksReloaded.Base.Character;
using TrainworksReloaded.Base.Effect;
using TrainworksReloaded.Base.Extensions;
using TrainworksReloaded.Core.Interfaces;

namespace TrainworksReloaded.Base.CardUpgrade
{
    public class CardUpgradeFinalizer : IDataFinalizer
    {
        private readonly IModLogger<CardUpgradeFinalizer> logger;
        private readonly ICache<IDefinition<CardUpgradeData>> cache;
        private readonly IRegister<CharacterTriggerData> triggerRegister;
        private readonly IRegister<RoomModifierData> roomModifierRegister;

        public CardUpgradeFinalizer(
            IModLogger<CardUpgradeFinalizer> logger,
            ICache<IDefinition<CardUpgradeData>> cache,
            IRegister<CharacterTriggerData> triggerRegister,
            IRegister<RoomModifierData> roomModifierRegister
        )
        {
            this.logger = logger;
            this.cache = cache;
            this.triggerRegister = triggerRegister;
            this.roomModifierRegister = roomModifierRegister;
        }

        public void FinalizeData()
        {
            foreach (var definition in cache.GetCacheItems())
            {
                FinalizeCardUpgradeData(definition);
            }
            cache.Clear();
        }

        private void FinalizeCardUpgradeData(IDefinition<CardUpgradeData> definition)
        {
            var configuration = definition.Configuration;
            var data = definition.Data;
            var key = definition.Key;

            logger.Log(Core.Interfaces.LogLevel.Info, $"Finalizing Upgrade {data.name}... ");

            //handle triggers
            var triggerUpgrades = new List<CharacterTriggerData>();
            var triggerUpgradesConfig = configuration.GetSection("trigger_upgrades").GetChildren();
            foreach (var triggerUpgrade in triggerUpgradesConfig)
            {
                if (triggerUpgrade == null)
                {
                    continue;
                }
                var idConfig = triggerUpgrade.GetSection("id").Value;
                if (idConfig == null)
                {
                    continue;
                }

                var id = idConfig.ToId(key, "CTrigger");
                if (triggerRegister.TryLookupId(id, out var card, out var _))
                {
                    triggerUpgrades.Add(card);
                }
            }
            if (triggerUpgrades.Count != 0)
                AccessTools
                    .Field(typeof(CardUpgradeData), "triggerUpgrades")
                    .SetValue(data, triggerUpgrades);

            //handle roomModifiers
            var roomModifierUpgrades = new List<RoomModifierData>();
            var roomModifierUpgradesConfig = configuration
                .GetSection("room_modifier_upgrades")
                .GetChildren();
            foreach (var roomModifier in roomModifierUpgradesConfig)
            {
                if (roomModifier == null)
                {
                    continue;
                }
                var idConfig = roomModifier.GetSection("id").Value;
                if (idConfig == null)
                {
                    continue;
                }
                var id = idConfig.ToId(key, "RoomModifier");
                if (roomModifierRegister.TryLookupId(id, out var card, out var _))
                {
                    roomModifierUpgrades.Add(card);
                }
            }

            if (roomModifierUpgrades.Count != 0)
                AccessTools
                    .Field(typeof(CardUpgradeData), "roomModifierUpgrades")
                    .SetValue(data, roomModifierUpgrades);

            //status effects
            var status_effects = new List<StatusEffectStackData>();
            AccessTools
                .Field(typeof(CardUpgradeData), "statusEffectUpgrades")
                .SetValue(data, status_effects);

            //remove trait
            var removeTraitUpgrades = new List<string>();
            AccessTools
                .Field(typeof(CardUpgradeData), "removeTraitUpgrades")
                .SetValue(data, removeTraitUpgrades);

            //trait data
            var traitDataUpgrades = new List<CardTraitData>();
            AccessTools
                .Field(typeof(CardUpgradeData), "traitDataUpgrades")
                .SetValue(data, traitDataUpgrades);
        }
    }
}
