using System.Collections.Generic;

﻿[System.Serializable]
public class ItemDrop {
    public string name;
    public int maxAmount;
    public int chance;

    public KeyValuePair<string, int> GetDrop() {
        System.Random rngesus = new System.Random();
        KeyValuePair<string, int> drop = new KeyValuePair<string, int>(name, 0);

        for (int i = 0; i < maxAmount; i++) {
            if (rngesus.Next(this.chance + 1) <= this.chance) {
                // One of the item dropped
                drop = new KeyValuePair<string, int>(name, drop.Value + 1);
            }
        }

        return drop;
    }
}
