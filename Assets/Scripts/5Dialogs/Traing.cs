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
        public Vector3[] positions;


        public void set_CursorPosition(int pos)
        {

        }

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
                cursor.transform.parent.localPosition = positions[0];
                BattleControll.heroLogic.unitlogic.unitEvents.OnAttack.AddListener(() =>
                {
                    if (steps[step].type == 0)
                    {
                        BattleControll.heroLogic.Energy = 40;
                        @event.Invoke();
                        cursor.SetActive(false);
                    }

                    else if (steps[step].type == 1 && BattleControll.heroLogic.Energy < 10 && BattleLogic.BattleLogic.battleLogic.BattleQueue.Count <= 2)
                    {
                        @event.Invoke();
                    }
                });
            }
        }

        public void FocusOnEnemy()
        {
            if(step < 2)
            cursor.transform.parent.localPosition = positions[2];
        }

        GameObject fball = null;
        public void NextStep()
        {
            BattleControll.heroLogic.enabled = false;

            if(fball == null)
                if(step == 2 || step == 3)
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
                cursor.transform.parent.localPosition = positions[0];
                cursor.SetActive(true);
                cursor.GetComponent<Animator>().Play("TrainingCursor");
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
                    if (step != 3) return;
                    fball.GetComponent<Animator>().Play("effect_fireball_train");
                    if (BattleControll.heroLogic.UnitPosition == 0)
                    {
                        StartCoroutine(attack());
                        StartCoroutine(dialogActive());
                    }
                    else
                    {
                        cursor.SetActive(false);
                        step++;
                        GetComponent<Dialog.Dialog>().dialogStep = 10;
                        StartCoroutine(dialogActive());
                    }
                }, null, -1);
            }else if(step == 4)
            {
                Interactive._StunInteractive.Stun((Unit)BattleControll.heroLogic, 3f, true);
                StartCoroutine(dialogActive(.2f));
            }
        }

        public IEnumerator dialogActive(float time = 2f)
        {
            yield return new WaitForSeconds(time);
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