using HeroLeft.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HeroLeft.BattleLogic
{
    [System.Serializable]
    public class Logic : ICloneable
    {

        [SerializeField] private UnitProperty unitProperty;
        [SerializeField] public Slider hpSlider;

        public List<Effect> unitEffects = new List<Effect>();
        private List<Spell> callingSpells;

        //AttackTypeSpell
        public AttackType attackType;
        public SummonLogic totem;

        public EventAction unitEvents;

        public UnitObject unitObject;
        [HideInInspector] public Transform transform;
        [HideInInspector] public Transform unitImage;
        [HideInInspector] public Unit myUnit;
        private Vector3 vc;
        private bool momentalEnd = false;

        public Logic LastDamaget = null;

        public float position { get { return (transform.GetComponent<HeroLogic>()) ? (transform.GetComponent<HeroLogic>().UnitPosition + BattleControll.LoadedLevel.EnemyRows - 1f) / 2f : transform.GetComponent<UnitLogic>().position.x; } }

        public float Hp
        {
            get { return unitProperty.Hp; }
            set
            {
                float val = (float)Math.Round(value, 1);
                unitProperty.Hp = val;
                if (hpSlider != null)
                {
                    hpSlider.value = val;
                    hpSlider.GetComponentInChildren<Text>().text = val + "/" + unitObject.unitProperty.Hp;
                }
            }
        }

        public float Damage { get { return unitProperty.Damage; } set { unitProperty.Damage = value; } }

        public int UnitAction;

        public void TakeImpact(Impact impact, Logic unitLogic, Spell spell = null, Effect[] effects = null, string property = "Hp", bool zeroDuration = false, bool InstantAction = false)
        {
            if (transform == null) return;
            Helper.lstDamagedEnemy = this;
            if (Hp <= 0) return;
            float damage = 0;
            bool[] Avoided = new bool[1];
            if (unitLogic != null)
            {
                Avoided = new bool[Resources.Load<Transform>(GameManager.DamageIndicator[unitLogic != null && unitLogic.attackType != null ? unitLogic.attackType.damageIndicator : 0]).GetComponentsInChildren<Text>().Length];
            }

            if (property != null && property.Length > 0)
            {
                if (property == "Hp")
                {
                    for (int i = 0; i < Avoided.Length; i++)
                    {
                        if (spell == null)
                        {
                            if (impact.value > 0)
                                if (Randomize.Random(MissChanse(unitLogic)))
                                {
                                    Avoided[i] = true;
                                }
                        }
                        if (!Avoided[i])
                        {
                            if (!impact.isProcent)
                            {
                                damage = impact.value;
                            }
                            else
                            {
                                damage = (float)Math.Round(Hp / 100 * impact.value, 1);
                            }

                            if (unitLogic != null)
                            {
                                damage /= Avoided.Length;
                                damage -= unitProperty.DamageResist;
                                if (unitProperty.Armor != 0)
                                {
                                    BattleConstants.CalculateArmor(ref damage, unitProperty.Armor);
                                }
                            }
                            else if(unitProperty.MagicResist != 0)
                            {
                                BattleConstants.CalculateArmor(ref damage, unitProperty.MagicResist);
                            }
                            if (damage < 0 && spell == null) damage = 0;
                            Hp -= damage;

                            if (Hp > this.unitObject.unitProperty.Hp)
                                Hp = this.unitObject.unitProperty.Hp;
                        }
                    }
                }
                else
                {
                    ChangeValue(impact, property);
                }
            }
            UnitObject unitObject = null;
            UnitLogic MyLogic = transform.GetComponent<UnitLogic>();
            if (unitLogic != null)
                unitObject = unitLogic.unitObject;

            if (property == "Hp")
            {
                DestroyObject des = unitImage.GetComponentInChildren<DestroyObject>();
                Transform indicator = (des != null) ? des.transform : null;

                Transform loadedIndicator = Resources.Load<Transform>(damage >= 0 ? (GameManager.DamageIndicator[unitLogic != null && unitLogic.attackType != null ? unitLogic.attackType.damageIndicator : 0]) : GameManager.HealIndicator);
                if (des != null)
                    if (!des.name.StartsWith(loadedIndicator.name))
                    {
                        indicator = null;
                        des = null;
                    }

                if (des != null && des.lifeTime > 0.2f)
                {
                    indicator.GetComponent<Animation>().Stop();
                    indicator.GetComponent<Animation>().Play(des.stop.name);
                }

                for (int avd = 0; avd < Avoided.Length; avd++)
                {

                    if (damage != 0 || damage == 0 && Avoided[avd])
                    {
                        if (!Avoided[avd])
                        {
                            if (!transform.GetComponent<HeroLogic>())
                                BattleLog.battleLog.addLog("<color=red>" + MyLogic.UnitName + "</color>"
                                    + " теряет " +
                                    "<color=red>" + damage.ToString() + "</color>" +
                                    " от " + "<color=green>" + ((spell != null) ? spell.SpellName : unitObject.UnitName) + "</color>");
                            else
                                BattleLog.battleLog.addLog("<color=green>" + this.unitObject.UnitName + "</color>" + " теряет " +
                                  "<color=red>" + damage.ToString() + "</color>");
                            if (Hp <= 0)
                            {
                                BattleLog.battleLog.addLog("<color=red>" + ((MyLogic.UnitName != null) ? MyLogic.UnitName : this.unitObject.UnitName) + "</color>"
                                + " умерает от  " + "<color=green>" + ((spell != null) ? spell.SpellName : unitObject.UnitName) + "</color>");
                                Death(this);
                            }
                        }

                        if (indicator == null)
                            indicator = UnityEngine.Object.Instantiate<Transform>(loadedIndicator, unitImage);
                        if ((unitImage.localScale.x < 0))
                        {
                            indicator.GetChild(0).localScale = new Vector3(-1, 1, 1);
                        }

                        indicator.transform.localPosition = Vector3.zero;
                        Text[] txt = indicator.GetComponentsInChildren<Text>();
                        float val = (txt[avd].text == "Miss") ? 0 : float.Parse(txt[avd].text);
                        if (Avoided.Length == 1)
                        {
                            if (Avoided[0])
                                UnityEngine.Object.Instantiate<Transform>(Resources.Load<Transform>(GameManager.MissIndicator), indicator.transform);
                            txt[0].text = (val + Math.Abs(damage)).ToString();
                        }
                        else
                        {
                            if (Avoided[avd])
                            {
                                txt[avd].text = "Miss";
                            }
                            else
                            {
                                if (damage < 2)
                                {

                                    txt[avd].text = Math.Round(val + damage, 1).ToString();
                                }
                                else
                                {
                                    txt[avd].text = (Math.Round((avd != txt.Length - 1) ? val + Mathf.Floor(damage) : val + (damage * Avoided.Length - Mathf.Floor(damage) * (Avoided.Length - 1)), 1).ToString());
                                }
                            }
                        }
                    }
                }
            }

            if (Hp <= this.unitObject.unitProperty.Hp / 3)
                EnemyControll.enemyControll.NeedRefreshPos = true;

            if (spell != null && effects != null)
                if (spell.spellClass != classType.none)
                {
                    SpecClassSpellPassive.spec.UseSpell(spell, myUnit, spell.unitEvents.MyUnit);
                }

            if (unitLogic != null && unitEvents.OnGetDamage != null)
            {
                unitEvents.OnGetDamage.Invoke();
            }

            bool PutEvents = true;

            if (effects != null)
                foreach (Effect effect in effects)
                {
                    if (effect == null) continue;
                    //effect.effectStacking == EffectStacking.none
                    bool stop = false;
                    Effect eff = (Effect)effect.Clone();
                    eff.spell = spell;
                    eff.EffectFunction();

                    for (int i = 0; i < unitEffects.Count; i++)
                    {
                        if (unitEffects[i].Name == eff.Name)
                        {
                            PutEvents = false;
                            switch (effect.effectStacking)
                            {
                                case EffectStacking.Refresh:
                                    if (!zeroDuration)
                                        unitEffects[i].Duration = eff.Duration;
                                    stop = true;
                                    break;
                                case EffectStacking.Prolong:
                                    if (!zeroDuration)
                                        unitEffects[i].Duration += eff.Duration;
                                    stop = true;
                                    break;
                                case EffectStacking.Stack:
                                    if (unitEffects[i].stacks < eff.MaxImpact)
                                    {
                                        ChangeValue(eff.ImpactValue, eff.spellType);
                                        unitEffects[i].stacks++;
                                        unitEffects[i].ImpactValue.value += eff.ImpactValue.value;
                                    }
                                    unitEffects[i].Duration = Math.Max(unitEffects[i].Duration, eff.Duration);
                                    stop = true;
                                    break;
                            }
                            break;
                        }
                        else
                        {
                        }
                    }
                    if (zeroDuration)
                    {
                        eff.Duration = 0;
                    }

                    if (!stop)
                    {
                        ChangeValue(eff.ImpactValue, eff.spellType);
                        unitEffects.Add(eff);
                    }
                    if (InstantAction) { eff.Duration++; eff.Execute((Unit)MyLogic); }
                }

            if (spell != null)
                if (PutEvents && spell.unitEvents.AfterDead != null && spell.unitEvents.AfterDead.GetPersistentEventCount() > 0 && !UnderLink(spell))
                {
                    spell.linkedUnits.Add(this);
                    unitEvents.AfterDead.AddListener(() => { if (UnderSpell(spell)) spell.unitEvents.AfterDead.Invoke(); });
                }

        }

        public void ChangeValue(Impact impact, string property = "Hp")
        {
            if (property == "Hp" || property.Length == 0) return;

            try
            {

                bool isProp = false;
                bool haveMin = false;
                if (property.StartsWith("*"))
                {
                    property = property.Replace("*", "");
                    isProp = true;
                }
                if (property.EndsWith("^"))
                {
                    property = property.Replace("^", "");
                    haveMin = true;
                }
                object val = null;
                if (isProp)
                {
                    val = unitProperty.GetType().GetField(property).GetValue(unitProperty);
                }
                else
                {
                    val = GetType().GetField(property).GetValue(this);
                }

                float fVal = 0;
                if (val is int)
                {
                    fVal = Convert.ToInt32(val);
                }
                else if (val is SafeInt)
                {
                    fVal = (SafeInt)Convert.ToInt32(val);
                }
                else if (val is SafeFloat)
                {
                    fVal = (SafeFloat)val;
                }
                else
                {
                    fVal = (float)val;
                }
                if (!impact.isProcent)
                {
                    fVal -= impact.value;
                }
                else
                {
                    fVal = fVal / 100 * impact.value;
                }
                if (haveMin && fVal < 0) fVal = 0;

                object endvalue;
                if (val is int)
                {
                    endvalue = Convert.ToInt32(Math.Ceiling(fVal));
                }
                else if (val is SafeInt)
                {
                    endvalue = (SafeInt)Convert.ToInt32(Math.Ceiling(fVal));
                }
                else if (val is SafeFloat)
                {
                    endvalue = (SafeFloat)fVal;
                }
                else
                {
                    endvalue = fVal;
                }
                if (isProp)
                {
                    unitProperty.GetType().GetField(property).SetValue(unitProperty, endvalue);
                }
                else
                {
                    GetType().GetField(property).SetValue(this, endvalue);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void Death(Logic killer)
        {
            //DeathProcces
            if (killer != null)
                Helper.lstKiller = killer;
            Helper.lstDeathEnemy = (Logic)Clone();

            if (transform.GetComponent<UnitLogic>())
            {
                DestroyObject.Destroy(transform.gameObject);

                if (BattleControll.battleControll.UnitInfo == (Unit)transform.GetComponent<UnitLogic>())
                {
                    BattleControll.battleControll.UnitInfoHide();
                }

                if (BattleControll.LoadedLevel.enemySpawn == LevelObject.EnemySpawn.afterDead)
                {
                    BattleControll.battleControll.StartCoroutine(BattleLogic.battleLogic.coroutines.actionAfterSomeSec(0.5f, BattleControll.battleControll.SpawnEnemies));
                }
            }
            else
            {
                //heroDeath
            }
            if (unitEvents.AfterDead != null)
                BattleLogic.battleLogic.addAction(() =>
                {

                    unitEvents.AfterDead.Invoke();

                }, null, 0);
        }

        public void EffectsTick(Effect.actionCall OnStart)
        {
            for (int i = unitEffects.Count - 1; i >= 0; i--)
            {
                Effect effect = unitEffects[i];
                if (effect.ActionCall == OnStart)
                    if (effect.Execute(myUnit))
                    {
                        unitEffects.Remove(effect);
                    }
            }
        }

        public Logic(UnitObject unitObject, Transform transform, Transform unitImg, Unit myUnit, Slider HpSlider = null)
        {
            if (this.unitObject == null)
                this.unitObject = (UnitObject)unitObject.Clone();
            this.myUnit = myUnit;
            this.transform = transform;
            ReloadStartPosition(transform.position);

            unitImage = unitImg;
            hpSlider = HpSlider;
            UnitAction = unitObject.ActionsPerTurn;
            attackType = unitObject.attackType;
            if (attackType != null)
                attackType.ApplyUnit(myUnit);
            Init();
        }

        private void Init()
        {
            unitProperty = (UnitProperty)unitObject.unitProperty.Clone();
            unitEvents = new EventAction();
            unitEvents.MyUnit = myUnit;
            for (int i = 0; i < unitObject.Spells.Length; i++)
            {
                unitObject.Spells[i].linkedUnits.Add(this);

                int quate = i;
                //OnAttack
                if (unitObject.Spells[i].unitEvents.OnAttack.GetPersistentEventCount() > 0)
                {
                    unitEvents.OnAttack.AddListener(() =>
                    {
                        if (unitObject.Spells[quate].passiveSettings.IsPassiveSkill || UnderSpell(unitObject.Spells[quate]))
                            unitObject.Spells[quate].unitEvents.OnAttack.Invoke();
                    });
                }
                //OnGetDamage
                if (unitObject.Spells[i].unitEvents.OnGetDamage.GetPersistentEventCount() > 0)
                {
                    unitEvents.OnGetDamage.AddListener(() =>
                    {
                        if (unitObject.Spells[quate].passiveSettings.IsPassiveSkill || UnderSpell(unitObject.Spells[quate]))
                            unitObject.Spells[quate].unitEvents.OnGetDamage.Invoke();
                    });
                }
                //AfterDead
                if (unitObject.Spells[i].unitEvents.AfterDead.GetPersistentEventCount() > 0)
                {
                    unitObject.Spells[i].linkedUnits.Add(this);
                    unitEvents.AfterDead.AddListener(() =>
                    {
                        if (unitObject.Spells[quate].passiveSettings.IsPassiveSkill || UnderSpell(unitObject.Spells[quate]))
                            unitObject.Spells[quate].unitEvents.AfterDead.Invoke();
                    });
                }
                //BattleCry
                if (unitObject.Spells[i].unitEvents.BattleCry.GetPersistentEventCount() > 0)
                {
                    unitObject.Spells[i].linkedUnits.Add(this);
                    unitEvents.BattleCry.AddListener(() =>
                    {
                        if (unitObject.Spells[quate].passiveSettings.IsPassiveSkill || UnderSpell(unitObject.Spells[quate]))
                            unitObject.Spells[quate].unitEvents.BattleCry.Invoke();
                    });
                }
            }

            if (unitObject.unitActions.AfterDead != null && unitObject.unitActions.AfterDead.GetPersistentEventCount() > 0)
            {
                unitEvents.AfterDead.AddListener(() => unitObject.unitActions.AfterDead.Invoke());
            }
            if (unitObject.unitActions.BattleCry != null && unitObject.unitActions.BattleCry.GetPersistentEventCount() > 0)
            {
                unitEvents.BattleCry.AddListener(() => unitObject.unitActions.BattleCry.Invoke());
            }
            if (unitObject.unitActions.OnAttack != null && unitObject.unitActions.OnAttack.GetPersistentEventCount() > 0)
            {
                unitEvents.OnAttack.AddListener(() => unitObject.unitActions.OnAttack.Invoke());
            }
            if (attackType != null)
                if (attackType.AttackPerk != null)
                {
                    if (attackType.AttackPerk.unitEvents.OnAttack.GetPersistentEventCount() > 0)
                        unitEvents.OnAttack.AddListener(() =>
                        {
                            if (unitObject.attackType.Equals(attackType))
                            {
                                attackType.AttackPerk.unitEvents.OnAttack.Invoke();
                            }
                        });
                }

            if (unitObject.unitActions.OnGetDamage != null && unitObject.unitActions.OnGetDamage.GetPersistentEventCount() > 0)
            {
                unitEvents.OnGetDamage.AddListener(() => unitObject.unitActions.OnGetDamage.Invoke());
            }

            if (unitEvents.BattleCry.GetPersistentEventCount() > 0)
                unitEvents.BattleCry.Invoke();

        }

        public bool UnderSpell(Spell spell)
        {
            for (int i = 0; i < spell.effects.Length; i++)
            {
                for (int q = 0; q < unitEffects.Count; q++)
                {
                    if (unitEffects[q].Name == spell.effects[i].Name)
                        return true;
                }
            }

            return false;
        }

        public Effect UnderSpellEff(Spell spell)
        {
            for (int i = 0; i < spell.effects.Length; i++)
            {
                for (int q = 0; q < unitEffects.Count; q++)
                {
                    if (unitEffects[q].Name == spell.effects[i].Name)
                        return unitEffects[q];
                }
            }

            return null;
        }

        public bool UnderLink(Spell spell)
        {
            for (int i = 0; i < spell.linkedUnits.Count; i++)
            {
                if (spell.linkedUnits[i] == this)
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispell(Spell spell, int st = 0, bool proc = false)
        {
            for (int i = 0; i < spell.effects.Length; i++)
            {
                for (int q = 0; q < unitEffects.Count; q++)
                {
                    if (unitEffects[q].Name == spell.effects[i].Name)
                        if (unitEffects[q].Dispelling(myUnit, st, proc)) unitEffects.RemoveAt(q);
                }
            }
        }

        public void ReloadStartPosition(Vector3 pos)
        {
            vc = pos;
        }

        public void AttackUnit(Unit unit, float cost)
        {
            if (!CanAttack(unit) || transform == null) return;
            if (unit.unitlogic.unitProperty.Hp <= 0)
            {
                return;
            }

            if (transform.GetComponent<HeroLogic>())
            {
                transform.GetComponent<HeroLogic>().Energy -= cost;
                BattleLogic.battleLogic.addAction(() =>
                {

                    if (unit.unitlogic.Hp > 0 && !unit.unitlogic.HasBlocked(myUnit))
                    {
                        SetPos(unit.unitlogic.transform.position);
                    }
                    else
                    {
                        transform.GetComponent<HeroLogic>().Energy += cost;
                        momentalEnd = true;
                    }
                }, HasNear);
                BattleLogic.battleLogic.addAction(() =>
                {
                    if (unit.unitlogic.Hp > 0 && !unit.unitlogic.HasBlocked(myUnit))
                    {
                        Helper.lstOffender = this;
                        Helper.lstTargetEnemy = unit.unitlogic;
                        LastDamaget = unit.unitlogic;
                        unitEvents.OnAttack.Invoke();
                        unit.unitlogic.TakeImpact(new Impact { value = Damage, isProcent = false, }, this);
                        if (attackType != null)
                            attackType.AttackPerk.Execute(null, unit, true, 0);
                    }
                }, null);
                BattleLogic.battleLogic.addAction(() =>
                {
                    SetPos(vc);
                }, HasNear);
            }
            else
            {
                BattleLogic.battleLogic.addAction(() =>
                {
                    UnitAction -= (int)cost;
                    if (!CanAttack(unit)) return;
                    if (unit.unitlogic.Hp > 0)
                    {
                        SetPos(unit.unitlogic.transform.position);
                    }
                }, HasNear);
                BattleLogic.battleLogic.addAction(() =>
                {
                    if (transform == null) return;
                    if (!CanAttack(unit)) return;
                    Helper.lstOffender = this;
                    Helper.lstTargetEnemy = unit.unitlogic;
                    LastDamaget = unit.unitlogic;
                    unitEvents.OnAttack.Invoke();
                    unit.unitlogic.TakeImpact(new Impact { value = Damage, isProcent = false, }, this);
                    if (attackType != null && attackType.AttackPerk != null)
                        attackType.AttackPerk.Execute(null, unit, true, 0);

                }, null);
                BattleLogic.battleLogic.addAction(() =>
                {
                    if (transform == null) return;
                    SetPos(vc);
                }, HasNear);
            }
        }

        public void SetPos(Vector3 vc)
        {
            if (transform != null)
            {
                transform.position = vc;
                unitImage.GetComponent<ImageMove>().Setup();
            }
        }

        public void AddCallingSpell(Spell spell)
        {
            if (callingSpells == null) callingSpells = new List<Spell>();
            if (callingSpells.Contains(spell)) return;
            callingSpells.Add(spell);
        }

        public void CallLinkedSpells()
        {
            if (callingSpells == null) return;
            for (int i = 0; i < callingSpells.Count; i++)
            {
                if (callingSpells[i].unitEvents.BattleCry.GetPersistentEventCount() > 0)
                    if (callingSpells[i].passiveSettings.IsPassiveSkill || UnderSpell(callingSpells[i]))
                    {
                        callingSpells[i].Execute(null, null, true, 0);
                    }
            }
        }

        public bool HasNear()
        {
            if (unitImage == null || transform == null) return true;
            if (momentalEnd)
            {
                momentalEnd = false;
                return true;
            }

            if (Vector3.Distance(unitImage.position, transform.position) > GameManager.AttackReactionDistance)
            {

                return false;
            }
            return true;
        }

        public object Clone()
        {
            return new Logic(unitObject, transform, unitImage, myUnit, hpSlider)
            {
                unitEffects = unitEffects,
                unitEvents = unitEvents,
                hpSlider = hpSlider,
                momentalEnd = momentalEnd,
                vc = vc,
                unitObject = unitObject,
                transform = transform,
                unitImage = unitImage,
                myUnit = myUnit,
                unitProperty = (UnitProperty)unitProperty.Clone(),
            };

        }

        public bool HasBlocked(Unit unit = null)
        {
            if (unit != null && unit.unitObject.IsRangeUnit) return false;
            UnitLogic unitLogic = transform.GetComponent<UnitLogic>();
            if (unitLogic != null && BattleControll.battleControll.EnemyLines > 1 && unitLogic.position.y == BattleControll.battleControll.EnemyLines - 1)
            {
                if (unitLogic.position.x > 0)
                {
                    if (BattleControll.battleControll.ConstainsPositionUnit(unitLogic.position.x - 1) == null)
                        return false;
                }
                if (unitLogic.position.x < BattleControll.LoadedLevel.EnemyRows - 1)
                {
                    if (BattleControll.battleControll.ConstainsPositionUnit(unitLogic.position.x + 1) == null)
                        return false;
                }

                UnitLogic lg = BattleControll.battleControll.ConstainsPositionUnit(unitLogic.position.x);
                if (lg != unitLogic && lg != null)
                {
                    return true;
                }
            }
            else if (unit != null && BattleControll.battleControll.EnemyLines > 1)
            {
                UnitLogic un = (transform.GetComponent<UnitLogic>()) ? transform.GetComponent<UnitLogic>() : unit.unitlogic.transform.GetComponent<UnitLogic>();
                HeroLogic hr = (transform.GetComponent<HeroLogic>()) ? transform.GetComponent<HeroLogic>() : unit.unitlogic.transform.GetComponent<HeroLogic>();
                //   if (un.position.y == BattleControll.battleControll.EnemyLines - 1) return false;
                if (un != null && hr != null)
                {
                    if (Math.Abs(hr.unitlogic.position - un.unitlogic.position) < .6f)
                    {
                        if (BattleControll.battleControll.ConstainsPositionUnit(un.position.x) == null)
                            return false;
                        else
                            return true;
                    }
                    if (hr.unitlogic.position > un.unitlogic.position)
                    {
                        for (int i = un.position.x + 1; i < Mathf.FloorToInt(hr.unitlogic.position) + 1; i++)
                        {
                            if (BattleControll.battleControll.ConstainsPositionUnit(i) != null)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        for (int i = un.position.x - 1; i > Mathf.CeilToInt(hr.unitlogic.position) - 1; i--)
                        {
                            if (BattleControll.battleControll.ConstainsPositionUnit(i) != null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool CanUseSpecSpell()
        {
            if (attackType == null) return false;
            SpellPart sp = attackType.AvailableSpell;
            if (sp != null)
                return sp.Available(attackType);
            return false;
        }

        public bool CanAttack(Unit unit)
        {
            if (transform == null) return false;
            UnitLogic lg = transform.GetComponent<UnitLogic>();
            if (lg != null)
            {
                if (lg.unitlogic.UnitAction >= 0)
                    if (lg.position.y > 0 || unitObject.IsRangeUnit || !HasBlocked(unit))
                        return true;
                return false;
            }
            return true;
        }

        public float MissChanse(Logic unit)
        {
            float pos = 0;
            pos = position - unit.position;
            return Math.Max(Math.Abs(pos * GameManager.MissChansePerPosition) + unitProperty.Evasion, 0);
        }
    }
}
