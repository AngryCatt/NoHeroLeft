using System;
using UnityEngine;

namespace HeroLeft.BattleLogic {
    public class SpecClassSpellPassive : MonoBehaviour {
        public static SpecClassSpellPassive spec;
        public specClass[] specClasses;

        private void Awake()
        {
            spec = this;
        }


        [Serializable]
        public class specClass {
            public classType @class;
            public int classParts = 2;
            [Range(0, 100)]
            public int Chanse;
            public Spell lowPerk;
            public Spell mainPerk;
        }

        public void UseSpell(Spell spell, Unit target, Unit user)
        {
            int q = 0;
            for (int i = 0; i < user.unitObject.Spells.Length; i++)
            {
                if (user.unitObject.Spells[i].spellClass == spell.spellClass)
                {
                    q++;
                }
            }
            foreach (specClass spec in specClasses)
            {
                if (spec.@class == spell.spellClass)
                {
                    if (spell.Perk == 0 || spell.Perk == 1)
                        if (spec.lowPerk.spellTarget == Spell.SpellTarget.Alies ^ !target.Alien)
                            spec.lowPerk.Execute(null, target, true, -2);

                    if (spell.Perk == 0 || spell.Perk == 2)
                        if (q >= spec.classParts)
                        {
                            if (Randomize.Random(spec.Chanse))
                            {
                                if (spec.mainPerk.spellTarget == Spell.SpellTarget.Alies ^ !target.Alien)
                                    spec.mainPerk.Execute(null, target, true, -2);
                            }
                        }
                }
            }
        }
    }

    public enum classType : uint {
        none = 0,
        IceSchool,
        FireSchool,
        EarthSchool,
        AirSchool
    }
}
