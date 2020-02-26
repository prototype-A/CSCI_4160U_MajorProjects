[System.Serializable]
public class ItemDrop {
    public string name;
    public int chance;

    public string GetDrop() {
        System.Random rngesus = new System.Random();
        if (rngesus.Next(this.chance + 1) <= this.chance) {
            // Item dropped
            return name;
        }

        // Item did not drop...
        return null;
    }
}
