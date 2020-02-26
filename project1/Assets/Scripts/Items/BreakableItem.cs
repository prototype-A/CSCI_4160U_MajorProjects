public abstract class BreakableItem : Item {

    private int durability { get; }
    private int maxDurability { get; }

    public BreakableItem(string name, string desc, string icon, int durability, int maxDurability) : base(name, desc, icon) {
        this.durability = durability;
        this.maxDurability = maxDurability;
    }
}
