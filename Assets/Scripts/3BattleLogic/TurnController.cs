using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HeroLeft.BattleLogic {
    public class TurnController : MonoBehaviour {
        public static TurnController turnController;
        public Text text;
        public int TurnSeconds = 30;
        private static TurnPosition beforeTurn = TurnPosition.Nothing;

        public void SetTimer(int seconds) {
            turnController = this;
            TurnSeconds = seconds;
            startTimer();
        }

        public void startTimer() {
            StartCoroutine(StartTimer());
        }

        private IEnumerator StartTimer() {
            if (BattleLogic.battleLogic.turnPosition != TurnPosition.MyTurn) yield break;

            int sec = TurnSeconds;

            while (sec != 0) {
                text.text = sec.ToString();
                yield return new WaitForSeconds(1);
                sec--;
            }
            text.text = "";
            NextTurn();
        }

        public void SkipTurn() {
            if (BattleLogic.battleLogic.turnPosition != TurnPosition.MyTurn) return;
            StopAllCoroutines();
            text.text = "";
            NextTurn();
        }

        Coroutine queueCoroutine;

        public void NextTurn() {
            beforeTurn = BattleLogic.battleLogic.turnPosition;
            if(queueCoroutine != null) {
                StopCoroutine(queueCoroutine);
                queueCoroutine = null;
            }

            if (BattleLogic.battleLogic.BattleQueue.Count > 0) {
                BattleLogic.battleLogic.turnPosition = TurnPosition.Nothing;
                queueCoroutine = StartCoroutine(BattleLogic.battleLogic.coroutines.conditionCoroutine(EmptyQueue, StartAnotherTurn));
            } else {
                StartAnotherTurn();
            }
        }

        public bool EmptyQueue() {
            if (BattleLogic.battleLogic.BattleQueue.Count > 0) return false;
            return true;
        }


        public void StartAnotherTurn() {
            if (beforeTurn == TurnPosition.MyTurn) {
                BattleLogic.battleLogic.turnPosition = TurnPosition.EnemyTurn;
                BattleLogic.battleLogic.RealizeAllQueue(ref BattleLogic.battleLogic.NextTurnQueue);
                //  EnemyControll.enemyControll.JustHitStrategy();

            } else {
                BattleLogic.battleLogic.turnPosition = TurnPosition.MyTurn;
                BattleLogic.battleLogic.RealizeAllQueue(ref BattleLogic.battleLogic.MyturnQueue);
                startTimer();
              //  EnemyControll.enemyControll.RealizeLogicToNextTurn();
                //   TurnController.turnController.NextTurn();
            }
        }
    }
}
