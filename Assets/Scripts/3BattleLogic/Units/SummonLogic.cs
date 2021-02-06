using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HeroLeft.BattleLogic {
    public class SummonLogic : MonoBehaviour {
        public int Duration;
        public Spell spell;

        public void Execute(Effect.actionCall turnPos)
        {
            for(int i = 0; i < spell.effects.Length; i++)
            {
                if(spell.effects[i].ActionCall == turnPos)
                {
                    Unit[] targets = spell.GetTargets(null, spell.splashType, spell.spellTarget);
                    foreach(Unit unit in targets)
                    {
                        spell.Execute(null, unit);
                    }
                }
            }
            Duration--;
        }


    }
}
