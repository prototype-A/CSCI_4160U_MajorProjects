using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class Items {
    public Dictionary<string, Item> GetDict(Item[] items) {
        // Generate dictionary from array
        Dictionary<string, Item> dict = new Dictionary<string, Item>();
        foreach (Item item in items) {
            dict.Add(item.name, item);
        }

        return dict;
    }
}
