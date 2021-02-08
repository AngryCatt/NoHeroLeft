using HeroLeft.Interfaces;
using System;

namespace HeroLeft.BattleLogic {
    [System.Serializable]
    public class UnitProperty : ICloneable {
        public SafeFloat Hp;
        public SafeFloat HpRegen;
        public SafeFloat Damage;
        public SafeFloat DamageResist;
        public SafeFloat MagicResist;
        public float Armor;
        [Range(0, 100)] public SafeInt Evasion;

        public object Clone() {
            return new UnitProperty { Hp = Hp, Damage = Damage, 
            Evasion = Evasion, HpRegen = HpRegen,
            Armor = Armor, DamageResist = DamageResist,
            MagicResist = MagicResist};
        }
    }
}
