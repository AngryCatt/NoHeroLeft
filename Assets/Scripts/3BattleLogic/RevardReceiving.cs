using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HeroLeft.BattleLogic {
    public class RevardReceiving : MonoBehaviour {
        public static RevardReceiving revard;

        void Awake() {
            revard = this;
        }
    }
}
