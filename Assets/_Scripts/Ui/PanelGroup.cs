using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGroup : MonoBehaviour
{
    public GameObject[] panels;
    public TabGroup tabGroup;
    public int panelIndex;


    // Start is called before the first frame update
    void Awake()
    {
        ShowCurrentPAnel();
    }

    void ShowCurrentPAnel()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == panelIndex) panels[i].gameObject.SetActive(true);
            else panels[i].gameObject.SetActive(false);
            
        }
    }

    public void SetPageIndex (int index)
    {
        panelIndex = index;
        ShowCurrentPAnel();
    }
}
