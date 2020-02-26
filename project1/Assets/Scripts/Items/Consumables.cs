using System.Collections.Generic;

[System.Serializable]
public class Consumables : Items {
    public Consumable[] consumables;

    public Dictionary<string, Consumable> GetDict() {
        // Generate dictionary from array
        Dictionary<string, Consumable> dict = new Dictionary<string, Consumable>();
        foreach (Consumable item in consumables) {
            dict.Add(item.name, item);
        }

        return dict;
    }
}
