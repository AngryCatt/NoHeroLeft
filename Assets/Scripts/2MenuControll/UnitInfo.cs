using UnityEngine;
using UnityEngine.UI;

namespace HeroLeft.BattleLogic {
    public class UnitInfo : MonoBehaviour {
        public int Number;
        public UnitObject unitObject;

        [Header("UIs")]
        public Text num;

        public void Load(UnitObject unitObject, int num) {
            Number = num;

            this.unitObject = unitObject;
            this.num.text = num.ToString();

            if(unitObject.ico.img != null)
            GetComponent<Image>().sprite = unitObject.ico.img;

            if(unitObject.ico.size != Vector2.zero) {
                GetComponent<RectTransform>().sizeDelta = unitObject.ico.size;
            }
        }
    }
}
