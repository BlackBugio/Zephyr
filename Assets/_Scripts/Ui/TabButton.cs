using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    public Image background;
    public string tabButtonText;
    public bool occupied = false;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;
    public UnityEvent onTabOccupied;
    public UnityEvent onTabDeOccupied;

    void Start()
    {
        background = GetComponent<Image>();
        transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = tabButtonText;
    }

    #region PointerHandle
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }
    #endregion

    public void Select()
    {
        if (onTabSelected != null) onTabSelected.Invoke();
    }
    public void Deselect()
    {
        if (onTabDeselected != null) onTabDeselected.Invoke();
    }

    public void Occupied()
    {
        if (onTabOccupied != null) onTabOccupied.Invoke();
    }

    public void DeOccupied()
    {
        if (onTabDeOccupied != null) onTabDeOccupied.Invoke();
    }

}
