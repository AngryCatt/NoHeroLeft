using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroLeft.BattleLogic {
    public static class BattleConstants
    {
        public const float EnergyCost = 10;
        public const float ArmorDamageReduction = 10;

        public static void CalculateArmor(ref float damage, float armor)
        {
            damage -= damage / 100 * armor;
        }
    }
}
