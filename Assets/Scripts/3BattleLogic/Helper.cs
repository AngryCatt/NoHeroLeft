using UnityEngine;
using HeroLeft.BattleLogic;
namespace HeroLeft {
    public class Helper : MonoBehaviour {
        public static int Enemies { get { return helper.transform.childCount; } }
        public static int EnemiesMinus { get { return Enemies - 1; } }
        public static Unit NewEnemy { get {
                return (
                    BattleControll.battleControll.EnemyQueue.transform.childCount == 0)
? null : (Unit)BattleControll.battleControll.EnemyQueue.transform.GetChild(0).GetComponent<UnitLogic>().InitLogic();
                    } }
        public static Logic lstSpellFocused;
        public static Logic lstSpellCaller;
        public static Logic lstOffender;
        public static Logic lstDeathEnemy;
        public static Logic lstDamagedEnemy;
        public static Logic lstTargetEnemy;
        public static Logic lstKiller;


        public static Logic getTarget(int targ)
        {
            switch (targ)
            {
                case 4:
                    return lstSpellFocused;
                case 5:
                    return lstOffender;
                case 6:
                    return lstDamagedEnemy;
                case 7:
                    return lstKiller;
                case 8:
                    return lstTargetEnemy;
                case 9:
                    return lstSpellCaller;
            }
            return null;
        }

        private void Dispose()
        {
            lstTargetEnemy = null;
            lstDeathEnemy = null;
            lstSpellFocused = null;
            lstKiller = null;
            lstOffender = null;
            lstDamagedEnemy = null;
            lstSpellCaller = null;
        }

        public static Helper helper;

        private void Awake() {
            helper = this;
            Dispose();
        }
    }
}
