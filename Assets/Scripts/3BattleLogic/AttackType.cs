using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HeroLeft.BattleLogic
{
    [CreateAssetMenu(menuName = "AttackType", fileName = "New Type", order = 51)]
    public class AttackType : ScriptableObject, ICloneable, IEquatable<AttackType>
    {
        public string TypeName = "simple";
        public int damageIndicator = 0;
        public Spell AttackPerk;
        public bool PartSpell = false;
        public bool OnHero = false;
        public SpellPart[] spellParts;
        [HideInInspector] public int part = 0;
        [HideInInspector] public Logic target;
        public Spell.SplashType splashType;

        public SpellPart AvailableSpell
        {
            get
            {

                for (int i = PartSpell ? part : spellParts.Length - 1; i >= 0; i--)
                {
                    if (spellParts[i].Available(this)) return spellParts[i];
                }
                return spellParts[0];
            }
        }
        public object Clone()
        {
            SpellPart[] spells = new SpellPart[spellParts.Length];

            for(int i = 0; i < spells.Length; i++)
            {
                spells[i] = (SpellPart)spellParts[i].Clone();
            }

            return new AttackType
            {
                splashType = splashType,
                TypeName = TypeName,
                damageIndicator = damageIndicator,
                spellParts = spells,
                AttackPerk = AttackPerk,
                part = 0,
                PartSpell = PartSpell,
                OnHero = OnHero,
                
            };
        }

        public void Reload()
        {
            foreach(SpellPart sp in spellParts)
            {
                sp.Reload();
            }
        }

        public void ApplyUnit(Unit unit)
        {
            if(AttackPerk != null)
            AttackPerk.unitEvents.MyUnit = unit;

            for (int i = 0; i < spellParts.Length; i++)
            {
                spellParts[i].spell.unitEvents.MyUnit = unit;
            }
        }

        public bool Equals(AttackType other)
        {
            return TypeName == other.TypeName;
        }
    }

    [Serializable]
    public class SpellPart : ICloneable
    {
        public int needPoints;
        public int reload = 0;
        public Spell spell;
        public pointTypes pointType;

        public bool Available(AttackType attackType)
        {
            Logic logic = attackType.OnHero ? BattleControll.heroLogic.unitlogic : attackType.target;
            if (logic != null && reload == 0)
            {

                switch (pointType)
                {
                    case pointTypes.stacks:
                        Effect eff = logic.UnderSpellEff(attackType.AttackPerk);

                        if (eff != null && eff.stacks >= needPoints)
                        {
                            return true;
                        }
                        break;
                    case pointTypes.points:
                        return attackType.part >= needPoints;
                }
            }
            return false;
        }

        public void Reload()
        {
            if(reload > 0)
            reload--;
        }

        public void GoReload()
        {
            reload = spell.ReloadTurns;
        }

        public enum pointTypes
        {
            points,
            stacks,
        }

        public object Clone()
        {
            return new SpellPart
            {
                needPoints = needPoints,
                pointType = pointType,
                reload = reload,
                spell = (Spell)spell.Clone(),
            };
        }
    }
}
