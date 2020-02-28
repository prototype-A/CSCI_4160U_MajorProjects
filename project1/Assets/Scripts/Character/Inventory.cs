using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

[System.Serializable]
﻿public class Inventory {

    private readonly int INVENTORY_SIZE = 30;
    private ObservableCollection<KeyValuePair<string, Item>> items;

    public Inventory() {
        this.items = new ObservableCollection<KeyValuePair<string, Item>>();
    }

    public bool AddItem(string itemName, Item itemToAdd, int amount) {
        try {
            // Check if item is stackable
            StackableItem item = (StackableItem)itemToAdd;

            // Check if (a stack of the) item exists in the inventory
            item = null;
            IEnumerator<KeyValuePair<string, Item>> sameItems = items.Where(i => i.Key == itemName).GetEnumerator();
            while (sameItems.MoveNext()) {
                // Check all stacks of the item
                item = (StackableItem)sameItems.Current.Value;

                if (item.numHeld + amount <= item.maxNumPerStack) {
                    // Add to item stack
                    item.numHeld += amount;

                    return true;
                } else {
                    // Split the amount to each stack of the item, if they are too full to add all at once
                    int amountToAdd = item.maxNumPerStack - item.numHeld;

                    // Add to item stack
                    item.numHeld += amountToAdd;

                    amount -= amountToAdd;
                }
            }

            // Add item to player's inventory only if it's not full
            throw new Exception();
        } catch {
            // Item is not stackable
            if (items.Count < INVENTORY_SIZE) {
                // Add item if inventory is not full
                items.Add(new KeyValuePair<string, Item>(itemName, itemToAdd));

                return true;
            } else {
                // Inventory full
                return false;
            }
        }
    }

    public void MoveItem(int origIndex, int endIndex) {
        /*
        // Change the position (index) of the item in the inventory or swaps the positions of two items in the inventory
        Item otherItem = null;
        if (items[endIndex].Equals()) {
            // Another item exists at the destination
            otherItem = items[endIndex];
        }
        // Move the item
        items.SetItem(endIndex, items[origIndex]);
        if (otherItem != null) {
            // Move the other item to where the item that moved originally was
            items.SetItem(origIndex, otherItem);
        }
        */
    }

    public void DropItem(string name, int count) {

    }
}
