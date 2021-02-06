using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HeroLeft.Interfaces;

namespace HeroLeft.BattleLogic
{
    public class RegionSpell : MonoBehaviour
    {
        public Color32 newColor;
        public RegionSpellLogic logic;
        public bool RefreshFunctionPerTurn = false;
        private bool hasApplied = false;
        public bool DurInTicks = false;
        private Spell spell;
        private SpellInBattle spellInBattle;
        public Effect.actionCall actionCall;


        private void Start()
        {
            ShowRegion();
        }

        public void ShowRegion()
        {
            Color32 color = GetComponent<Image>().color;
            color.r = 125;
            color.g = 125;
            GetComponent<Image>().color = color;
        }

        public bool ApplyRegion(Spell spell, SpellInBattle spellInBattle)
        {
            if (logic.unitsInRegions.Count == 0)
            {
                Destroy(gameObject);
                return false;
            }
            this.spell = spell;
            this.spellInBattle = spellInBattle;
            hasApplied = true;
            GetComponent<Image>().color = Color.white;
            logic.EffectFunction();

            for (int i = 0; i < logic.unitsInRegions.Count; i++)
            {
                logic.unitsInRegions[i].SetStandartColor();
            }

            if (spell.InstantAction)
                RegionSpellExecute(2);
            EnemyControll.enemyControll.regions.Add(this);
            return true;
        }

        public bool NextTurnRepose(uint start)
        {
            if (!DurInTicks)
                logic.LifeTime++;
            RegionSpellExecute(start);
            if (logic.LifeTime >= logic.Duration)
            {
                Destroy(gameObject);
                return false;
            }
            return true;
        }

        public void RegionSpellExecute(uint Start)
        {
            if (Start == 2 || (uint)actionCall == Start)
                for (int i = 0; i < logic.unitsInRegions.Count; i++)
                {
                    if (logic.LifeTime >= logic.Duration) return;
                    if (logic.unitsInRegions[i].unit.transform == null)
                    {
                        logic.unitsInRegions.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (DurInTicks)
                    {
                        logic.LifeTime++;
                        if (logic.LifeTime >= logic.Duration)
                        {
                            Destroy(gameObject);
                        }
                    }
                    spell.Execute(spellInBattle, logic.unitsInRegions[i].unit.myUnit);
                }
            if (RefreshFunctionPerTurn)
            {
                logic.EffectFunction();
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            Transform newCollider = collision.transform;
            Logic logic = null;
            if (collision.transform.GetComponent<HeroLogic>())
            {
                logic = newCollider.GetComponent<HeroLogic>().unitlogic;
            }
            else if (collision.transform.GetComponent<UnitLogic>())
            {
                logic = newCollider.GetComponent<UnitLogic>().unitlogic;
            }
            if (logic == null || Constains(logic)) return;
            this.logic.unitsInRegions.Add(new unitInRegion(logic, logic.unitImage.GetComponent<Image>().color));
            if (!hasApplied) logic.unitImage.GetComponent<Image>().color = newColor;
            //  else spell.Execute(spellInBattle, logic.myUnit);
        }

        private bool Constains(Logic l)
        {
            for (int i = 0; i < logic.unitsInRegions.Count; i++)
            {
                if (logic.unitsInRegions[i].unit == l)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (hasApplied) return;
            Transform Collider = collision.transform;
            if (collision)
            {
                Logic unit = null;
                if (Collider.GetComponent<HeroLogic>())
                {
                    unit = Collider.GetComponent<HeroLogic>().unitlogic;
                }
                else if (Collider.GetComponent<UnitLogic>())
                {
                    unit = Collider.GetComponent<UnitLogic>().unitlogic;
                }
                if (unit == null) return;

                if (unit != null)
                {
                    for (int i = 0; i < logic.unitsInRegions.Count; i++)
                    {
                        if (logic.unitsInRegions[i].unit == unit)
                        {
                            logic.unitsInRegions[i].SetStandartColor();
                            logic.unitsInRegions.Remove(logic.unitsInRegions[i]);
                        }
                    }
                }
            }
        }

        public class unitInRegion
        {
            public Logic unit;
            private Color32 color;

            public unitInRegion(Logic unit, Color32 color)
            {
                this.unit = unit;
                this.color = color;
            }

            public void SetStandartColor()
            {
                unit.unitImage.GetComponent<Image>().color = color;
            }
        }
        [System.Serializable]
        public class RegionSpellLogic : CommandHandler
        {
            public int Duration = 1;
            public int LifeTime = 0;
            public List<unitInRegion> unitsInRegions = new List<unitInRegion>();

            public int UnitsInField { get { return unitsInRegions.Count; } }
        }
    }
}


