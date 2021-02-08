using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitch : MonoBehaviour
{
    public int activePanel = -1;
    public int DefultPanel = -1;
    public bool DisableOnClick = false;
    public GameObject[] panels;
    

    private void OnEnable() {
        if (activePanel != DefultPanel) panels[activePanel].SetActive(false);
        foreach (GameObject panel in panels) {
            panel.SetActive(false);
        }
        if(DefultPanel >= 0) panels[DefultPanel].SetActive(true);
        activePanel = DefultPanel;
    }

    public void Show(int panel) {
        if(DisableOnClick && activePanel == panel)
        {
            return;
        }

        if (activePanel != -1) {
            panels[activePanel].SetActive(false);
            if (activePanel == panel) {
                activePanel = DefultPanel;
                return;
            }
        }
        panels[panel].SetActive(!panels[panel].activeSelf);
        activePanel = panel;
    }
}
