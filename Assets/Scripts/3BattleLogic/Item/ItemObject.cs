using UnityEngine;

namespace HeroLeft.BattleLogic {
    [CreateAssetMenu(menuName = "Item", fileName = "New Item", order = 51)]
    public class ItemObject : ScriptableObject {
        public string ItemName;
        public int itemID;

        public slotType SlotType = slotType.weapon;
        public classType ClassType = classType.none;
        public int NeedEnergy;

        [Header("Property")]
        public UnitProperty ItemProperty;
        public UnitProperty ProcentProperty;
        public EventAction ItemAction;


        [Header("Visual")]
        public UImage image;

        public UnitProperty sum_property => (classObject == null) ? ItemProperty : ItemProperty + classObject.ItemProperty;

        private ItemObject classObject => (ClassType == classType.none) ? null : Resources.Load<ItemObject>(GameManager.ClassItemsFolder + ClassType.ToString());

        public enum slotType
        {
            weapon, //2
            ears, //2
            amulet, //1
            rings, //2
            belt, //1 class
            boots, //1 class
            body, //1 class
            hands, //1 class
            legs, //1 class
            shoulders, //1 class
            head, //1 class
            wrist, //1 class
        }

        public enum classType {
            none,

            cloth_1,
            cloth_2,
            cloth_3,

            leather_1,
            leather_2,
            leather_3,

            plate_1,
            plate_2,
            plate_3,
        }
    }
}