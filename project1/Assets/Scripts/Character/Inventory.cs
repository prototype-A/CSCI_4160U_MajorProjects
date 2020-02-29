using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
﻿public class Inventory {

    private readonly int INVENTORY_SIZE;
    private Item[] items;
    private Transform inventoryPanel;

    public Inventory(int inventorySize, Transform inventoryPanel) {
        this.INVENTORY_SIZE = inventorySize;
        this.items = new Item[this.INVENTORY_SIZE];
        for (int i = 0; i < this.INVENTORY_SIZE; i++) {
            // Initialize empty inventory
            this.items[i] = null;
        }
        this.inventoryPanel = inventoryPanel;
    }

    public bool AddItem(string itemName, Item itemToAdd, int amountToAdd = -1) {
        try {
            // Stackable item
            StackableItem item = (StackableItem)itemToAdd;

            // Find first available stack of the item in player's inventory and add it to that stack
            int i = 0;
            for (; i < this.INVENTORY_SIZE; i++) {
                if (items[i].name == itemToAdd.name) {
                    if (((StackableItem)items[i]).numHeld + amountToAdd <= 99) {
                        // Can add all to this stack
                        ((StackableItem)items[i]).numHeld += amountToAdd;
                        amountToAdd = 0;
                    } else {
                        // Split amountToAdd between this and next stack
                        int numToAdd = 99 - ((StackableItem)items[i]).numHeld;
                        ((StackableItem)items[i]).numHeld += numToAdd;
                        amountToAdd -= numToAdd;
                    }
                }
            }

            // Item stacked
            if (amountToAdd == 0) {
                // Update icon of item at the slot in the UI
                UpdateInventoryUI(i);

                return true;
            } else {
                // Throw exception to continue to code in catch clause
                throw new Exception();
            }
        } catch {
            // Non-stackable item/create new stack
            for (int i = 0; i < this.INVENTORY_SIZE; i++) {
                if (items[i] == null) {
                    // Add item
                    items[i] = itemToAdd;

                    return true;
                }
            }
        }

        // Failed to add item
        return false;
    }

    public void MoveItem(int index1, int index2) {
        // Move item from index 1 to index 2
        // If there is an item at index2, move it to index1 (swap)
        Item otherItem = this.items[index2];
        this.items[index2] = this.items[index1];
        this.items[index1] = otherItem;
        UpdateInventoryUI(index1, index2);
    }

    public void DropItem(int index, int amountToDrop = -1) {
        try {
            // Dropping stackable item
            StackableItem item = (StackableItem)items[index];

            if (amountToDrop <= item.numHeld) {
                // Drop partial amount of item stack
                item.numHeld -= amountToDrop;

                UpdateInventoryUI(index, false, true);
            } else {
                throw new Exception();
            }
        } catch {
            // Dropping non-stackable item/full stack of stackable item
            items[index] = null;
            UpdateInventoryUI(index, true);
        }
    }

    private void UpdateInventoryUI(int index, bool removeAll = false, bool removingAmount = false) {
        if (removingAmount) {
            // Update item's amount held indicator
            inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + index).Find("Image").Find("Amount").gameObject.GetComponent<TextMeshProUGUI>().text = "" + ((StackableItem)items[index]).numHeld;
        }

        if (!removeAll) {
            // Add icon
            float ppu = 100.0f;
            Texture2D itemImg = Resources.Load("Sprites/Items/" + items[index].icon) as Texture2D;
            Sprite itemIcon = Sprite.Create(itemImg, new Rect(0, 0, itemImg.width, itemImg.height), new Vector2(0, 0), ppu);
            inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + index).Find("Image").gameObject.GetComponent<Image>().sprite = itemIcon;
        } else {
            // Remove icon and number held indicator
            inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + index).Find("Image").gameObject.GetComponent<Image>().sprite = null;
            inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + index).Find("Image").Find("Amount").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    private void UpdateInventoryUI(int index1, int index2) {
        // Swap the sprites of the two items at the indexes
        GameObject item1 = inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + index1).gameObject;
        GameObject item2 = inventoryPanel.Find("ItemPanel").Find("Items").Find("Item" + index2).gameObject;
        Sprite item2Icon = item2.GetComponent<Image>().sprite;
        item2.GetComponent<Image>().sprite = item1.GetComponent<Image>().sprite;
        item1.GetComponent<Image>().sprite = item2Icon;
    }
}
