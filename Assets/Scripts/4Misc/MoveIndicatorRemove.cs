using UnityEngine;

namespace HeroLeft.Misc
{
    public class MoveIndicatorRemove : MonoBehaviour
    {
        void Update()
        {
            if (BattleLogic.BattleControll.heroLogic.UnitPosition != 0)
            {
                Deactive();
                enabled = false;
            }
        }

        public void Deactive()
        {
            transform.parent.localPosition = BattleLogic.BattleControll.battleControll.GetComponent<Traing>().positions[1];
            GetComponent<Animator>().Play("clickAnim");
        }
    }
}