public abstract class Item {

    private string name { get; }
    private string description { get; }

    public Item(string name, string desc) {
        this.name = name;
        this.description = desc;
    }
}
