using HarmonyLib;
using Malee;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Interfaces;
using UnityEngine;

namespace TrainworksReloaded.Base
{
    public class CustomCardDataRegister : Dictionary<string, CardData>, IRegister<CardData>
    {
        public CardPool CustomCardPool;
        public ReorderableArray<CardData> CardPoolBacking;
        public CustomCardDataRegister()
        {
            CustomCardPool = ScriptableObject.CreateInstance<CardPool>();
            CardPoolBacking = (ReorderableArray<CardData>)AccessTools.Field(typeof(CardPool), "cardDataList").GetValue(CustomCardPool);
        }
        public void Register(string key, CardData item)
        {
            CardPoolBacking.Add(item);
            this.Add(key, item);
        }
    }
}
