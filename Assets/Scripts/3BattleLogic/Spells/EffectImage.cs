using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HeroLeft.Interfaces;
namespace HeroLeft.BattleLogic
{

    [CreateAssetMenu(menuName = "SpecEffect", fileName = "New Spec Effect", order = 51)]
    public class EffectImage : ScriptableObject
    {

        public bool IsUniversal = false;
        public Sprite Img;
        public EffectType effectType;

        public enum EffectType
        {
            none,
            _2D,
            _3D,
        }
    }

    public enum EffectStacking
    {
        Refresh,
        Prolong,
        Stack,
        none,
    }

    [Serializable]
    public class Effect : CommandHandler, ICloneable
    {
        public EffectImage VisualEffect;

        public int Duration;
        public Impact ImpactValue;
        public float MaxImpact;
        public actionCall ActionCall = actionCall.OnEndTurn;

        public EffectStacking effectStacking;
        [HideInInspector] public int stacks = 1;

        public string spellType;
        public Spell.SpellTarget spellTarget;
        public Spell.SplashType splashType;
        [HideInInspector] public Spell spell;

        public bool Execute(Unit target)
        {
            Duration--;

            if (!spellType.StartsWith("*"))
                spell.DamageAction(target, splashType, spellTarget, ImpactValue, false, spellType);
            if (Duration <= 0 && spellType != "Hp") Dispelling(target);
            return Duration <= 0;
        }
        public void PutEffect(Unit target)
        {
            spell.DamageAction(target, splashType, spellTarget, new Impact { value = 0 }, true, spell.spellType, true, false);
        }

        public bool Dispelling(Unit target, int st = 0, bool proc = false)
        {
            if (spellType != "Hp")
            {
                if (st == 0) {
                    target.unitlogic.ChangeValue(new Impact { value = -ImpactValue.value, isProcent = ImpactValue.isProcent }, spellType);
                    return true;
                }
                else if (proc == false)
                {
                    target.unitlogic.ChangeValue(new Impact { value = -(ImpactValue.value / stacks * st), isProcent = ImpactValue.isProcent }, spellType);
                    stacks -= st;
                }
                else
                {
                    int it = Mathf.RoundToInt((float)stacks * st / 100f);
                    target.unitlogic.ChangeValue(new Impact { value = -(ImpactValue.value / stacks * it), isProcent = ImpactValue.isProcent }, spellType);
                    stacks -= it;
                }
                return stacks <= 0;
            }
            return true;
        }

        public object Clone()
        {
            return new Effect()
            {
                VisualEffect = VisualEffect,
                Duration = Duration,
                ImpactValue = (Impact)ImpactValue.Clone(),
                spellType = spellType,
                spellTarget = spellTarget,
                Function = Function,
                Name = Name,
                spell = spell,
                ActionCall = ActionCall,
                effectStacking = effectStacking,
                splashType = splashType,
                MaxImpact = MaxImpact,
                stacks = stacks,
            };
        }

        [Flags]
        public enum actionCall : uint
        {
            OnEndTurn = 1,
            OnStartTurn = 2,
        }
    }
}

namespace HeroLeft.Interfaces
{
    public class CommandHandler
    {
        public string Name;
        [Multiline] public string Function;

        public void EffectFunction()
        {
            try
            {
                if (Function == null || Function.Length == 0) return;
                string[] fnc = Function.Split('\n');
                foreach (string function in fnc)
                {
                    string[] cmd = function.Split(';');
                    List<command> commands = new List<command>();
                    for (int i = -1; i < cmd.Length; i += 3)
                    {
                        string op = "";
                        if (i == -1) op = null;
                        else op = cmd[i];
                        Type type = (cmd[i + 1] != "this") ? Type.GetType(cmd[i + 1]) : this.GetType();
                        bool fld = true;
                        bool justvalue = false;
                        if (cmd[i + 2].StartsWith("*"))
                        {
                            cmd[i + 2] = cmd[i + 2].Remove(0, 1);
                            fld = false;
                        }
                        char ch = cmd[i + 2][0];
                        if (ch == '0' || ch == '1' || ch == '2' || ch == '3' || ch == '4' || ch == '5' || ch == '6' || ch == '7' || ch == '8' || ch == '9')
                        {
                            justvalue = true;
                        }

                        object field = null;
                        if (fld)
                        {
                            if (justvalue)
                            {
                                field = cmd[i + 2];
                            }
                            else
                            {
                                field = type.GetField(cmd[i + 2]);
                            }
                        }
                        else
                        {
                            if (justvalue)
                            {
                                field = cmd[i + 2];
                            }
                            else
                            {
                                field = type.GetProperty(cmd[i + 2]);
                            }
                        }
                        commands.Add(new command(field, op, fld, justvalue, cmd[i + 1] != "this"));
                    }
                    command mainField = commands[0];
                    object mainType = commands[0].b ? null : this;
                    foreach (command command in commands)
                    {
                        if (command.field == mainField) continue;
                        CommandWorker(command, mainField, mainType);
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        private void CommandWorker(command command, command mainField, object mainType)
        {
            if (command._operator != null)
            {
                object tp = (command.b) ? null : this;
                FieldInfo fieldInfo = mainField.fild ? (FieldInfo)mainField.field : null;
                PropertyInfo propertyInfo = !mainField.fild ? (PropertyInfo)mainField.field : null;

                object startValue = mainField.fild ? fieldInfo.GetValue(mainType) : propertyInfo.GetValue(mainType);
                object endValue = 0;
                bool fld = command.fild;

                if (!command.justValue)
                {
                    fieldInfo = fld ? (FieldInfo)command.field : null;
                    propertyInfo = !fld ? (PropertyInfo)command.field : null;
                }
                if (startValue is int || startValue is Interfaces.SafeInt)
                {
                    int val = (command.justValue) ? Convert.ToInt32(command.field) : Convert.ToInt32(fld ? fieldInfo.GetValue(tp) : propertyInfo.GetValue(tp));
                    switch (command._operator)
                    {
                        case "=":
                            endValue = val;
                            break;
                        case "+":
                            endValue = (int)startValue + val;
                            break;
                        case "-":
                            endValue = (int)startValue - val;
                            break;
                        case "*":
                            endValue = (int)startValue * val;
                            break;
                        case "/":
                            endValue = (int)startValue / val;
                            break;
                    }

                    fieldInfo = mainField.fild ? (FieldInfo)mainField.field : null;
                    propertyInfo = !mainField.fild ? (PropertyInfo)mainField.field : null;
                    int value = 0;
                    if (endValue.GetType() == typeof(Interfaces.SafeInt)) value = (Interfaces.SafeInt)endValue;
                    else value = (int)endValue;

                    if (mainField.fild)
                    {
                        fieldInfo.SetValue(mainType, value);
                    }
                    else
                    {
                        propertyInfo.SetValue(mainType, value);
                    }
                }
                else
                {
                    float val = (command.justValue) ? Convert.ToSingle(command.field) : Convert.ToSingle(fld ? fieldInfo.GetValue(tp) : propertyInfo.GetValue(tp));
                    switch (command._operator)
                    {
                        case "=":
                            endValue = val;
                            break;
                        case "+":
                            endValue = (float)startValue + val;
                            break;
                        case "-":
                            endValue = (float)startValue - val;
                            break;
                        case "*":
                            endValue = (float)startValue * val;
                            break;
                        case "/":
                            endValue = (float)startValue / val;
                            break;
                    }
                    fieldInfo = mainField.fild ? (FieldInfo)mainField.field : null;
                    propertyInfo = !mainField.fild ? (PropertyInfo)mainField.field : null;
                    float value = 0;
                    if (endValue.GetType() == typeof(Interfaces.SafeFloat)) value = (Interfaces.SafeFloat)endValue;
                    else value = (float)endValue;

                    if (mainField.fild)
                    {
                        fieldInfo.SetValue(mainType, value);
                    }
                    else
                    {
                        propertyInfo.SetValue(mainType, value);
                    }
                }
            }
        }
        private class command
        {
            public object field;
            public string _operator;
            public bool fild = true;
            public bool b;
            public bool justValue = false;

            public command(object f, string op, bool fld, bool justValue = false, bool b = false)
            {
                field = f;
                this.justValue = justValue;
                _operator = op;
                fild = fld;
                this.b = b;
            }
        }
    }
}
