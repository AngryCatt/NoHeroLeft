using UnityEngine;
using UnityEngine.EventSystems;

namespace HeroLeft.BattleLogic {
    public class HeroDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

        [Header("Marks")]
        public Transform MarkParent;
        public GameObject MarkPrefab;

        [Header("Poses")]
        public float PositionDelta = 200f;
        public float EnergyCost = 10;
        public int myPos = 0;
        public int selectedPos = 0;

        private HeroLogic myhero;
        private int MaxPos = 3;
        private Vector3 startPos;

        public static HeroDrag heroDrag;

        public void OnBeginDrag(PointerEventData eventData)
        {

            if (BattleLogic.battleLogic.turnPosition == TurnPosition.MyTurn)
            {
                myPos = myhero.UnitPosition;
                MarkShow();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (BattleLogic.battleLogic.turnPosition != TurnPosition.MyTurn)
            {
                MarkParent.gameObject.SetActive(false);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (myhero.Energy < EnergyCost) {
                MarkParent.gameObject.SetActive(false);
                return; 
            }
            if (BattleLogic.battleLogic.turnPosition == TurnPosition.MyTurn)
            {

                if (selectedPos != 999)
                {
                    BattleControll.heroLogic.Energy -= EnergyCost * Mathf.Abs(myPos - selectedPos);
                    myPos = selectedPos;
                    SetPos();
                }
                MarkParent.gameObject.SetActive(false);
            }
        }

        public void SetPos()
        {
            BattleLogic.battleLogic.addAction(() =>
            {
                myhero.UnitPosition = myPos;
                myhero.transform.position = new Vector3(startPos.x + (PositionDelta * myPos) / GameManager.DeltaYScreen, transform.position.y, 0);
                myhero.unitlogic.ReloadStartPosition(myhero.transform.position);
                BattleControll.heroLogic.GetRealPos();
            }, null);
        }

        private void Awake()
        {
            heroDrag = this;
            myhero = transform.parent.GetComponent<HeroLogic>();
            startPos = transform.parent.position;
        }

        private void Start()
        {
            Init(BattleControll.loadedLevel.EnemyRows - 1);
        }

        public void Init(int MaxPos)
        {
            this.MaxPos = MaxPos;
            for (int i = 0; i < MaxPos * 2; i++)
            {
                Instantiate(MarkPrefab, MarkParent);
            }
            MarkParent.gameObject.SetActive(false);
        }

        public void MarkShow()
        {
            for (int i = -MaxPos; i < MarkParent.childCount / 2; i++)
            {
                GameObject mark = MarkParent.GetChild(i + MaxPos).gameObject;
                mark.transform.position = new Vector3(startPos.x + ((i < myPos) ? i : i + 1) * PositionDelta / GameManager.DeltaYScreen, transform.position.y, 0);
                mark.name = ((i < myPos) ? i : i + 1).ToString();
            }
            MarkParent.gameObject.SetActive(true);
        }
    }
}