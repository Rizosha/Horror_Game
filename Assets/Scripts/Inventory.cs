using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private List<OnItemHover> inventoryItemHoverData = new();
    [SerializeField] private GameObject hotbar;
    [SerializeField] private CanvasGroup hotbarCanvasGroup;

    [SerializeField] private Image selectedItemImage;
    [SerializeField] private TMP_Text selectedItemName;
    [SerializeField] private TMP_Text selectedItemDescription;

    [SerializeField] private TMP_Text informationText;
    
    private InputSystem_Actions _playerInputActions;
    
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
    
    protected override void Awake()
    {
        base.Awake();
        _playerInputActions = new InputSystem_Actions();
    }
    
    private void OnEnable()
    {
        _playerInputActions.Enable();
        _playerInputActions.Player.ToggleInventory.performed += ToggleInventoryEvent;
    }
    
    private void Start()
    {
        ToggleInventory(); // Comment out if using hotbar
        playerHotbarTexts = new List<TMP_Text>(hotbar.GetComponentsInChildren<TMP_Text>().Where(text => text.CompareTag("HotbarText")));
        informationText.text = "";
        
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
                inventoryItemHoverData.Add(img.GetComponent<OnItemHover>());
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
    
    public int AddItemToInventory(InventoryItem item, int quantity)
    {
        if (CheckIfInventoryIsFull())
        {
             return HandleInventoryOverflow(quantity);
        }

        float amountToAddOverall = quantity;
        item.quantity = item.itemData.maxStack; // How many are in a stack of the item
        var stacksToAdd = amountToAddOverall / item.itemData.maxStack;
        var stacksToAddRounded = Mathf.Ceil(stacksToAdd);
        var totalStacks = stacksToAddRounded;
        
        if (item.itemData.stackable)
        {
            Debug.LogError($"Item is stackable, we have {stacksToAdd} stacks, we have an item quantity to add of {item.quantity} and a max stack of {item.itemData.maxStack}, and an overall of {amountToAddOverall}");
            {
                for (var i = 0; i < totalStacks; i++)
                {
                    if (CheckIfInventoryIsFull())
                    {
                        return HandleInventoryOverflow((int)amountToAddOverall);
                    }

                    if (stacksToAddRounded > 1)
                    {
                        stacksToAddRounded--;
                        amountToAddOverall -= item.itemData.maxStack;
                        Debug.LogError($"overall amount now: {amountToAddOverall}, deducted {item.itemData.maxStack}");
                        playerInventory.Add(item);
                    }
                    else if (stacksToAddRounded == 1)
                    {
                        item.quantity = (int)amountToAddOverall;
                        Debug.LogError($"Final stack {amountToAddOverall}");
                        playerInventory.Add(item);

                        amountToAddOverall = 0; // Ensure that the pickup item deletes itself
                    }
                }
            }
        }
        if (!item.itemData.stackable)
        {
            for (var i = 0; i < quantity; i++)
            {
                if (playerInventory.Count >= inventorySlots)
                {
                    return HandleInventoryOverflow((int)amountToAddOverall);
                } 
                
                amountToAddOverall -= item.itemData.maxStack;
                playerInventory.Add(item);
            }
        }
        
        UpdateInventoryIcons();
        return (int)amountToAddOverall;
    }

    private bool CheckIfInventoryIsFull()
    {
        return playerInventory.Count >= inventorySlots;
    }

    private int HandleInventoryOverflow(int amount)
    {
        Debug.Log("There are inventory overflow items");
        
        StopCoroutine(HandleInformationText());
        StartCoroutine(HandleInformationText());
        return amount;
    }

    private IEnumerator HandleInformationText()
    {
        informationText.text = "Your inventory is full, could not pick up all of the items";
        yield return new WaitForSeconds(2.5f);
        
        informationText.text = ""; // Clear it
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
    
    public void UpdateInventoryIcons()
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
                
                // Feed in the data of the inventory item to the inventory icon so that when hovered over it can display it on the descriptor side
                inventoryItemHoverData[i].Item = playerInventory[i];
            }
            catch
            {
                playerHotbarImages[i].color = new Color(1, 1, 1, 0);
                playerHotbarTexts[i].color = new Color(1, 1, 1, 0);
                Debug.Log("player hot bar image could not be filled - likely due to the inventory containing less items than slots");
            }
        }
    }

    private void ToggleInventoryEvent(InputAction.CallbackContext context)
    {
        ToggleInventory();
    }
    
    private void ToggleInventory()
    {
        Debug.LogError("Tab pressed");
        if (hotbarCanvasGroup.alpha == 1)
        {
            hotbarCanvasGroup.alpha = 0;

            ClearInventoryDescriptor();
        }
        else
        {
            hotbarCanvasGroup.alpha = 1;
        }
    }

    private void ClearInventoryDescriptor()
    {
        selectedItemImage.sprite = null;
        selectedItemImage.color = new Color32(0,0,0, 0);
        selectedItemName.text = "";
        selectedItemDescription.text = "";
    }
    
    public void UpdateInventoryDescriptor(Sprite descriptorImage, string descriptorName, string descriptorDescription)
    {
        selectedItemImage.color = new Color32(255, 255, 255, 255);
        selectedItemImage.sprite = descriptorImage;
        selectedItemName.text = descriptorName;
        selectedItemDescription.text = descriptorDescription;
    }
}
