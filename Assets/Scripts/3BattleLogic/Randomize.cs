using UnityEngine;

namespace HeroLeft.BattleLogic {
    public class Randomize {

        public static bool Random(float value) {
            return value >= UnityEngine.Random.Range(1, 101);
        }
        public static bool Random(int value) {
            return value >= UnityEngine.Random.Range(1, 101);
        }
    }
}
