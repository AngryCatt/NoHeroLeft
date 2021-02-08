using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HeroLeft.BattleLogic {
    public class SpellLogic : MonoBehaviour, Selected, IBeginDragHandler, IDragHandler, IEndDragHandler {

        public Spell spellImage;
        public SpellInBattle spellInBattle;
        public bool byRegion = false;
        public bool attackSpell = false;

        private RegionSpell regionSpell;
        private string _selectZone = "";
        private string _reloadImg = "";
        private GameObject selectZone;
        private GameObject reloadImg;
        private bool started = false;

        public void Start()
        {
            if (spellImage == null)
            {
                Destroy(gameObject);
                return;
            }
            GetComponent<Image>().sprite = spellImage.uImage.img;
            if (spellImage.splashType == Spell.SplashType.Field) byRegion = true;

            if (spellInBattle != null)
                spellInBattle.spellImage = spellImage;

            if (!started)
            {
                if (!byRegion)
                    GetComponent<Button>().onClick.AddListener(() => Select());

                spellInBattle = new SpellInBattle(transform, spellImage);
                started = true;
            }

        }

        public void Select()
        {
            if (!BattleLogic.battleLogic.IsSelected(this))
            {
                OnSelect();
            }
            else
            {
                OnDeselect();
            }
        }

        public void OnSelect()
        {
            if ((uint)spellImage.splashType < 4)
            {
                BattleLogic.battleLogic.SelectAction(this, true);
                transform.localScale = Vector3.one * 1.1f;
            }
            else
            {
                RealizeTo(null);
            }
        }

        public void OnDeselect()
        {
            BattleLogic.battleLogic.SelectAction(this, false);
            transform.localScale = Vector3.one;
        }

        public void RealizeTo(Unit unit)
        {
            if (BattleControll.heroLogic.Energy >= spellImage.EnergyCost && BattleControll.heroLogic.Mana >= spellImage.ManaCost)
            {
                spellInBattle.Realizeto(BattleControll.heroLogic,unit);
            }
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if (GetComponent<Button>().interactable == true)
                if (byRegion && CanBeUsed() && spellImage.DummyUnit != null)
                {
                    regionSpell = Instantiate<RegionSpell>(spellImage.DummyUnit, BattleControll.battleControll.DummyParent);
                }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (regionSpell != null)
                if (byRegion)
                {
                    regionSpell.transform.position = Input.mousePosition;
                }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (regionSpell != null)
                if (byRegion)
                {
                    if (CanBeUsed())
                    {
                        if (regionSpell.ApplyRegion(spellImage, spellInBattle))
                        {
                            regionSpell = null;
                            BattleControll.heroLogic.Energy -= spellImage.EnergyCost;
                            BattleControll.heroLogic.Mana -= spellImage.ManaCost;
                            spellInBattle.GoReload(spellImage.ReloadTurns);
                        }
                    }
                    else
                    {
                        Destroy(regionSpell);
                    }

                }
        }

        public bool CanBeUsed()
        {
            return BattleLogic.battleLogic.turnPosition == TurnPosition.MyTurn && BattleControll.heroLogic.Energy >= spellImage.EnergyCost && BattleControll.heroLogic.Mana >= spellImage.ManaCost;
        }
    }

    public class SpellInBattle {
        public int RestTurn;
        public Transform transform;
        public Text ReloadText;
        public Spell spellImage;


        public SpellInBattle(Transform transform, Spell spell, bool isUnit = false)
        {
            this.transform = transform;
            spellImage = spell;
            if (!isUnit)
                ReloadText = transform.GetChild(0).GetComponent<Text>();
        }

        public void active(bool state)
        {
            byte bt = (state) ? (byte)255 : (byte)200;
            transform.GetComponent<Image>().color = new Color32(bt, bt, bt, (state) ? (byte)255 : (byte)240);
            transform.GetComponent<Button>().interactable = state;
        }

        public void GoReload(int Reload)
        {
            RestTurn = Reload;
            //INACTIVE
            if (RestTurn > 0 && ReloadText != null)
            {
                ReloadText.gameObject.SetActive(true);
                ReloadText.text = RestTurn.ToString();
                transform.GetComponent<Image>().color = new Color32(200, 200, 200, 240);
                transform.GetComponent<Button>().interactable = false;
            }
        }

        public bool Reloading()
        {
            if (RestTurn <= 0) return true;
            RestMinus();
            if (RestTurn <= 0 && ReloadText != null)
            {
                ReloadText.gameObject.SetActive(false);
                transform.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                transform.GetComponent<Button>().interactable = true;
                return true;
            }
            return false;
        }

        private void RestMinus()
        {
            RestTurn--;
            if(transform.GetComponentInChildren<Text>() != null)
            transform.GetComponentInChildren<Text>().text = RestTurn.ToString();
        }

        public void MomentalStopReloading()
        {
            RestTurn = 0;
            if (ReloadText != null)
            {
                ReloadText.gameObject.SetActive(false);
                transform.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                transform.GetComponent<Button>().interactable = true;
            }
        }

        public void Realizeto(Unit realizer,Unit unit, int queue = -1)
        {
            if (unit != null)
            {
                if (unit.unitlogic.Hp <= 0) return;
            }
            if (realizer is HeroLogic) {
                if(unit != null)
                if (spellImage.spellTarget.HasFlag(Spell.SpellTarget.Enemy) && unit.Alien || spellImage.spellTarget.HasFlag(Spell.SpellTarget.Alies) && !unit.Alien) return;
                HeroLogic heroLogic = (HeroLogic)realizer;
                heroLogic.Energy -= spellImage.EnergyCost;
                heroLogic.Mana -= spellImage.ManaCost;
            }
            GoReload(spellImage.ReloadTurns);
            spellImage.Execute(this, unit, true, queue);
        }
    }
}
