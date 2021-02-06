using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HeroLeft.BattleLogic {

    [CreateAssetMenu(menuName = "Phys Buff", fileName = "New Buff", order = 51)]
    public class Buff : ScriptableObject {
        public Effect effect;
    }
}
