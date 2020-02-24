public abstract class StackableItem : Item {

    private int maxNumPerStack { get; }
    private int numHeld { get; }

    public StackableItem(string name, string desc) : base(name, desc) {
        this.numHeld = 0;
    }

    public StackableItem(string name, string desc, int numHeld) : this(name, desc) {
        this.numHeld = numHeld;
        this.maxNumPerStack = 99;
    }
}
