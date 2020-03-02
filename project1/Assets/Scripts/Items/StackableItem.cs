[System.Serializable]
﻿public class StackableItem : Item {

    public int numHeld;
    public readonly int maxNumPerStack;

    public StackableItem() {}

    public StackableItem(string name, string desc, string icon) : base(name, desc, icon) {
        this.numHeld = 1;
        this.maxNumPerStack = 99;
    }
    
    public StackableItem(string name, string desc, string icon, int numHeld) : base(name, desc, icon) {
        this.numHeld = numHeld;
        this.maxNumPerStack = 99;
    }
}
