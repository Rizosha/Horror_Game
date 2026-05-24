using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnItemHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Inventory.InventoryItem item;

    public Inventory.InventoryItem Item
    {
        get => item;
        set => item = value;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse is over UI image.");
        canvasGroup.alpha = 1;

        if (item.itemData == null) return;
        
        Inventory.Instance.UpdateInventoryDescriptor(item.itemData.icon, item.itemData.itemName, item.itemData.itemDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse is no longer over UI image.");
        canvasGroup.alpha = 0;
    }
}