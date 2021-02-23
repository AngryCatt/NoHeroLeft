using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeroLeft.BattleLogic {
    [CreateAssetMenu(menuName = "SpellImage", fileName = "New Spell", order = 51)]
    public partial class Spell : ScriptableObject, ICloneable {
        public string SpellName;
        public bool HeroSpell = false;
        public Impact ImpactValue;
        public UImage uImage;
        public PassiveSkill passiveSettings;

        [Header("Only For Hero"), Space(30)]
        public int ManaCost;
        public int EnergyCost;

        [Header("Main Settings")]
        public int ReloadTurns;
        public bool InstantAction;
        public int Perk = 0;
        public classType spellClass;
        public SpellTarget spellTarget;
        public string spellType = "Hp";
        public Effect[] effects;

        [Header("Splash Settings")]
        public SplashType splashType = new SplashType();
        public int Targets = 1;
        [Range(0, 100)] public int DamageDecrase = 100;

        [Header("Events")]
        public RegionSpell DummyUnit;
        public bool CallEveryTurn = false;
        public EventAction unitEvents;

        [Header("Visual Settings")]
        public EffectImage effectImage;

        [HideInInspector] public List<Logic> linkedUnits = new List<Logic>();

        public float Damage { get { return ImpactValue.value; } set { ImpactValue.value = value; } }

        public object Clone()
        {
            return new Spell()
            {
                effects = (Effect[])effects.Clone(),
                DamageDecrase = DamageDecrase,
                effectImage = effectImage,
                EnergyCost = EnergyCost,
                HeroSpell = HeroSpell,
                ImpactValue = ImpactValue,
                InstantAction = InstantAction,
                ManaCost = ManaCost,
                ReloadTurns = ReloadTurns,
                SpellName = SpellName,
                spellTarget = spellTarget,
                spellType = spellType,
                splashType = splashType,
                passiveSettings = passiveSettings,
                Targets = Targets,
                uImage = uImage,
                DummyUnit = DummyUnit,
                unitEvents = (EventAction)unitEvents.Clone(),
                name = name,
                spellClass = spellClass,
                CallEveryTurn = CallEveryTurn,
                Perk = Perk,
                
            };
        }

        [Flags]
        public enum SpellTarget {
            Alies = 1,
            Enemy = 2,
        }

        public enum SplashType : uint {
            SoloTarget = 0,
            VerticalLine = 1,
            HorizontalLine = 2,
            BothLines = 3,

            lstSpellTarget = 4,
            lstOffender = 5,
            lstDamaget = 6,
            lstKiller = 7,
            lstTarget = 8,
            lstCaller = 9,

            RandomTargets = 10,
            StrongerFirst = 11,
            WekerFirst = 12,

            Field = 50,

            AllTargets = 100,
        }
    }

    public partial class Spell {

        public void Execute(SpellInBattle spellInBattle, Unit target, bool effectImpose = true, int queue = -1, params string[] args)
        {
            if (!Continue()) return;
            if (target != null)
                Helper.lstSpellFocused = target.unitlogic;

            if (unitEvents.MyUnit != null && CallEveryTurn)
            {
                unitEvents.MyUnit.unitlogic.AddCallingSpell(this);
            }
            DamageAction(target, splashType, spellTarget, ImpactValue, effectImpose, spellType, false, InstantAction, spellInBattle, queue);

        }

        public void DamageAction(Unit unit, SplashType splashType, SpellTarget spellTarget, Impact ImpactValue, bool effectImpose = true, string spellType = "Hp", bool effectZeroDuration = false, bool InstantAction = false, SpellInBattle spellInBattle = null, int queue = -1)
        {
            if (queue != -2)
            {
                BattleLogic.battleLogic.addAction(() =>
                {
                    realize();
                }, null, queue);
            }
            else
            {
                realize();
            }
            void realize()
            {
                if (passiveSettings.HaveChanse)
                    if (!Randomize.Random(passiveSettings.chanse)) return;

                if (unit == null || unit.unitlogic.Hp > 0)
                {
                    if (unitEvents.MyUnit != null)
                    {
                        Helper.lstSpellCaller = unitEvents.MyUnit.unitlogic;
                    }
                    if (unitEvents.BattleCry.GetPersistentEventCount() > 0 && effectImpose)
                    {
                        unitEvents.BattleCry.Invoke();
                    }

                    Unit[] targs = GetTargets(unit, splashType, spellTarget);
                    for (int i = 0; i < targs.Length; i++)
                    {
                        if (targs[i] != null)
                        {
                            Effect[] efs = (effectImpose) ? effects : null;
                            targs[i].unitlogic.TakeImpact(ImpactValue, this, efs, spellType, effectZeroDuration, InstantAction);
                        }
                    }
                }
                else
                {
                    if (spellInBattle == null || !effectImpose) return;
                    BattleControll.heroLogic.Energy += EnergyCost;
                    BattleControll.heroLogic.Mana += ManaCost;
                    spellInBattle.MomentalStopReloading();
                }

            }
        }

        public Unit[] GetTargets(Unit unit, SplashType splashType, SpellTarget spellTarget)
        {
            Unit[] targs = null;
            if (splashType == SplashType.SoloTarget || splashType == SplashType.Field) targs = new Unit[1] { unit };
            else if (splashType == SplashType.lstDamaget) targs = new Unit[1] { unitEvents.MyUnit.unitlogic.LastDamaget.myUnit };
            else if (splashType == SplashType.lstTarget) targs = new Unit[1] { Helper.lstTargetEnemy.myUnit };
            else if (splashType == SplashType.AllTargets)
            {
                Transform enm = BattleControll.battleControll.enemyUnitsParent.transform;
                int lenght = 0;
                if (spellTarget.HasFlag(SpellTarget.Alies))
                {
                    lenght++;
                }
                if (spellTarget.HasFlag(SpellTarget.Enemy))
                {
                    lenght += enm.childCount;
                }
                targs = new Unit[lenght];

                if (spellTarget.HasFlag(SpellTarget.Enemy))
                {
                    for (int i = 0; i < enm.childCount; i++)
                    {
                        targs[i] = enm.GetChild(i).GetComponent<UnitLogic>();
                    }
                }
                if (spellTarget.HasFlag(SpellTarget.Alies))
                {
                    targs[targs.Length - 1] = BattleControll.heroLogic;
                }
            }

            return targs;
        }
    }

    public partial class Spell {
        public void CopyEffects(int varible)
        {
            switch (varible)
            {
                case 0:
                    for (int r = 0; r < BattleControll.battleControll.enemyQueue.childCount; r++)
                    {
                        Logic lg = BattleControll.battleControll.enemyQueue.GetChild(r).GetComponent<UnitLogic>().unitlogic;
                        if (!lg.UnderSpell(this))
                        {

                            List<Effect> effects = new List<Effect>();
                            for (int w = 0; w < this.effects.Length; w++)
                            {
                                for (int q = 0; q < Helper.lstDeathEnemy.unitEffects.Count; q++)
                                {
                                    if (Helper.lstDeathEnemy.unitEffects[q].spell.SpellName == this.effects[w].spell.SpellName)
                                    {

                                        effects.Add((Effect)Helper.lstDeathEnemy.unitEffects[q].Clone());
                                    }
                                }
                            }
                            lg.unitEffects.AddRange(effects);
                            lg.unitEvents.AfterDead.AddListener(() => unitEvents.AfterDead.Invoke());
                            lg.unitEvents.OnGetDamage.AddListener(() => unitEvents.OnGetDamage.Invoke());
                            lg.unitEvents.OnAttack.AddListener(() => unitEvents.OnAttack.Invoke());
                            lg.unitEvents.BattleCry.AddListener(() => unitEvents.BattleCry.Invoke());
                            //   for (int i = 0; i < Helper.NewEnemy.unitlogic.unitEffects.Count; i++)
                            //   {
                            //       lg.unitEffects[i].PutEffect(Helper.NewEnemy);
                            //   }
                            return;
                        }
                    }
                    if (true)
                    {
                        Effect[] effects = new Effect[this.effects.Length];
                        for (int w = 0; w < this.effects.Length; w++)
                        {
                            for (int q = 0; q < Helper.lstDeathEnemy.unitEffects.Count; q++)
                            {
                                if (Helper.lstDeathEnemy.unitEffects[q].spell.SpellName == this.effects[w].spell.SpellName)
                                {
                                    effects[w] = (Effect)Helper.lstDeathEnemy.unitEffects[q].Clone();
                                }
                            }
                        }
                        for (int i = 0; i < BattleControll.battleControll.enemyUnitsParent.transform.childCount + BattleControll.battleControll.enemyQueue.childCount; i++)
                        {
                            Transform enemy = null;
                            if (i < BattleControll.battleControll.enemyUnitsParent.transform.childCount)
                                enemy = BattleControll.battleControll.enemyUnitsParent.transform.GetChild(i);
                            else enemy = BattleControll.battleControll.enemyQueue.transform.GetChild(i - BattleControll.battleControll.enemyUnitsParent.transform.childCount);
                            Logic l = (enemy.GetComponent<UnitLogic>()) ? enemy.GetComponent<UnitLogic>().unitlogic : enemy.GetComponent<HeroLogic>().unitlogic;
                            if (Helper.lstDeathEnemy != l)
                            {
                                l.TakeImpact(new Impact { value = 0 }, this, effects);
                            }
                        }
                    }

                    break;
                case 1:
                    if (Helper.NewEnemy != null)
                    {
                        Helper.NewEnemy.unitlogic.unitEffects = Helper.lstDeathEnemy.unitEffects;
                        Helper.NewEnemy.unitlogic.unitEvents.AfterDead.AddListener(() => unitEvents.AfterDead.Invoke());
                        Helper.NewEnemy.unitlogic.unitEvents.OnGetDamage.AddListener(() => unitEvents.OnGetDamage.Invoke());
                        Helper.NewEnemy.unitlogic.unitEvents.OnAttack.AddListener(() => unitEvents.OnAttack.Invoke());
                        Helper.NewEnemy.unitlogic.unitEvents.BattleCry.AddListener(() => unitEvents.BattleCry.Invoke());
                        for (int i = 0; i < Helper.NewEnemy.unitlogic.unitEffects.Count; i++)
                        {
                            Helper.NewEnemy.unitlogic.unitEffects[i].PutEffect(Helper.NewEnemy);
                        }
                    }
                    else
                    {
                        Effect[] effects = new Effect[Helper.lstDeathEnemy.unitEffects.Count];
                        for (int q = 0; q < effects.Length; q++)
                        {
                            effects[q] = (Effect)Helper.lstDeathEnemy.unitEffects[q].Clone();
                        }

                        for (int i = 0; i < BattleControll.battleControll.enemyUnitsParent.transform.childCount; i++)
                        {
                            Transform enemy = BattleControll.battleControll.enemyUnitsParent.transform.GetChild(i);
                            Logic l = (enemy.GetComponent<UnitLogic>()) ? enemy.GetComponent<UnitLogic>().unitlogic : enemy.GetComponent<HeroLogic>().unitlogic;
                            if (Helper.lstDeathEnemy != l)
                            {
                                l.TakeImpact(new Impact { value = 0 }, this, effects);
                            }
                        }
                    }
                    break;
            }
        }

        public void SpellExecute(string cmd)
        {
            string[] cmds = cmd.Split(';');
            string unit = cmds[0];
            string arg = null;
            if (cmds.Length > 1)
                arg = cmds[1];

            if (unit == "Null" || unit.Length == 0)
            {
                if (unitEvents.MyUnit == null)
                    unitEvents.MyUnit = Helper.lstSpellFocused.myUnit;
                if (arg == null || arg.Length == 0)
                {
                    Execute(null, unitEvents.MyUnit, true, 0);
                }
                else if (arg == "0")
                {
                    BattleLogic.battleLogic.addNextQueue(() =>
                    {
                        Execute(null, unitEvents.MyUnit, true, 0);
                    }, null, 0);
                }
            }
            else if (unit == "LastDamaget")
            {
                if (unitEvents.MyUnit == null)
                    unitEvents.MyUnit = Helper.lstOffender.myUnit;
                Execute(null, Helper.lstDamagedEnemy.myUnit, true, 0);
            }
            else
            {
                if (unitEvents.MyUnit == null)
                    unitEvents.MyUnit = Helper.lstDamagedEnemy.myUnit;
                if(Helper.lstOffender != null)
                    Execute(null, Helper.lstOffender.myUnit, true, 0);
            }
        }

        public void SpellExecute(int target)
        {
            Logic targ = Helper.getTarget(target);
            Execute(null, targ.myUnit, true, 0);
        }

        public void Interactive(string active)
        {
            try
            {
                if (!Continue()) return;
                if (active == "stun")
                {

                    HeroLeft.Interactive._StunInteractive.Stun(Helper.lstDamagedEnemy.myUnit, BattleControll.battleControll.interactiveParent);
                }
            }
            catch
            {
                Debug.Log("Cannot Interactive");
            }
        }

        public void AddAttackSpellPoint(int splash)
        {
            Logic log = splash == 0 ? unitEvents.MyUnit.unitlogic : Helper.getTarget(splash);
            if (log == null || log.attackType == null) return;
            log.attackType.target = log.attackType.splashType == 0 ? unitEvents.MyUnit.unitlogic : Helper.getTarget((int)log.attackType.splashType);
            if (log == BattleControll.heroLogic.unitlogic)
            {
                BattleControll.battleControll.RefreshHeroAttackSpell();
            }
        }

        public void GoToNextPartAttackSpell(int splash)
        {
            Logic log = splash == 0 ? unitEvents.MyUnit.unitlogic : Helper.getTarget(splash);
            if (log == null || log.attackType == null) return;
            log.attackType.part++;
            if (log == BattleControll.heroLogic.unitlogic)
            {
                BattleControll.battleControll.RefreshHeroAttackSpell();
            }
        }

        public void RemoveAttackSpellPoint(int splash)
        {
            Logic log = splash == 0 ? unitEvents.MyUnit.unitlogic : Helper.getTarget(splash);
            if (log == null || log.attackType == null) return;
            log.attackType.part = 0;
            if (log == BattleControll.heroLogic.unitlogic)
            {
                BattleControll.battleControll.RefreshHeroAttackSpell();
            }
        }

        public void test()
        {
            Debug.Log("123");
        }

        public void LastUnitDispell(string st)
        {
            if (unitEvents.MyUnit.unitlogic == BattleControll.heroLogic.unitlogic)
            {
                for (int i = 0; i < BattleControll.battleControll.enemyUnitsParent.transform.childCount; i++)
                {
                    Logic logic = BattleControll.battleControll.enemyUnitsParent.transform.GetChild(i).GetComponent<UnitLogic>().unitlogic;

                    if (Helper.lstTargetEnemy != logic)
                    {
                        if (logic.UnderSpell(this))
                        {
                            if (st == null || st.Length == 0)
                                logic.Dispell(this);
                            else if (st.StartsWith("%"))
                            {
                                st = st.Remove(0, 1);
                                logic.Dispell(this, int.Parse(st), true);
                            }
                            else
                            {
                                logic.Dispell(this, int.Parse(st), false);
                            }

                        }
                    }
                }
            }
            /*
            else
            {
                Logic logic = BattleControll.heroLogic.unitlogic;
                if (logic.UnderSpell(this))
                {
                    Debug.Log("Called");
                    if (st == null || st.Length == 0)
                        logic.Dispell(this);
                    else if (st.StartsWith("%"))
                    {
                        st = st.Remove(0, 1);
                        logic.Dispell(this, int.Parse(st), true);
                    }
                    else
                    {
                        logic.Dispell(this, int.Parse(st), false);
                    }

                }
            }
            */
        }

        public void MomentalDeath(float HpPorog)
        {
            Logic logic = Helper.getTarget((int)splashType);
            float hp = logic.Hp;
            float maxHp = logic.unitObject.unitProperty.Hp;

            if (hp / maxHp * 100 < HpPorog)
                logic.Death(null);
        }

        public void Dispell()
        {
            Logic logic = unitEvents.MyUnit.unitlogic;
            while (logic.unitEffects.Count != 0)
            {
                if (logic.unitEffects[0].Dispelling(logic.myUnit)) logic.unitEffects.RemoveAt(0);
            }
        }

        public void TotemSummon(SummonLogic ward)
        {
            Logic unit = Helper.getTarget(9);
            if (unit.totem == null)
            {
                unit.totem = Instantiate<SummonLogic>(ward, unit.unitImage);
            }
            else
            {
                throw new Exception("Summon not null");
            }
        }

        private bool Continue()
        {
            if (passiveSettings.IsPassiveSkill && passiveSettings.HaveChanse)
            {
                return Randomize.Random(passiveSettings.chanse);
            }
            return true;
        }
    }

    [Serializable]
    public class Impact : ICloneable {
        public float value;
        public bool isProcent = false;

        public object Clone()
        {
            return new Impact() { isProcent = isProcent, value = value };
        }
    }
}
