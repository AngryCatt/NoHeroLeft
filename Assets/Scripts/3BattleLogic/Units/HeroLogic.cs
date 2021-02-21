using UnityEngine;
using UnityEngine.UI;

namespace HeroLeft.BattleLogic {
    public class HeroLogic : MonoBehaviour, Unit, Selected {

        [SerializeField] private Logic logic;

        public UnitObject unitObject { get { return logic.unitObject; } set { logic.unitObject = (UnitObject)value.Clone(); } }
        public Logic unitlogic { get { return logic; } set { logic = value; } }
        public bool Alien { get; set; }

        public float Energy { get { return (EnergySlider != null) ? EnergySlider.value : 100; } set { if (EnergySlider == null) return; EnergySlider.value = value; EnergySlider.GetComponentInChildren<Text>().text = value.ToString() + "/100"; SpellsChecker(); } }
        public float Mana { get { return (ManaSlider != null) ? ManaSlider.value : 100; } set { if (ManaSlider == null) return; ManaSlider.value = value; ManaSlider.GetComponentInChildren<Text>().text = value.ToString() + "/100"; SpellsChecker(); } }

        [SerializeField] private Slider HpSlider;
        [SerializeField] private Slider EnergySlider;
        [SerializeField] private Slider ManaSlider;

        [SerializeField] private Transform SpellParent;
        [SerializeField] private SpellLogic SpellPrefab;
        public int UnitPosition = 0;
        private GameObject SelectCircle;
        public Transform unitImage;

        private void Awake()
        {
            unitObject = Resources.Load<UnitObject>(GameManager.HeroObjectPath);
            Alien = true;
            if (Alien) BattleControll.heroLogic = this;
        }

        private void Start()
        {
            unitImage = transform.GetChild(0);
            unitImage.SetParent(BattleControll.battleControll.unitImages);
            logic = new Logic(unitObject, transform, unitImage, this, HpSlider);

            BattleLogic.battleLogic.addMyNextQueue(NextTurnRepose, null);
            ReloadSliders();
            ReloadSpells();
        }

        private void NextTurnRepose()
        {
            if (this == null) return;
            Energy = Mathf.Clamp(Energy + 50, 0, EnergySlider.maxValue);
            Mana = Mathf.Clamp(Mana + 50, 0, ManaSlider.maxValue);
            logic.Hp = Mathf.Clamp(logic.Hp + 5, 0, HpSlider.maxValue);
            ReloadAllSpells();
            EffectsTick();
            BattleLogic.battleLogic.addMyNextQueue(NextTurnRepose, null);
            BattleLogic.battleLogic.addNextQueue(() => { logic.EffectsTick(Effect.actionCall.OnStartTurn); }, null);
        }

        public void LinkedSpells()
        {
            logic.CallLinkedSpells();
        }

        private void ReloadSliders()
        {
            if (HpSlider != null && EnergySlider != null && ManaSlider != null)
            {
                HpSlider.maxValue = unitObject.unitProperty.Hp;
                EnergySlider.maxValue = 100;
                ManaSlider.maxValue = 100;

                logic.Hp = unitObject.unitProperty.Hp;
                Energy = 100;
                Mana = 100;
            }
        }

        public float GetRealPos()
        {
            return (float)(UnitPosition + BattleControll.loadedLevel.EnemiesOnField / BattleControll.loadedLevel.EnemyRows + 1) / 2f;
        }

        private void EffectsTick()
        {
            logic.EffectsTick(Effect.actionCall.OnEndTurn);
            BattleControll.battleControll.HeroEffectsRefresh();
        }

        private void ReloadSpells()
        {
            for (int i = 0; i < SpellParent.childCount; i++)
            {
                Destroy(SpellParent.GetChild(i).gameObject);
            }
            if (unitObject.Spells.Length > 0)
                foreach (Spell spell in unitObject.Spells)
                {
                    spell.unitEvents.MyUnit = this;
                    Instantiate<SpellLogic>(SpellPrefab, SpellParent).spellImage = spell;
                }
            BattleControll.battleControll.RefreshHeroAttackSpell();
        }


        public void Select()
        {
            if (!BattleLogic.battleLogic.IsSelected(this))
            {
                BattleLogic.battleLogic.RealizeAction(this);
                OnSelect();
            }
            else
            {
                OnDeselect();
            }
        }

        public void OnSelect()
        {
            BattleLogic.battleLogic.SelectAction(this, true);
            BattleLogic.battleLogic.CreateSelectCircle(unitImage, ref SelectCircle);
        }

        public void OnDeselect()
        {
            Destroy(SelectCircle);
            BattleLogic.battleLogic.SelectAction(this, false);

        }

        public void RealizeTo(Unit unit)
        {
            if (Energy >= BattleConstants.EnergyCost)
            {
                logic.AttackUnit(unit, BattleConstants.EnergyCost);
            }
        }

        public void ReloadAllSpells()
        {
            for (int i = 0; i < SpellParent.childCount; i++)
            {
                SpellLogic spellLogic = SpellParent.GetChild(i).GetComponent<SpellLogic>();
                spellLogic.spellInBattle.Reloading();
            }
            if (BattleControll.battleControll.attackSpell.spellInBattle.Reloading()) 
                BattleControll.battleControll.AttackSpellTurn(logic.CanUseSpecSpell());
        }

        public void SpellsChecker()
        {
            for (int i = 0; i < SpellParent.childCount; i++)
            {
                SpellLogic spell = SpellParent.GetChild(i).GetComponent<SpellLogic>();

                if (spell != null && spell.spellImage != null)
                    if (spell.spellImage.EnergyCost > Energy || spell.spellImage.ManaCost > Mana)
                    {
                        spell.GetComponent<Image>().color = new Color32(200, 200, 255, 240);
                        spell.GetComponent<Button>().interactable = false;
                    }
                    else 
                    {
                        if (spell.spellInBattle.RestTurn == 0)
                        {
                            spell.GetComponent<Image>().color = Color.white;
                            spell.GetComponent<Button>().interactable = true;
                        }
                        else
                        {
                            spell.GetComponent<Image>().color = new Color32(200, 200, 200, 240);
                        }
                    }
            }
        }
    }
}
