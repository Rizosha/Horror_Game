using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private List<Image> playerHotbarImages = new();
    [SerializeField] private List<TMP_Text> playerHotbarTexts = new();
    [SerializeField] private GameObject hotbar;
    
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
    
    private void Start()
    {
        playerHotbarTexts = new List<TMP_Text>(hotbar.GetComponentsInChildren<TMP_Text>());
        
        // Find all hotbar images via tag and then add them to an array after sorting them in order via hierarchy from the parent
        var hotbarImages = GameObject.FindGameObjectsWithTag("HotbarImage")
            .OrderBy(obj => obj.transform.parent.GetSiblingIndex())
            .ToArray();
        
        foreach (var image in hotbarImages)
        {
            playerHotbarImages.Add(image.GetComponent<Image>());
        }

        if (playerHotbarImages != null)
        {
            foreach (var img in playerHotbarImages)
            {
                img.color = new Color(1, 1, 1, 0);
            }
        }

        if (playerHotbarTexts != null)
        {
            foreach (var text in playerHotbarTexts)
            {
                text.color = new Color(1, 1, 1, 0);
            }
        }
        else
        {
            Debug.LogWarning("Player hot bar text list is null");
        }
    }
    
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

                if (item.quantity >= item.itemData.maxStack)
                {
                    float amountToAdd = item.quantity;
                    
                    // Add one stack
                    item.quantity = item.itemData.maxStack;
                    playerInventory.Add(item);

                    amountToAdd -= item.itemData.maxStack;
                    Debug.LogError(amountToAdd);
                    var stacksToAdd = amountToAdd /  item.itemData.maxStack;
                    var stacksToAddRoundedUp = Mathf.Ceil(stacksToAdd);
                    Debug.LogError(stacksToAddRoundedUp);

                    for (var i = 0; i < stacksToAddRoundedUp; i++)
                    {
                        if (stacksToAddRoundedUp > 1)
                        {
                            stacksToAddRoundedUp--;
                            amountToAdd -= item.itemData.maxStack;
                            playerInventory.Add(item);
                        }
                        if (stacksToAddRoundedUp == 1) // Sort out the final not full stack
                        {
                            item.quantity = (int)amountToAdd;
                            Debug.LogError($"Final stack {amountToAdd}");
                            playerInventory.Add(item);
                        }
                    }
                }
            }
            
            else
            {
                for (var i = 0; i < quantity; i++)
                {
                    if (playerInventory.Count >= inventorySlots)
                    {
                        Debug.Log("Possible overflow items not added to inventory");
                        // TODO: Sort this out
                        // - Item is left on ground with pickup quantity reduced?
                        break;
                    }
                    
                    playerInventory.Add(item);
                }      
            }
        }
        
        UpdateInventoryIcons();
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
        
        UpdateInventoryIcons();
    }
    
    private void UpdateInventoryIcons()
    {
        var sprites = new List<Sprite>();
        sprites.Clear();
        
        foreach (var inventoryItem in playerInventory)
        {
            var itemIcon = inventoryItem.itemData.icon;
            sprites.Add(itemIcon);
        }

        for (var i = 0; i < inventorySlots; i++)
        {
            try
            {
                playerHotbarImages[i].sprite = sprites[i];
                playerHotbarImages[i].color = new Color(1, 1, 1, 1);
                
                playerHotbarTexts[i].text = playerInventory[i].quantity.ToString();
                playerHotbarTexts[i].color = new Color(1, 1, 1, 1);
            }
            catch
            {
                playerHotbarImages[i].color = new Color(1, 1, 1, 0);
                playerHotbarTexts[i].color = new Color(1, 1, 1, 0);
                Debug.Log("player hot bar image could not be filled - likely due to the inventory containing less items than slots");
            }
        }
    }
}
