using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroLeft.BattleLogic {
    public class EnemyControll : MonoBehaviour {
        public static EnemyControll enemyControll;
        public List<RegionSpell> regions;
        public bool NeedRefreshPos = false;
        private void Awake()
        {
            enemyControll = this;
        }

        private void Start()
        {
            RealizeLogicToNextTurn();
        }

        public void JustHitStrategy()
        {
            Transform enemies = BattleControll.battleControll.EnemyUnitsParent.transform;
            for (int i = 0; i < regions.Count; i++)
            {
                if (!regions[i].NextTurnRepose(1))
                    regions.RemoveAt(i);
            }
            for (int i = 0; i < enemies.childCount; i++)
            {
                UnitLogic enamy = enemies.GetChild(i).GetComponent<UnitLogic>();
                enamy.NextTurnRepose(1);
                if (NeedRefreshPos)
                    PositionReload(enamy);
            }

            BattleControll.heroLogic.LinkedSpells();
            for (int i = 0; i < enemies.childCount; i++)
            {

                UnitLogic enamy = enemies.GetChild(i).GetComponent<UnitLogic>();
                int act = enamy.unitlogic.UnitAction;

                if(enamy.unitlogic.attackType != null)
                {
                    SpellPart sp = enamy.unitlogic.attackType.AvailableSpell;
                    if (sp.Available(enamy.unitlogic.attackType))
                    {
                        try
                        {
                            switch (sp.spell.spellTarget){
                                case Spell.SpellTarget.Enemy:
                                    sp.spell.Execute(null, BattleControll.heroLogic);
                                    break;
                                case Spell.SpellTarget.Alies:

                                    break;
                                default:
                                    sp.spell.Execute(null, null,true,-2);
                                    break;
                            }
                        }
                        catch
                        {
                            Debug.Log("eerr");
                        }
                        finally
                        {
                            enamy.unitlogic.UnitAction--;
                            act -= sp.spell.EnergyCost;
                        }
                    }
                }

                if (enamy.spells.Length > 0)
                {
                    for (int sp = 0; sp < enamy.spells.Length; sp++)
                    {
                        SpellInBattle spell = enamy.spells[sp];
                        if (spell.spellImage.passiveSettings.IsPassiveSkill) continue;
                        if (spell.Reloading())
                        {
                            if (spell.spellImage.spellTarget == Spell.SpellTarget.Enemy)
                            {
                                spell.Realizeto(enamy, BattleControll.heroLogic);
                                enamy.unitlogic.UnitAction--;
                                act -= spell.spellImage.EnergyCost;
                            }
                        }

                    }
                }

                if (enamy.unitlogic.CanAttack(BattleControll.heroLogic))
                    for (int q = 0; q < act; q++)
                    {
                        if (enamy.unitlogic.UnitAction > 0)
                        {
                            enamy.unitlogic.AttackUnit(BattleControll.heroLogic, 1);
                        }
                    }
            }

            for (int i = 0; i < regions.Count; i++)
            {
                if (!regions[i].NextTurnRepose(0))
                    regions.RemoveAt(i);
            }

            for (int i = 0; i < enemies.childCount; i++)
            {
                UnitLogic enamy = enemies.GetChild(i).GetComponent<UnitLogic>();
                enamy.EffectsTick(1);
            }
            NeedRefreshPos = false;
            BattleLogic.battleLogic.addAction(() =>
            {
                BattleControll.battleControll.SpawnEnemies();
                GoToFirstLine();
                TurnController.turnController.NextTurn();
                RealizeLogicToNextTurn();
            }, null);
        }

        public void RealizeLogicToNextTurn()
        {
            BattleLogic.battleLogic.addNextQueue(JustHitStrategy, null);
        }

        public void PositionReload(UnitLogic unit)
        {
            int Lines = BattleControll.LoadedLevel.EnemiesOnField / BattleControll.LoadedLevel.EnemyRows;
            Vector2Int myPos = unit.position;
            UnitLogic swapUnit = null;
            Transform enemies = BattleControll.battleControll.EnemyUnitsParent.transform;

            for (int i = 0; i < enemies.childCount; i++)
            {
                UnitLogic enamy = enemies.GetChild(i).GetComponent<UnitLogic>();
                if (enamy == unit) continue;
                if (unit.unitObject.FirstLinePriority)
                {
                    if (enamy.position.y > myPos.y)
                    {
                        if (swapUnit == null)
                            swapUnit = enamy;
                        else
                        {
                            if (enamy.unitlogic.Hp < swapUnit.unitlogic.Hp)
                            {
                                swapUnit = enamy;
                            }
                        }
                    }
                }
                else if (unit.unitObject.IsRangeUnit && unit.position.y != 0)
                {
                    if (enamy.position.y == 0 && !enamy.unitObject.IsRangeUnit)
                    {
                        if (swapUnit == null)
                        {
                            swapUnit = enamy;
                        }
                        else
                        {
                            if (enamy.unitlogic.Hp > swapUnit.unitlogic.Hp)
                            {
                                swapUnit = enamy;
                            }
                        }
                    }
                }
                else
                {
                    // if (unit.position.y > 0 && enamy.position.y > 0)
                    if (unit.unitlogic.Hp <= unit.unitObject.unitProperty.Hp / 3 && !enamy.unitObject.IsRangeUnit && unit.position.y > 0)
                    {
                        if (swapUnit == null)
                        {
                            if (enamy.unitlogic.Hp > unit.unitlogic.Hp && Math.Abs(BattleControll.heroLogic.GetRealPos() - enamy.position.x) > Math.Abs(BattleControll.heroLogic.GetRealPos() - unit.position.x) || enamy.position.y == 0 && enamy.unitlogic.Hp > unit.unitlogic.Hp)
                                swapUnit = enamy;
                        }
                        else if (enamy.position.y == 0 && enamy.unitlogic.Hp > unit.unitlogic.Hp)
                        {
                            if (swapUnit.position.y > 0 || enamy.unitlogic.Hp > swapUnit.unitlogic.Hp)
                                swapUnit = enamy;
                        }
                        else if (enamy.unitlogic.Hp > unit.unitlogic.Hp && Math.Abs(BattleControll.heroLogic.GetRealPos() - enamy.position.x) > Math.Abs(BattleControll.heroLogic.GetRealPos() - swapUnit.position.x))
                        {
                            if (swapUnit.position.y != 0 || enamy.unitlogic.Hp > swapUnit.unitlogic.Hp * 2)
                                swapUnit = enamy;
                        }
                    }
                }
            }

            if (swapUnit != null && swapUnit != unit)
            {
                Vector2Int newPos = swapUnit.position;
                Vector3 pos1 = BattleControll.battleControll.EnemyUnitsParent.GetPosition(newPos.x, newPos.y);
                Vector3 pos2 = BattleControll.battleControll.EnemyUnitsParent.GetPosition(myPos.x, myPos.y);

                swapUnit.SetPosition(myPos, pos2);
                unit.SetPosition(newPos, pos1);

                BattleControll.battleControll.EnemyUnitsParent.units[myPos.x, myPos.y] = swapUnit;
                BattleControll.battleControll.EnemyUnitsParent.units[newPos.x, newPos.y] = unit;

                //   unit.unitlogic.UnitAction = 0;

                StartCoroutine(enumerator(unit, swapUnit, myPos, newPos));
            }
        }

        public void GoToFirstLine()
        {
            if (BattleControll.battleControll.EnemyLines <= 1) return;
            UnitLogic[,] units = BattleControll.battleControll.EnemyUnitsParent.units;

            int y = units.GetLength(1) - 1;

            for (int x = 0; x < units.GetLength(0); x++)
            {
                if (units[x, y] == null)
                {
                    UnitLogic logic = null;
                    for (int q = 0; q < units.GetLength(0); q++)
                    {
                        if (!units[q, 0].unitObject.IsRangeUnit)
                        {
                            if (logic == null || units[q, 0].unitlogic.Hp > logic.unitlogic.Hp || units[q, 0].unitlogic.Hp == logic.unitlogic.Hp && Mathf.Abs(x - q) < Math.Abs(x - logic.position.x))
                            {
                                logic = units[q, 0];
                            }
                        }
                    }
                    if (logic != null)
                    {
                        Vector3 pos = BattleControll.battleControll.EnemyUnitsParent.GetPosition(x, y);

                        logic.SetPosition(new Vector2Int(x, y), pos);

                        BattleControll.battleControll.EnemyUnitsParent.units[logic.position.x, logic.position.y] = null;
                        BattleControll.battleControll.EnemyUnitsParent.units[x, y] = logic;
                    }
                }
            }
        }

        private IEnumerator enumerator(UnitLogic unit, UnitLogic swapUnit, Vector2Int myPos, Vector2Int newPos)
        {
            yield return new WaitForEndOfFrame();
            if (myPos.y == 0)
            {
                swapUnit.transform.SetAsFirstSibling();
            }
            else
            {
                swapUnit.transform.SetAsLastSibling();
            }

            if (newPos.y == 0)
            {
                unit.transform.SetAsFirstSibling();
            }
            else
            {
                unit.transform.SetAsLastSibling();
            }

        }
    }
}
