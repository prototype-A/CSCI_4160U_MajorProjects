public abstract class StackableItem : Item {

    private int maxNumPerStack { get; }
    private int numHeld { get; }

    public StackableItem(string name, string desc, string icon) : base(name, desc, icon) {
        this.numHeld = 0;
    }

    public StackableItem(string name, string desc, string icon, int numHeld) : this(name, desc, icon) {
        this.numHeld = numHeld;
        this.maxNumPerStack = 99;
    }
}
