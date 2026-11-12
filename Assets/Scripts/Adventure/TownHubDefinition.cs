using System;
using System.Collections.Generic;
using UnityEngine;
namespace StepQuest.Adventure
{
    [Serializable] public class TownShopOffer
    {
        public Equipment equipment;
        [Min(1)] public int copperPrice = 15;
    }
    [CreateAssetMenu(menuName = "StepQuest/Town hub")]
    public class TownHubDefinition : ScriptableObject
    {
        [Min(1)] public int randomBattleStepCost = 100;
        public List<string> randomEncounterStageIds = new List<string> { "1.1" };
        public List<TownShopOffer> shop = new List<TownShopOffer>();
    }
}
