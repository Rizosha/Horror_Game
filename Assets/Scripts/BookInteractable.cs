using UnityEngine;

public class BookInteractable : MonoBehaviour, Interfaces.IInteractable
{
    [SerializeField] private Inventory.InventoryItem item;
    [SerializeField] private int amountToAdd;
    
    public void Interact()
    {
        Debug.Log("Interaction happened");
        
        var leftover = Inventory.Instance.AddItemToInventory(item, amountToAdd);
        if (leftover <= 0)
        {
            Destroy(gameObject);
            Debug.Log(gameObject.name + " has been destroyed as all of it has been added to the inventory");
        }
        else
        {
            amountToAdd = leftover;
            Inventory.Instance.UpdateInventoryIcons();
            Debug.Log($"The inventory was full this item has {leftover} amount to be added to the inventory");
        }
    }
}
