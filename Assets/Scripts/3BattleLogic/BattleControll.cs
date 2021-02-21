using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace HeroLeft.BattleLogic
{
    public class BattleControll : MonoBehaviour
    {
        public static LevelObject loadedLevel;
        public static BattleControll battleControll;
        public static HeroLogic heroLogic;

        [SerializeField] private Image backGround;
        [SerializeField] private GameObject winPanel;
        public Transform unitImages;

        [Header("TurnControll")]
        public TurnController turnController;
        public int turnLenght = 30;

        [Header("Enemy Settings")]
        public GridGroup enemyUnitsParent;
        public Transform enemyQueue;
        [SerializeField] private UnitLogic enemyPrefab;
        public Transform interactiveParent;

        [Header("Enemy info")]
        public GameObject enemyInfo;
        public Slider enemyHp;
        [SerializeField] private Image enemyAvatar;
        [SerializeField] private Text nameTag;
        public Unit unitInfo;

        [Header("Dummy Settings")]
        public Transform dummyParent;

        [Header("Enemy Effects")]
        [SerializeField] private Transform emEffects;
        [SerializeField] private GameObject effectPrefab;

        [Header("Hero Spell")]
        public Transform hrEffects;
        public SpellLogic attackSpell;

        public int EnemyLines { get { return loadedLevel.EnemiesOnField / loadedLevel.EnemyRows; } }

        private void Awake()
        {
            if (loadedLevel == null) { GoMenu(); return; }
            BattleLogic.battleLogic = new BattleLogic();

            battleControll = this;
            backGround.sprite = loadedLevel.BackGround;
            //Set Enemy Settings
            enemyUnitsParent.InitGrid(loadedLevel.EnemyRows);

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
            foreach (UnitObject unitObject in loadedLevel.Units)
            {
                SpawnPerson(unitObject);
            }
        }

        public void SpawnEnemies()
        {
            while (enemyUnitsParent.transform.childCount < loadedLevel.EnemiesOnField && enemyQueue.childCount > 0)
            {
                Transform enemy = enemyQueue.GetChild(0);
                enemy.SetParent(enemyUnitsParent.transform);
                UnitLogic logic = enemy.GetComponent<UnitLogic>();
                enemyUnitsParent.NewChilder(logic);
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
            if (enemyUnitsParent.transform.childCount == 0 && enemyQueue.childCount == 0)
            {
                winPanel.SetActive(true);
            }
            EnemyControll.enemyControll.NeedRefreshPos = true;
        }

        private UnitLogic SpawnPerson(UnitObject unitObject)
        {
            UnitLogic enemy = Instantiate<UnitLogic>(enemyPrefab, enemyQueue);
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
            enemyUnitsParent.NewChilder(unit);
            unit.unitlogic.unitImage.GetComponent<ImageMove>().Setup();
        }

        public void UnitInfoShow(UnitLogic unit)
        {
            enemyInfo.SetActive(true);

            unitInfo = unit;
            enemyHp.maxValue = unit.unitObject.unitProperty.Hp;
            enemyHp.value = unit.unitlogic.Hp;
            enemyHp.GetComponentInChildren<Text>().text = System.Math.Round(unit.unitlogic.Hp, 1) + "/" + unit.unitObject.unitProperty.Hp;
            enemyAvatar.sprite = unit.unitObject.ico.img;
            enemyAvatar.GetComponent<RectTransform>().localScale = (unit.unitObject.ico.size == Vector2.zero) ? Vector2.one : unit.unitObject.ico.size;
            enemyAvatar.GetComponent<RectTransform>().pivot = (unit.unitObject.ico.pos == Vector2.zero) ? Vector2.one / 2 : unit.unitObject.ico.pos;
            nameTag.text = unit.UnitName;
            ShowUnitEffects(unit);

        }

        public void UnitInfoHide()
        {
            enemyInfo.SetActive(false);
        }

        public void RefreshHeroAttackSpell()
        {
            if (heroLogic.unitlogic.attackType == null) return;

            Spell spell = heroLogic.unitlogic.attackType.AvailableSpell.spell;
            if (attackSpell.spellImage != spell)
            {
                attackSpell.spellImage = spell;
                attackSpell.Start();
            }

            if (attackSpell.spellInBattle.RestTurn > 0) return;

            AttackSpellTurn(heroLogic.unitlogic.CanUseSpecSpell());
        }

        public void ShowUnitEffects(UnitLogic unit)
        {
            for (int i = 0; i < emEffects.childCount; i++)
            {
                Destroy(emEffects.GetChild(i).gameObject);
            }

            foreach (Effect effect in unit.unitlogic.unitEffects)
            {
                if (effect.Duration == 0) continue;
                GameObject obj = Instantiate<GameObject>(effectPrefab, emEffects);
                if (effect.effectStacking != EffectStacking.Stack)
                    obj.GetComponentInChildren<Text>().text = effect.Duration.ToString();
                else
                    obj.GetComponentInChildren<Text>().text = effect.stacks.ToString();

                obj.GetComponent<Image>().sprite = (effect.VisualEffect == null || effect.VisualEffect.Img == null) ? effect.spell.uImage.img : effect.VisualEffect.Img;
            }
        }

        public UnitLogic ConstainsPositionUnit(int x)
        {
            return enemyUnitsParent.units[x, EnemyLines - 1];
        }

        public void AttackSpellTurn(bool bl)
        {
            attackSpell.spellInBattle.active(bl);
        }

        public void HeroEffectsRefresh()
        {
            int effs = heroLogic.unitlogic.unitEffects.Count;
             
            if (effs < hrEffects.childCount)
            {
                for(int i = hrEffects.childCount; i < effs; i--)
                {
                    Destroy(hrEffects.GetChild(i - 1).gameObject);
                }
            }else if(effs > hrEffects.childCount)
            {
                for(int i = hrEffects.childCount; i < effs; i++)
                {
                    Instantiate(effectPrefab, hrEffects);
                }
            }

            for(int i = 0; i < effs; i++)
            {
                Effect ef = heroLogic.unitlogic.unitEffects[i];
                Transform obj = hrEffects.GetChild(i);

                if (ef.effectStacking != EffectStacking.Stack)
                    obj.GetComponentInChildren<Text>().text = ef.Duration.ToString();
                else
                    obj.GetComponentInChildren<Text>().text = ef.stacks.ToString();

                obj.GetComponent<Image>().sprite = (ef.VisualEffect == null || ef.VisualEffect.Img == null) ? ef.spell.uImage.img : ef.VisualEffect.Img;
            }
        }

    }
}
