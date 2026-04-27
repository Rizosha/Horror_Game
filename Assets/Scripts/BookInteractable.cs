using UnityEngine;

public class BookInteractable : MonoBehaviour, Interfaces.IInteractable
{
    [SerializeField] private Inventory.InventoryItem item;
    
    public void Interact()
    {
        Debug.Log("Interaction happened");
        Inventory.Instance.AddItemToInventory(item, item.quantity);
        Destroy(gameObject);
    }
}
