using HeroLeft.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HeroLeft.BattleLogic {

    [CreateAssetMenu(menuName = "UnitImage", fileName = "New Unit", order = 51)]
    public partial class UnitObject : ScriptableObject, ICloneable {
        public string UnitName = "";
        public bool IsHero = false;
        public UImage Avatar = new UImage();
        public bool MirrorImage;
        public UnitProperty unitProperty;
        public Spell[] Spells;
        public ItemObject[] Items;

        [Header("Only For nonHero units")]
        public int ActionsPerTurn = 1;
        public bool FirstLinePriority = false;
        public bool IsRangeUnit = false;
        public AttackType attackType;
        public UImage ico = new UImage();

        [Header("Unit Events")]
        public EventAction unitActions;

        public XmlManager.UnitXML.UnitInfo UnitInfo {
            get {
                if (unitInfo == null) {
                    unitInfo = XmlManager.UnitXML.GetUnitInfo(UnitName);
                }
                return unitInfo;
            }
        }
        private XmlManager.UnitXML.UnitInfo unitInfo;

        public object Clone() {
            Spell[] spells = new Spell[Spells.Length];
            for(int i = 0; i < spells.Length; i++) {
                spells[i] = (Spell)Spells[i].Clone();
            }

            return new UnitObject() {
                ActionsPerTurn = ActionsPerTurn,
                UnitName = UnitName,
                Avatar = Avatar,
                ico = ico,
                IsRangeUnit = IsRangeUnit,
                MirrorImage = MirrorImage,
                Spells = spells,
                unitInfo = unitInfo,
                unitActions = (EventAction)unitActions.Clone(),
                unitProperty = (UnitProperty)unitProperty.Clone(),
                name = name,
                FirstLinePriority = FirstLinePriority,
                attackType = attackType == null ? null : (AttackType)attackType.Clone(),
                IsHero = IsHero,
                Items = Items
            };
        }

        public void Summon()
        {
            BattleControll.battleControll.Summon(this);
        }

        public void Spawn(string units)
        {
            string[] un = units.Split(',');
            bool rnd = Randomize.Random(50);

            UnitObject unit = (UnitObject)Resources.Load<UnitObject>(rnd ? un[0] : un[1]);
            BattleControll.battleControll.Summon(unit);
        }
    }
    [Serializable]
    public class UImage {
        public Sprite img;
        public Sprite Container;
        public Vector2 pos;
        public Vector2 size;
    }
}

