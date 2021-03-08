using UnityEngine;
using UnityEngine.Events;
using HeroLeft.BattleLogic;
using System.Collections;

namespace HeroLeft.Misc
{
    public class Traing : MonoBehaviour
    {
        public int step = 0;
        public trainingStep[] steps;
        public Spell RaimondHeal;
        public GameObject FireBall;
        public GameObject cursor;

        void Start()
        {
            UnityAction @event = () =>
            {
                steps[step].UnityEvent.Invoke();
                step++;
            };

            if (steps[step].type == 0)
            {
                BattleControll.heroLogic.unitImage.gameObject.GetComponent<HeroDrag>().enabled = false;
                BattleControll.heroLogic.unitlogic.unitEvents.OnAttack.AddListener(() =>
                {
                    if (steps[step].type == 0)
                    {
                        BattleControll.heroLogic.Energy = 40;
                        @event.Invoke();
                    }

                    else if (steps[step].type == 1 && BattleControll.heroLogic.Energy < 10 && BattleLogic.BattleLogic.battleLogic.BattleQueue.Count <= 2)
                    {
                        @event.Invoke();
                    }
                });
            }
        }

        GameObject fball = null;
        public void NextStep()
        {
            BattleControll.heroLogic.enabled = false;

            if(fball == null)
            fball = Instantiate(FireBall, BattleControll.battleControll.enemyUnitsParent.transform.GetChild(0).GetComponent<UnitLogic>().ChildImage);

            if (step == 2)
            {
                BattleLogic.BattleLogic.battleLogic.DisposeAct();
                step++;
                StartCoroutine(dialogActive());
            }
            else if (step == 3)
            {
                BattleControll.heroLogic.Energy = 1;
                HeroDrag drag = BattleControll.heroLogic.unitImage.gameObject.GetComponent<HeroDrag>();
                cursor.SetActive(true);
                GetComponent<Dialog.Dialog>().dialogStep = 9;
                if (drag.MarkParent.childCount == 0)
                    drag.Init(1);
                drag.enabled = true;

                if (BattleControll.heroLogic.unitlogic.Hp != BattleControll.heroLogic.unitlogic.unitObject.unitProperty.Hp)
                {
                    RaimondHeal.Execute(null, BattleControll.heroLogic.unitlogic.myUnit, false, -1);
                }

                BattleLogic.BattleLogic.battleLogic.addNextQueue(() =>
                {
                    fball.GetComponent<Animator>().Play("effect_fireball_train");
                    if (BattleControll.heroLogic.UnitPosition == 0)
                    {
                        StartCoroutine(attack());
                        StartCoroutine(dialogActive());
                    }
                    else
                    {
                        GetComponent<Dialog.Dialog>().dialogStep = 10;
                        StartCoroutine(dialogActive());
                    }
                }, null, -1);
            }
        }

        public IEnumerator dialogActive()
        {
            yield return new WaitForSeconds(2f);
            GetComponent<Dialog.Dialog>().MainDialogActive();
        }

        public IEnumerator attack()
        {
            yield return new WaitForSeconds(0.5f);
            BattleControll.heroLogic.unitlogic.TakeImpact(new Impact() { value = 20, isProcent = false }, BattleControll.battleControll.enemyUnitsParent.transform.GetChild(0).GetComponent<UnitLogic>().unitlogic);
        }

        public void ClearQueue()
        {
            BattleLogic.BattleLogic.battleLogic.BattleQueue.RemoveRange(2, BattleLogic.BattleLogic.battleLogic.BattleQueue.Count - 2);
        }


        [System.Serializable]
        public class trainingStep
        {
            public int type = 0;
            public UnityEvent UnityEvent;
        }
    }
}