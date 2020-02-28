using System.Collections.Generic;

[System.Serializable]
public class Weapons : Items {
    public Item[] weapons;
    public Prefix[] prefixes;

    public Dictionary<string, Item> GetDict() {
        return base.GetDict(this.weapons);
    }
}
