using UnityEngine;

namespace HeroLeft.BattleLogic {
    public class GridGroup : MonoBehaviour {

        public UnitLogic[,] units;
        public Vector2 Spacing;
        public Vector2 CellSize;
        Vector2Int vc;

        public void InitGrid(int rows)
        {
            units = new UnitLogic[rows, BattleControll.battleControll.EnemyLines];
        }

        public void NewChilder(UnitLogic unitLogic)
        {
            for (int y = units.GetLength(1)-1; y >= 0; y--)
            {
                for (int x = 0; x < units.GetLength(0); x++)
                {
                    if (units[x, y] == null)
                    {
                        vc = new Vector2Int(x, y);

                        if (y > 0 && unitLogic.unitObject.IsRangeUnit && BattleControll.battleControll.EnemyLines > 1) continue;
                        if (y == 0 && !unitLogic.unitObject.IsRangeUnit && BattleControll.battleControll.EnemyLines > 1) continue;

                        if (unitLogic != null)
                        {
                            units[x, y] = unitLogic;
                            units[x, y].SetPosition(new Vector2Int(x, y), GetPosition(x, y));
                            if (y == 0)
                                unitLogic.transform.SetAsFirstSibling();
                            else
                                unitLogic.transform.SetAsLastSibling();
                            return;
                        }
                    }
                }
            }
            units[vc.x, vc.y] = unitLogic;
            units[vc.x, vc.y].SetPosition(new Vector2Int(vc.x, vc.y), GetPosition(vc.x, vc.y));
            EnemyControll.enemyControll.NeedRefreshPos = true;

        }

        public Vector3 GetPosition(int x, int y)
        {
            Vector3 pos = transform.position;
            float xDelta = (x - ((float)units.GetLength(0) / 2 - .5f)) / GameManager.DeltaXScreen;
            pos.x += xDelta * Spacing.x + xDelta * CellSize.x;
            pos.y -= (y * Spacing.y + y * CellSize.y) / GameManager.DeltaYScreen;
            return pos;
        }
    }
}
