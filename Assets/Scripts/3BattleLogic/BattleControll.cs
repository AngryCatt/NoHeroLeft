using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace HeroLeft.BattleLogic
{
    public class BattleControll : MonoBehaviour
    {
        public static LevelObject LoadedLevel;
        public static BattleControll battleControll;
        public static HeroLogic heroLogic;

        [SerializeField] private Image BackGround;
        [SerializeField] private GameObject WinPanel;
        public Transform UnitImages;

        [Header("TurnControll")]
        public TurnController turnController;
        public int turnLenght = 30;

        [Header("Enemy Settings")]
        public GridGroup EnemyUnitsParent;
        public Transform EnemyQueue;
        [SerializeField] private UnitLogic EnemyPrefab;
        public Transform InteractiveParent;

        [Header("Enemy info")]
        public GameObject EnemyInfo;
        public Slider EnemyHp;
        [SerializeField] private Image EnemyAvatar;
        [SerializeField] private Text nameTag;
        public Unit UnitInfo;

        [Header("Dummy Settings")]
        public Transform DummyParent;

        [Header("Enemy Effects")]
        [SerializeField] private Transform EffectsParent;
        [SerializeField] private GameObject EffectPrefab;

        [Header("Hero Spell")]
        public SpellLogic AttackSpell;

        public int EnemyLines { get { return LoadedLevel.EnemiesOnField / LoadedLevel.EnemyRows; } }

        private void Awake()
        {
            if (LoadedLevel == null) { GoMenu(); return; }
            BattleLogic.battleLogic = new BattleLogic();

            battleControll = this;
            BackGround.sprite = LoadedLevel.BackGround;
            //Set Enemy Settings
            EnemyUnitsParent.InitGrid(LoadedLevel.EnemyRows);

            ReloadTimer();
        }

        public void GoMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScean");
        }

        private void Start()
        {
            SpawnEnemyQueue();
            SpawnEnemies();
        }

        private void ReloadTimer()
        {
            turnController.SetTimer(turnLenght);
        }

        private void SpawnEnemyQueue()
        {
            foreach (UnitObject unitObject in LoadedLevel.Units)
            {
                SpawnPerson(unitObject);
            }
        }

        public void SpawnEnemies()
        {
            while (EnemyUnitsParent.transform.childCount < LoadedLevel.EnemiesOnField && EnemyQueue.childCount > 0)
            {
                Transform enemy = EnemyQueue.GetChild(0);
                enemy.SetParent(EnemyUnitsParent.transform);
                UnitLogic logic = enemy.GetComponent<UnitLogic>();
                EnemyUnitsParent.NewChilder(logic);
                if (logic.unitObject.FirstLinePriority && logic.position.y == 0)
                {
                    EnemyControll.enemyControll.PositionReload(logic);
                }
                else if (logic.unitObject.IsRangeUnit && logic.position.y > 0)
                {
                    EnemyControll.enemyControll.PositionReload(logic);
                }

                enemy.gameObject.SetActive(true);
            }
            if (EnemyUnitsParent.transform.childCount == 0 && EnemyQueue.childCount == 0)
            {
                WinPanel.SetActive(true);
            }
            EnemyControll.enemyControll.NeedRefreshPos = true;
        }

        private UnitLogic SpawnPerson(UnitObject unitObject)
        {
            UnitLogic enemy = Instantiate<UnitLogic>(EnemyPrefab, EnemyQueue);
            enemy.unitObject = unitObject;
            enemy.transform.GetChild(0).position = new Vector3(Screen.width + enemy.GetComponent<RectTransform>().sizeDelta.x * 2, enemy.transform.position.y, 0);
            enemy.gameObject.SetActive(false);
            enemy.ReloadNameTag();
            enemy.InitLogic();

            return enemy;
        }

        public void Summon(UnitObject unitObject)
        {
            UnitLogic unit = SpawnPerson(unitObject);
            unit.gameObject.SetActive(true);
            EnemyUnitsParent.NewChilder(unit);
            unit.unitlogic.unitImage.GetComponent<ImageMove>().Setup();
        }

        public void UnitInfoShow(UnitLogic unit)
        {
            EnemyInfo.SetActive(true);

            UnitInfo = unit;
            EnemyHp.maxValue = unit.unitObject.unitProperty.Hp;
            EnemyHp.value = unit.unitlogic.Hp;
            EnemyHp.GetComponentInChildren<Text>().text = System.Math.Round(unit.unitlogic.Hp, 1) + "/" + unit.unitObject.unitProperty.Hp;
            EnemyAvatar.sprite = unit.unitObject.ico.img;
            EnemyAvatar.GetComponent<RectTransform>().localScale = (unit.unitObject.ico.size == Vector2.zero) ? Vector2.one : unit.unitObject.ico.size;
            EnemyAvatar.GetComponent<RectTransform>().pivot = (unit.unitObject.ico.pos == Vector2.zero) ? Vector2.one / 2 : unit.unitObject.ico.pos;
            nameTag.text = unit.UnitName;
            ShowUnitEffects(unit);

        }

        public void UnitInfoHide()
        {
            EnemyInfo.SetActive(false);
        }

        public void RefreshHeroAttackSpell()
        {
            if (heroLogic.unitlogic.attackType == null) return;

            Spell spell = heroLogic.unitlogic.attackType.AvailableSpell.spell;
            if (AttackSpell.spellImage != spell)
            {
                AttackSpell.spellImage = spell;
                AttackSpell.Start();
            }

            if (AttackSpell.spellInBattle.RestTurn > 0) return;

            AttackSpellTurn(heroLogic.unitlogic.CanUseSpecSpell());
        }

        public void ShowUnitEffects(UnitLogic unit)
        {
            for (int i = 0; i < EffectsParent.childCount; i++)
            {
                Destroy(EffectsParent.GetChild(i).gameObject);
            }

            foreach (Effect effect in unit.unitlogic.unitEffects)
            {
                if (effect.Duration == 0) continue;
                GameObject obj = Instantiate<GameObject>(EffectPrefab, EffectsParent);
                if (effect.effectStacking != EffectStacking.Stack)
                    obj.GetComponentInChildren<Text>().text = effect.Duration.ToString();
                else
                    obj.GetComponentInChildren<Text>().text = effect.stacks.ToString();

                obj.GetComponent<Image>().sprite = (effect.VisualEffect == null || effect.VisualEffect.Img == null) ? effect.spell.uImage.img : effect.VisualEffect.Img;
            }
        }

        public UnitLogic ConstainsPositionUnit(int x)
        {
            return EnemyUnitsParent.units[x, EnemyLines - 1];
        }

        public void AttackSpellTurn(bool bl)
        {
            AttackSpell.spellInBattle.active(bl);
        }
    }
}
