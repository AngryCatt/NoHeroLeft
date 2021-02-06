using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace HeroLeft.BattleLogic {
    [Serializable]
    public class PassiveSkill {
        public bool IsPassiveSkill = false;
        public bool HaveChanse = false;

        [Range(0, 100)]
        public float chanse;
    }
}
