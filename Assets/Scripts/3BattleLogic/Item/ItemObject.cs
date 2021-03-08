using UnityEngine;

namespace HeroLeft.BattleLogic {
    [CreateAssetMenu(menuName = "Item", fileName = "New Item", order = 51)]
    public class ItemObject : ScriptableObject {
        public string ItemName;
        public int itemID;

        public slotType SlotType = slotType.weapon;
        public int NeedEnergy;

        [Header("Property")]
        public UnitProperty ItemProperty;
        public EventAction ItemAction;

        [Header("Visual")]
        public UImage image;

        public enum slotType
        {
            weapon,
        }
    }
}
