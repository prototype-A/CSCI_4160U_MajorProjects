using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

﻿public class InventoryManager : MonoBehaviour {

    public Transform inventoryPanel;

    void Start() {
        GameData.inventoryManager = this;
        //this.inventoryPanel = transform.Find("Inventory");
    }

    public bool AddItem(Item itemToAdd, int amountToAdd) {
        int i = 0;
        try {
            // Stackable item
            StackableItem item = (StackableItem)itemToAdd;

            // Find first available stack of the item in player's inventory and add it to that stack
            for (; i < GameData.playerData.INVENTORY_SIZE; i++) {
                if (GameData.playerData.items[i].name == itemToAdd.name) {
                    if (((StackableItem)GameData.playerData.items[i]).numHeld + amountToAdd <= 99) {
                        // Can add all to this stack
                        ((StackableItem)GameData.playerData.items[i]).numHeld += amountToAdd;
                        amountToAdd = 0;
                        break;
                    } else {
                        // Split amountToAdd between this and next stack
                        int numToAdd = 99 - ((StackableItem)GameData.playerData.items[i]).numHeld;
                        ((StackableItem)GameData.playerData.items[i]).numHeld += numToAdd;
                        amountToAdd -= numToAdd;
                    }
                }
            }

            // Item stacked
            if (amountToAdd == 0) {
                return true;
            } else {
                // Throw exception to continue to code in catch clause
                throw new Exception();
            }
        } catch {
            // Non-stackable item/create new stack
            for (int it = 0; it < GameData.playerData.INVENTORY_SIZE; it++) {
                if (GameData.playerData.items[it] == null) {
                    // Add item
                    GameData.playerData.items[it] = itemToAdd;
                    return true;
                }
            }
        } finally {
            UpdateInventoryUI(i);
        }

        // Failed to add item
        return false;
    }

    public void MoveItem(int index1, int index2) {
        if (index1 != index2) {
            // Move item from index 1 to index 2
            // If there is an item at index2, move it to index1 (swap)
            Item otherItem = GameData.playerData.items[index2];
            GameData.playerData.items[index2] = GameData.playerData.items[index1];
            GameData.playerData.items[index1] = otherItem;
            UpdateInventoryUI(index1, index2);
        }
    }

    public void DropItem(int index, int amountToDrop) {
        try {
            // Dropping stackable item
            StackableItem item = (StackableItem)GameData.playerData.items[index];

            if (amountToDrop <= item.numHeld) {
                // Drop partial amount of item stack
                item.numHeld -= amountToDrop;

                UpdateInventoryUI(index, true);
            } else {
                throw new Exception();
            }
        } catch {
            // Dropping non-stackable item/full stack of stackable item
            GameData.playerData.items[index] = null;
            UpdateInventoryUI(index, true);
        }
    }

    public void LoadInventory(Item[] inventory) {
        for (int i = 0; i < inventory.Length; i++) {
            if (inventory[i] != null) {
                UpdateInventoryUI(i);
            }
        }
    }

    private void UpdateInventoryUI(int index, bool removeAllInStack = false) {
        if (!removeAllInStack) {
            Texture2D itemImg = Resources.Load("Sprites/Items/" + GameData.playerData.items[index].icon) as Texture2D;
            Sprite itemIcon = Sprite.Create(itemImg, new Rect(0, 0, itemImg.width, itemImg.height), new Vector2(0, 0), 34);
            ShowItem(index, itemIcon);
        } else {
            HideItem(index);
        }
    }

    private void UpdateInventoryUI(int index1, int index2) {

        // Swap the sprites of the two items at the indexes
        GameObject item1 = inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + (index1 + 1)).Find("Image").gameObject;
        GameObject item2 = inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + (index2 + 1)).Find("Image").gameObject;
        Sprite item1Icon = item1.GetComponent<Image>().sprite;
        Sprite item2Icon = item2.GetComponent<Image>().sprite;

        if (GameData.playerData.items[index1] == null) {
            // Move item icon to new slot
            ShowItem(index2, item1Icon);
            HideItem(index1);
        } else {
            // Swapping item slots
            ShowItem(index2, item1Icon);
            ShowItem(index1, item2Icon);
        }
    }

    private void ShowItem(int index, Sprite sprite) {
        // Add icon
        Image itemImage = inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + (index + 1)).Find("Image").gameObject.GetComponent<Image>();
        itemImage.color = new Color(255, 255, 255, 255);
        itemImage.sprite = sprite;

        // Update item's amount held indicator if it is stackable
        try {
            TextMeshProUGUI amountText = inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + (index + 1)).Find("Image").Find("Amount").gameObject.GetComponent<TextMeshProUGUI>();
            amountText.text = "" + ((StackableItem)GameData.playerData.items[index]).numHeld;
        } catch {}
    }

    private void HideItem(int index) {
        index++;
        // Remove icon
        Image itemImage = inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + index).Find("Image").gameObject.GetComponent<Image>();
        itemImage.color = new Color(255, 255, 255, 0);
        itemImage.sprite = null;
        // Remove amount of the item held indicator
        TextMeshProUGUI amountText = inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + index).Find("Image").Find("Amount").gameObject.GetComponent<TextMeshProUGUI>();
        amountText.text = "";
    }
}
