using UnityEngine;

namespace HeroLeft.Misc
{
    public class MoveIndicatorRemove : MonoBehaviour
    {
        void Update()
        {
            if (BattleLogic.BattleControll.heroLogic.UnitPosition != 0)
                Destroy(gameObject);
        }
    }
}