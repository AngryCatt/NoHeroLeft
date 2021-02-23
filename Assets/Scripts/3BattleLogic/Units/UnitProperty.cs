using HeroLeft.Interfaces;
using System;

namespace HeroLeft.BattleLogic {
    [Serializable]
    public class UnitProperty : ICloneable {
        public SafeFloat Hp;
        public SafeFloat Energy;
        public SafeFloat Mana;

        public SafeFloat HpRegen;
        public SafeFloat Damage;

        [Range(0, 100)] public SafeInt DamageResist;
        [Range(0, 100)] public SafeInt MagicResist;
        [Range(0, 100)] public SafeInt PureResist;

        [Range(0, 100)] public SafeInt FireResist;
        [Range(0, 100)] public SafeInt IceResist;
        [Range(0, 100)] public SafeInt NatureResist;
        [Range(0, 100)] public SafeInt DarkResist;
        [Range(0, 100)] public SafeInt BloodResist;
        [Range(0, 100)] public SafeInt LightResist;
        [Range(0, 100)] public SafeInt PoisonResist;

        public SafeFloat CriticalDamage;

        public SafeFloat Armor;
        public SafeFloat BlockDamage;

        [Range(0, 100)] public SafeInt Evasion;

        [Range(0, 100)] public SafeInt AttackCritChanse;
        [Range(0, 100)] public SafeInt MagicCritChanse;
        [Range(0, 100)] public SafeInt AbsoluteBlockChanse;

        [Range(0, 100)] public SafeInt BlockChanse;
        [Range(0, 100)] public SafeInt ParryChanse;

        public SafeFloat DirectSpellDamage;
        public SafeFloat FieldSpellDamage;

        public object Clone() {
            return new UnitProperty { Hp = Hp, Damage = Damage, 
            Evasion = Evasion, HpRegen = HpRegen, Energy = Energy,
            Armor = Armor, DamageResist = DamageResist, Mana = Mana,
            AttackCritChanse = AttackCritChanse, BlockChanse = BlockChanse,
            BlockDamage = BlockDamage, BloodResist = BloodResist,
            CriticalDamage = CriticalDamage, DarkResist = DarkResist,
            DirectSpellDamage = DirectSpellDamage, FireResist = FireResist,
            FieldSpellDamage = FieldSpellDamage, IceResist = IceResist,
            LightResist = LightResist, MagicCritChanse = MagicCritChanse,
            NatureResist = NatureResist, ParryChanse = ParryChanse,
            PoisonResist = PoisonResist, PureResist = PureResist,
            MagicResist = MagicResist, AbsoluteBlockChanse = AbsoluteBlockChanse };
        }

        public static UnitProperty operator +(UnitProperty a, UnitProperty b) => new UnitProperty()
        {
            AbsoluteBlockChanse = a.AbsoluteBlockChanse + b.AbsoluteBlockChanse,
            Armor = a.Armor + b.Armor,
            AttackCritChanse = a.AttackCritChanse + b.AttackCritChanse,
            BlockChanse = a.BlockChanse + b.BlockChanse,
            BlockDamage = a.BlockDamage + b.BlockDamage,
            BloodResist = a.BloodResist + b.BloodResist,
            CriticalDamage = a.CriticalDamage + b.CriticalDamage,
            Damage = a.Damage + b.Damage,
            DamageResist = a.DamageResist + b.DamageResist,
            DarkResist = a.DarkResist + b.DarkResist,
            DirectSpellDamage = a.DirectSpellDamage + b.DirectSpellDamage,
            Evasion = a.Evasion + b.Evasion,
            FieldSpellDamage = a.FieldSpellDamage + b.FieldSpellDamage,
            FireResist = a.FireResist + b.FireResist,
            Hp = a.Hp + b.Hp,
            HpRegen = a.HpRegen + b.HpRegen,
            IceResist = a.IceResist + b.IceResist,
            LightResist = a.LightResist + b.LightResist,
            MagicCritChanse = a.MagicCritChanse + b.MagicCritChanse,
            MagicResist = a.MagicResist + b.MagicResist,
            NatureResist = a.NatureResist + b.NatureResist,
            ParryChanse = a.ParryChanse + b.ParryChanse,
            PoisonResist = a.PoisonResist + b.PoisonResist,
            PureResist = a.PureResist + b.PureResist,
            Energy = a.Energy + b.Energy,
            Mana = a.Mana + b.Mana,
        };

        public static UnitProperty operator -(UnitProperty a, UnitProperty b) => new UnitProperty()
        {
            AbsoluteBlockChanse = a.AbsoluteBlockChanse - b.AbsoluteBlockChanse,
            Armor = a.Armor - b.Armor,
            AttackCritChanse = a.AttackCritChanse - b.AttackCritChanse,
            BlockChanse = a.BlockChanse - b.BlockChanse,
            BlockDamage = a.BlockDamage - b.BlockDamage,
            BloodResist = a.BloodResist - b.BloodResist,
            CriticalDamage = a.CriticalDamage - b.CriticalDamage,
            Damage = a.Damage - b.Damage,
            DamageResist = a.DamageResist - b.DamageResist,
            DarkResist = a.DarkResist - b.DarkResist,
            DirectSpellDamage = a.DirectSpellDamage - b.DirectSpellDamage,
            Evasion = a.Evasion - b.Evasion,
            FieldSpellDamage = a.FieldSpellDamage - b.FieldSpellDamage,
            FireResist = a.FireResist - b.FireResist,
            Hp = a.Hp - b.Hp,
            HpRegen = a.HpRegen - b.HpRegen,
            IceResist = a.IceResist - b.IceResist,
            LightResist = a.LightResist - b.LightResist,
            MagicCritChanse = a.MagicCritChanse - b.MagicCritChanse,
            MagicResist = a.MagicResist - b.MagicResist,
            NatureResist = a.NatureResist - b.NatureResist,
            ParryChanse = a.ParryChanse - b.ParryChanse,
            PoisonResist = a.PoisonResist - b.PoisonResist,
            PureResist = a.PureResist - b.PureResist,
            Energy = a.Energy - b.Energy,
            Mana = a.Mana - b.Mana,
        };
    }
}
