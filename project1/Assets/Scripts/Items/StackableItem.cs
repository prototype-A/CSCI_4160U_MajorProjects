public class StackableItem : Item {

    public int numHeld;
    public readonly int maxNumPerStack;

    public StackableItem() {
        this.numHeld = 0;
        this.maxNumPerStack = 99;
    }

    public StackableItem(string name, string desc, string icon) : base(name, desc, icon) {
        this.numHeld = 0;
        this.maxNumPerStack = 99;
    }

    public StackableItem(string name, string desc, string icon, int numHeld) : base(name, desc, icon) {
        this.numHeld = numHeld;
        this.maxNumPerStack = 99;
    }
}
