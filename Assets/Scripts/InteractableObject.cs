using UnityEngine;

public class InteractableObject : MonoBehaviour, Interfaces.IInteractable
{
    [SerializeField] private Inventory.InventoryItem item;
    
    public void Interact()
    {
        Debug.Log("Interaction happened");
        
        var leftover = Inventory.Instance.AddItemToInventory(item, item.quantity);
        if (leftover <= 0)
        {
            Destroy(gameObject);
            Debug.Log(gameObject.name + " has been destroyed as all of it has been added to the inventory");
        }
        else
        {
            item.quantity = leftover;
            Inventory.Instance.UpdateInventoryIcons();
            Debug.Log($"The inventory was full this item has {leftover} amount to be added to the inventory");
        }
    }
}
