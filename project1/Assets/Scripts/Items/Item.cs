[System.Serializable]
public class Item {

    public string name;
    public string desc;
    public string iconName;

    public Item(string name, string desc) {
        this.name = name;
        this.desc = desc;
    }
}
