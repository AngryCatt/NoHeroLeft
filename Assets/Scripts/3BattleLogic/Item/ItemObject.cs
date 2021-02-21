using UnityEngine;

namespace HeroLeft.BattleLogic {
    [CreateAssetMenu(menuName = "Item", fileName = "New Item", order = 51)]
    public class ItemObject : ScriptableObject {
        public string ItemName;
        public int itemID;

        [Header("Property")]
        public UnitProperty ItemProperty;
        public EventAction ItemAction;

        [Header("Visual")]
        public UImage image;
    }
}
