using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    [CreateAssetMenu(menuName = "InventoryItems")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        [TextArea(3, 10)]
        public string itemDescription;
        public Sprite icon;
        public bool stackable;
        public int maxStack;
    }
    
    [Serializable]
    public struct InventoryItem
    {
        public ItemData itemData;
        public int quantity;
    }
    
    public List<InventoryItem> playerInventory = new List<InventoryItem>();
    [SerializeField] private int inventorySlots = 6; // How many slots in the inventory

    
    #region Testing
    public bool addItemToInventoryTest;
    public bool removeItemFromInventoryTest;
    public InventoryItem inventoryItemToAddTest;
    public InventoryItem inventoryItemToRemoveTest;
    public int quantityToAdd = 1;
    public int quantityToRemove = 1;
    
    private void Update()
    {
        if (addItemToInventoryTest)
        {
            AddItemToInventory(inventoryItemToAddTest, quantityToAdd);
            addItemToInventoryTest = false;
        }

        if (removeItemFromInventoryTest)
        {
            RemoveItemFromInventory(inventoryItemToRemoveTest, quantityToRemove);
            removeItemFromInventoryTest = false;
        }
    }
    #endregion


    public void AddItemToInventory(InventoryItem item, int quantity)
    {
        if (playerInventory.Count >= inventorySlots)
        {
            Debug.LogWarning("Inventory is full");
            return;
            // Maybe add visible feedback through UI here
        }

        if (playerInventory.Count < inventorySlots)
        {
            if (item.itemData.stackable)
            {
                item.quantity = quantity;
                playerInventory.Add(item);
            }
            else
            {
                for (var i = 0; i < quantity; i++)
                {
                    playerInventory.Add(item);
                }      
            }
        }
    }

    public void RemoveItemFromInventory(InventoryItem item, int quantity)
    {
        if (playerInventory.Contains(item))
        {
            if (!item.itemData.stackable)
            { 
                for (var i = 0; i < quantity; i++) 
                {
                    if (item.itemData.maxStack - quantity > 0 && playerInventory.Count > 0) 
                    {
                        playerInventory.Remove(item); 
                    } 
                    else 
                    {
                        Debug.Log("Inventory removal for loop broke, likely due to the inventory no longer containing the item"); 
                        break;
                    }
                }  
                
                Debug.Log("Inventory item removed");
            }
            else
            {
                item.quantity -= quantity;
                Debug.Log("Inventory item quantity decreased");
            }
        }
    }
}
