using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Sprite tabActive;
    public Sprite tabHover;
    public Sprite tabIdle;
    public TabButton selectedTab;
    public List<GameObject> objectsToSwap;
    public PanelGroup panelGroup;

    public delegate void TabActive(int tabIndex);
    public static event TabActive OnTabActive;
      
    public void Subscribe(TabButton button)
    {
        if (tabButtons == null) tabButtons = new List<TabButton>();
        tabButtons.Add(button);       
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.color = Color.white;
            //button.background.sprite = tabHover;
        }
    }
    public void OnTabExit(TabButton button)
    {
        if (button.occupied)
        {
            button.background.color = Color.red;
            //button.background.sprite = tabHover;
        }
        ResetTabs();
        //button.background.sprite = tabIdle;
    }

    public void OnTabOccupied(TabButton button)
    {
        button.background.color = Color.red;
        button.occupied = true;
    }

    public void OnTabDeOccupied(TabButton button)
    {
        button.occupied = false;
        button.background.color = Color.gray;
        ResetTabs();
    }
    public void OnTabSelected(TabButton button)
    {
        if (selectedTab != null) selectedTab.Deselect();

        selectedTab = button;
        selectedTab.Select();
        ResetTabs();
        if(!button.occupied)button.background.color = Color.white;
        //button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
                OnTabActive(i);
            }
            else objectsToSwap[i].SetActive(false);
        }        
        if (panelGroup != null)
        {
            panelGroup.SetPageIndex(selectedTab.transform.GetSiblingIndex());
        }
    }

    public void ResetTabs()
    {        
        foreach (TabButton button in tabButtons)
        {
            if (button == selectedTab || button.occupied) continue; 
            button.background.color = Color.gray;
            //button.background.sprite = tabIdle;
        }
    }   

}
