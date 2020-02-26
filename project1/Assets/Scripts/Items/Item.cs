[System.Serializable]
public class Item {

    public string name;
    public string desc;
    public string icon;

    public Item() {}

    public Item(string name, string desc, string icon) {
        this.name = name;
        this.desc = desc;
        this.icon = icon;
    }

    public Item(Item baseItem) : this(baseItem.name, baseItem.desc, baseItem.icon) {}
}
