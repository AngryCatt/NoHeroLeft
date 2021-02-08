using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HeroLeft.BattleLogic
{
    public class SummonLogic : MonoBehaviour
    {
        public int Duration;
        public Effect.actionCall CallTime;
        public Spell spell;

        public void Execute(Effect.actionCall turnPos)
        {
            if (CallTime.HasFlag(turnPos))
            {
                spell.Execute(null, null);

                Duration--;
            }
        }


    }
}
