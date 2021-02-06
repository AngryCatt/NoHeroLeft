using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroLeft.BattleLogic {

    [CreateAssetMenu(menuName = "Item", fileName = "New Item", order = 51)]
    public class ItemObject : ScriptableObject {
        public string ItemName;
        public int itemID;

        [Header("Visual")]
        public UImage image;
    }
}
