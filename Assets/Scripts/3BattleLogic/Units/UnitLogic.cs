using UnityEngine;
using UnityEngine.UI;

namespace HeroLeft.BattleLogic
{
    public class UnitLogic : MonoBehaviour, Unit, Selected
    {

        public string UnitName { get { return nameTag.text; } }
        [SerializeField] private Logic logic;

        public UnitObject unitObject { get { return logic.unitObject; } set { logic.unitObject = (UnitObject)value.Clone(); } }
        public Logic unitlogic { get { return logic; } set { logic = value; } }
        public bool Alien { get; set; }

        public Transform ChildImage;
        private GameObject SelectCircle;
        private Text nameTag;

        [HideInInspector] public SpellInBattle[] spells;
        public Vector2Int position;
        private bool init = false;

        public void Start()
        {
           // InitLogic();
            ChildImage.SetParent(BattleControll.battleControll.unitImages);

            ChildSettingsApp();
            RefreshSpells();
        }

        [ContextMenu("ll")]
        public void vv()
        {
            spells = new SpellInBattle[unitObject.Spells.Length];
            if (unitObject.Spells.Length > 0)
                for (int i = 0; i < unitObject.Spells.Length; i++)
                {
                    Debug.Log(unitObject.Spells[i].unitEvents.MyUnit);
                }
        }

        private void RefreshSpells()
        {
            spells = new SpellInBattle[unitObject.Spells.Length];
            if (unitObject.Spells.Length > 0)
                for (int i = 0; i < unitObject.Spells.Length; i++)
                {
                    unitObject.Spells[i].unitEvents.MyUnit = this;
                    spells[i] = new SpellInBattle(transform, unitObject.Spells[i], true);
                    if (spells[i].spellImage.passiveSettings.IsPassiveSkill && spells[i].spellImage.unitEvents.BattleCry.GetPersistentEventCount() > 0)
                        spells[i].spellImage.unitEvents.BattleCry.Invoke();
                }
        }

        public void SetPosition(Vector2Int pos, Vector3 myPosition)
        {
            transform.position = myPosition;
            logic.ReloadStartPosition(myPosition);
            position = pos;

        }

        public Unit InitLogic()
        {
            if (!init)
            {
                ChildImage = transform.GetChild(0);
                logic = new Logic(unitObject, transform, ChildImage, this);
                init = true;
            }
            return this;
        }

        public void NextTurnRepose(Effect.actionCall start)
        {
            if (this == null) return;

            if (logic.UnitAction <= 0)
                logic.UnitAction += (logic.UnitAction < unitObject.ActionsPerTurn) ? unitObject.ActionsPerTurn : 0;

            EffectsTick(start);
            if (this == null) return;
            SpellsReload();
            logic.CallLinkedSpells();
        }

        private void SpellsReload()
        {
            if (spells != null && spells.Length > 0)
                foreach (SpellInBattle spellInBattle in spells)
                {
                    spellInBattle.Reloading();
                }
        }

        public void EffectsTick(Effect.actionCall start)
        {
            logic.EffectsTick(start);
        }

        private void ChildSettingsApp()
        {
            ChildImage.GetComponent<Image>().sprite = unitObject.Avatar.img;
            ChildImage.GetComponent<RectTransform>().pivot = unitObject.Avatar.pos + Vector2.one / 2;
            Vector2 sz = (unitObject.Avatar.size == Vector2.zero) ? Vector2.one * 100 : unitObject.Avatar.size;
            ChildImage.GetComponent<RectTransform>().sizeDelta = sz;
            ChildImage.GetComponent<RectTransform>().localScale = (!unitObject.MirrorImage) ? Vector2.one : new Vector2(-1, 1);
            GetComponent<CapsuleCollider2D>().size = new Vector2(sz.x / 1.1f, sz.y / 1.15f);
            if (ChildImage.localScale.x < 0) nameTag.transform.localScale = new Vector3(-1, 1, 1);
        }

        public void Select()
        {
            if ((Object)BattleLogic.battleLogic.selectedTarget == this)
            {
                ActToTarget();
            }
            else if (BattleLogic.battleLogic.selectedTarget != null)
            {
                BattleLogic.battleLogic.selectedTarget.OnDeselect();
                OnSelect();
            }
            else
            {
                OnSelect();
            }
        }

        public void ActToTarget()
        {
            BattleLogic.battleLogic.RealizeAction(this,false);
        }

        public void OnDeselect()
        {
            Destroy(SelectCircle);
            if (nameTag != null)
                nameTag.gameObject.SetActive(false);

            BattleLogic.battleLogic.selectedTarget = null;
            logic.hpSlider = null;
            BattleControll.battleControll.UnitInfoHide();
        }

        public void OnSelect()
        {
            BattleLogic.battleLogic.selectedTarget = this;
            BattleLogic.battleLogic.CreateSelectCircle(ChildImage, ref SelectCircle);
            nameTag.gameObject.SetActive(true);

            logic.hpSlider = BattleControll.battleControll.enemyHp;
            BattleControll.battleControll.UnitInfoShow(this);
        }

        public void RealizeTo(Unit unit)
        {
            logic.AttackUnit(unit);
        }

        public void ReloadNameTag()
        {
            nameTag = transform.GetChild(0).GetChild(0).GetComponent<Text>();
            nameTag.text = Interfaces.XmlManager.UnitXML.GetUnitInfo(unitObject.UnitName).UnitName + " " + (transform.GetSiblingIndex() + 1).ToString();
            nameTag.gameObject.SetActive(false);
            nameTag.GetComponent<RectTransform>().localPosition = new Vector2(0, GetComponent<RectTransform>().rect.height);
        }
    }
}
