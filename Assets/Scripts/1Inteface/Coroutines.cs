using System.Collections;
using UnityEngine;
using HeroLeft.BattleLogic;

namespace HeroLeft.Interfaces {

    public class Coroutines : MonoBehaviour {

        public delegate void inProcess();
        public delegate void afterEnd();
        public delegate bool condition();

        public IEnumerator conditionCoroutine(condition condition, afterEnd afterEnd) {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            if (condition == null)
            {
                afterEnd();
                yield break;
            }
            yield return new WaitForSeconds(0.1f);

            if (condition != null)
            yield return new WaitUntil(() => condition());
            afterEnd();
        }

        public IEnumerator actionWithCondition(inProcess inProcess, condition condition, afterEnd afterEnd) {
            while(condition() == false) {
                inProcess?.Invoke();
                yield return new WaitForEndOfFrame();
            }
            afterEnd?.Invoke();
        }

        public IEnumerator actionAfterSomeSec(float sec, afterEnd afterEnd) {
            yield return new WaitForSeconds(sec);
            afterEnd?.Invoke();
        }
    }
}
