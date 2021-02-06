using UnityEngine;
using HeroLeft.BattleLogic;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace HeroLeft.Menu {
    public class LevelInfo : MonoBehaviour {
        public Image LevelIcon;
        public Text MapName;
        public Text RecomendedLevel;
        public Text Description;

        [Header("LevelUnits")]
        public Transform UnitParent;
        public UnitInfo UnitPrefab;

        [Header("Unit Info Panel")]
        public Image UnitIco;
        public Text UnitName;
        public Text UnitDescription;
        public Text UnitHp;
        public Text UnitDamage;
        public Text UnitHpRegen;
        public Text UnitEvasion;
        public UnityEvent OnClickAction;

        LevelObject levelObject;


        public void Load(LevelObject levelObject) {
            this.levelObject = levelObject;

            LevelIcon.sprite = levelObject.Ico;
            MapName.text = levelObject.LevelInfo.LevelName;
            RecomendedLevel.text = levelObject.LevelInfo.RecomendedLevel;
            Description.text = levelObject.LevelInfo.LevelDiscription;

            List<unitCategory> unitsCategory = new List<unitCategory>();

            foreach(UnitObject unit in levelObject.Units) {
                bool haveCategory = false;

                foreach (unitCategory ct in unitsCategory) {
                    if(unit == ct.unitImage) {
                        haveCategory = true;
                        ct.number++;
                    } 
                }
                if (!haveCategory) {
                    unitsCategory.Add(new unitCategory(unit));
                }
            }

            if(UnitParent.childCount > 0) {
                for(int i = 0; i < UnitParent.childCount; i++) {
                    Destroy(UnitParent.GetChild(i).gameObject);
                }
            }
            foreach(unitCategory uc in unitsCategory) {
                UnitInfo unitInfo = Instantiate<UnitInfo>(UnitPrefab, UnitParent);
                unitInfo.Load(uc.unitImage, uc.number);
                unitInfo.GetComponent<Button>().onClick.AddListener(() => LoadUnitInfo(uc.unitImage));
                unitInfo.GetComponent<Button>().onClick.AddListener(() => OnClickAction.Invoke());
            }
        }

        public void LoadUnitInfo(UnitObject unitObject) {
            UnitIco.sprite = unitObject.Avatar.img;
            UnitHp.text = unitObject.unitProperty.Hp.ToString();
            UnitDamage.text = unitObject.unitProperty.Damage.ToString();
            UnitHpRegen.text = unitObject.unitProperty.HpRegen.ToString();
            UnitEvasion.text = unitObject.unitProperty.Evasion.ToString();

            UnitName.text = unitObject.UnitInfo.UnitName;
            UnitDescription.text = unitObject.UnitInfo.UnitDiscription;
        }

        class unitCategory {
            public UnitObject unitImage;
            public int number = 0;

            public unitCategory(UnitObject unit) {
                unitImage = unit;
                number = 1;
            }
        }

        public void StartLevel() {
            MenuController.menuController.LoadLevel(levelObject);
        }
    }
}
