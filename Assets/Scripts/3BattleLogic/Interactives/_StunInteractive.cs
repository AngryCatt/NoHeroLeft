using UnityEngine.UI;
using UnityEngine;
using HeroLeft.BattleLogic;
using System.Collections;

namespace HeroLeft.Interactive
{
    public static class _StunInteractive
    {
        const string ResourceLink = "Prefabs/Interactive/StunInteractive";
        private static GameObject obj;
        private static int actions = 3;
        private static int _act = 0;
        private static bool complited = false;
        private static bool ended = false;

        public static void Stun(Unit target, float time = 3, bool training = false)
        {
            if (ended) return;
            Time.timeScale = 0;
            ended = true;
            if (target.Alien)
            {
                _act = 0;
                complited = false;
                obj = MonoBehaviour.Instantiate(Resources.Load<GameObject>(ResourceLink), BattleControll.battleControll.interactiveParent);
                obj.GetComponentInChildren<Button>().onClick.AddListener(()=> DoAction());
            }

            BattleControll.battleControll.StartCoroutine(timer(time, training));
        }

        public static IEnumerator timer(float time, bool training)
        {
            yield return new WaitForSecondsRealtime(time);
            if (obj != null)
            {
                if (!complited)
                {
                    if (!training)
                    {
                        BattleLogic.BattleLogic.battleLogic.addMyNextQueue(() => { TurnController.turnController.SkipTurn(); ended = false; }, null, 0);
                        MonoBehaviour.Destroy(obj);
                    }
                    else
                    {
                        BattleControll.battleControll.StartCoroutine(timer(time, training));
                        yield break;
                    }
                }
                else
                {
                    MonoBehaviour.Destroy(obj);
                }
            }
            Time.timeScale = 1;
        }

        public static void DoAction()
        {
            _act++;
            if (_act >= actions)
            {
                complited = true;
                ended = false;
            }
        }
    }
}
