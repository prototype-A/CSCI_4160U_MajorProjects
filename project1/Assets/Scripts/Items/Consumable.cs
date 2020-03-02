[System.Serializable]
public class Consumable : StackableItem {
    public string type;
    public int heal;

    public Consumable(StackableItem item, int amount) : base(item.name, item.desc, item.icon, amount) {}
}
