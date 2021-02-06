using HeroLeft.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HeroLeft.BattleLogic {

    public class BattleLogic {

        public static BattleLogic battleLogic;

        private Selected selectedAction;
        public Selected selectedTarget;
        public List<UnityAction> BattleQueue = new List<UnityAction>();
        public List<QueueTurn> MyturnQueue = new List<QueueTurn>();
        public List<QueueTurn> NextTurnQueue = new List<QueueTurn>();
        public Coroutines coroutines = new Coroutines();
        public TurnPosition turnPosition = TurnPosition.MyTurn;
        private UnityAction ActionNow;

        public void SelectAction(Selected action, bool select)
        {
            if (action == selectedAction && select) return;
            if (selectedTarget != null && action != selectedAction) selectedTarget.OnDeselect();
            if (!select)
            {
                selectedAction = null;
            }
            else
            {
                if (selectedAction != null) { selectedAction.OnDeselect(); }
                selectedAction = action;
            }
        }

        public bool IsSelected(Selected action)
        {
            return selectedAction == action;
        }

        public bool NullSelected()
        {
            return selectedAction == null;
        }

        public void addAction(UnityAction action, Coroutines.condition condition = null, int order = -1)
        {
            UnityAction unityAction = () =>
            {
                action();
                BattleControll.battleControll.StartCoroutine(coroutines.conditionCoroutine(condition, NextQueueStep));
            };
            if (order == -1)
            {
                BattleQueue.Add(unityAction);
            }
            else
            {
                BattleQueue.Insert(order, unityAction);
            }
            if (BattleQueue.Count == 1)
            {
                ActionNow = BattleQueue[0];
                BattleQueue[0].Invoke();
            }
        }

        public void addNextQueue(UnityAction action, Coroutines.condition condition = null, int order = -1)
        {
            if (order == -1)
            {
                NextTurnQueue.Add(new QueueTurn(action, condition));
            }else if(order == -2)
            {
                action.Invoke();
            }
            else
            {
                NextTurnQueue.Insert(order, new QueueTurn(action, condition));
            }
        }
        public void addMyNextQueue(UnityAction action, Coroutines.condition condition = null, int order = -1)
        {
            if (order == -1)
            {
                MyturnQueue.Add(new QueueTurn(action, condition));
            }
            else
            {
                MyturnQueue.Insert(order, new QueueTurn(action, condition));
            }
        }
        public void RealizeAllQueue(ref List<QueueTurn> queue)
        {
            List<QueueTurn> qq = new List<QueueTurn>(queue);
            queue.Clear();
            foreach (QueueTurn queueTurn in qq)
            {
                addAction(queueTurn.unityAction, queueTurn.condition);
            }
        }

        public void NextQueueStep()
        {
            BattleQueue.Remove(ActionNow);
            if (BattleQueue.Count > 0)
            {
                ActionNow = BattleQueue[0];
                BattleQueue[0].Invoke();
            }
        }

        public void RealizeAction(Unit unit, bool needDispose = true)
        {
            if (turnPosition != TurnPosition.MyTurn) return;
            if (!NullSelected())
            {
                selectedAction.RealizeTo(unit);
                if (needDispose) DisposeAct();
            }
        }

        public void ImpactOnUnit(Selected obj, Unit target)
        {
            selectedAction = obj;
            selectedTarget = (Selected)target;
            RealizeAction(target);
        }

        public void CreateSelectCircle(Transform unit, ref GameObject obj)
        {
            obj = MonoBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/SelectCircle"), unit);
            obj.GetComponent<RectTransform>().localPosition = new Vector2(0, -unit.GetComponent<RectTransform>().rect.height / 2);
            if (selectedAction != null && selectedTarget != null)
            {
                if(selectedAction is Unit && selectedTarget is Unit) {
                    Unit _ut = (Unit)selectedAction;
                    Unit _tUnit = (Unit)selectedTarget;
                    if (_ut != null && _tUnit != null)
                    {
                        obj.transform.GetComponentInChildren<Text>().text = (100 - _tUnit.unitlogic.MissChanse(_ut.unitlogic)).ToString() + "<size=18>%</size>";
                    }
                }
            }
        }

        public void DisposeAct()
        {
            selectedAction.OnDeselect();
            if (selectedTarget != null)
                selectedTarget.OnDeselect();

            selectedAction = null;
            selectedTarget = null;
        }
    }

    public interface Unit {
        bool Alien { get; set; }
        Logic unitlogic { get; set; }
        UnitObject unitObject { get; set; }
    }

    public interface Selected {
        void OnDeselect();
        void OnSelect();
        void RealizeTo(Unit unit);
    }

    public enum TurnPosition {
        MyTurn,
        Nothing,
        EnemyTurn
    }
    public class QueueTurn {
        public UnityAction unityAction;
        public Coroutines.condition condition;

        public QueueTurn(UnityAction unityAction, Coroutines.condition condition)
        {
            this.condition = condition;
            this.unityAction = unityAction;
        }
    }

    [Serializable]
    public class EventAction : ICloneable {
        public Unit MyUnit;
        public Spell.SpellTarget target;
        public UnityEvent BattleCry;
        public UnityEvent OnGetDamage;
        public UnityEvent OnAttack;
        public UnityEvent AfterDead;

        public object Clone()
        {
            return new EventAction()
            {
                AfterDead = (AfterDead != null) ? AfterDead : new UnityEvent(),
                OnGetDamage = (OnGetDamage != null) ? OnGetDamage : new UnityEvent(),
                OnAttack = (OnAttack != null) ? OnAttack : new UnityEvent(),
                BattleCry = (BattleCry != null) ? BattleCry : new UnityEvent(),
                target = target,
                MyUnit = MyUnit,
            };
        }

        public EventAction()
        {
            AfterDead = new UnityEvent();
            OnGetDamage = new UnityEvent();
            OnAttack = new UnityEvent();
            BattleCry = new UnityEvent();
        }
    }
}
