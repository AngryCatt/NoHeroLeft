#if UNITY_EDITOR
using HeroLeft.BattleLogic;
using HeroLeft;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

[ExecuteInEditMode]
public class GMShow : EditorWindow {

    [MenuItem("GameManager/AdminPanel")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GMShow));
    }
    public int selectedPopup = 0;
    public int heroAttackType = 0;
    public int heroImage = 0;

    [ExecuteInEditMode]
    private void OnGUI()
    {
        GUILayout.Label("Queue:");

        BattleLogic logic = BattleLogic.battleLogic;
        if (logic != null)
        {
            string s0 = "   BattleQueue: ";
            for (int i = 0; i < logic.BattleQueue.Count; i++)
            {
                s0 += logic.BattleQueue[i].Method.Name;
            }
            GUILayout.Label(s0);

            string s1 = "   MyTurnQueue: ";
            for (int i = 0; i < logic.MyturnQueue.Count; i++)
            {
                s1 += logic.MyturnQueue[i].unityAction.Method.Name;
            }
            GUILayout.Label(s1);

            string s2 = "   NextTurnQueue: ";
            for (int i = 0; i < logic.NextTurnQueue.Count; i++)
            {
                s2 += logic.NextTurnQueue[i].unityAction.Method.Name;
            }
            GUILayout.Label(s2);
        }

        GUILayout.Label("Enemies:");

        if(BattleControll.battleControll != null)
        {
            GridGroup gridGroup = BattleControll.battleControll.enemyUnitsParent;
            selectedPopup = EditorGUILayout.Popup("GridType", selectedPopup, new string[] { "UnitNames", "UnitAvailability", "MissChanse", "MissChanseOnMe", "CanAttack" });
            for (int y = 0; y < gridGroup.units.GetLength(1); y++)
            {
                string s = "";
                for (int x = 0; x < gridGroup.units.GetLength(0); x++)
                {
                    if (selectedPopup == 0)
                    {
                        s += (gridGroup.units[x, y] != null) ? gridGroup.units[x, y].UnitName + " " : "  _  ";
                    }
                    else if(selectedPopup == 1)
                    {
                        s += (gridGroup.units[x, y] != null) ? gridGroup.units[x, y].unitlogic.HasBlocked() + " " : " ___  ";
                    }
                    else if(selectedPopup == 2)
                    {
                        s += (gridGroup.units[x, y] != null) ? gridGroup.units[x, y].unitlogic.MissChanse(BattleControll.heroLogic.unitlogic) + " " : " _ ";
                    }
                    else if(selectedPopup == 3)
                    {
                        s += (gridGroup.units[x, y] != null) ? BattleControll.heroLogic.unitlogic.MissChanse(gridGroup.units[x, y].unitlogic) + " " : " _ ";
                    }
                    else
                    {
                        s += (gridGroup.units[x, y] != null) ? gridGroup.units[x, y].unitlogic.CanAttack(BattleControll.heroLogic) + " " : " ___  ";
                    }
                }
                GUILayout.Label("   " + s);
            }
        }
    }

}
#endif
