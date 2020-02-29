[System.Serializable]
public class InventoryItem {
    private Item item;
    private int slotNum;

    public InventoryItem(Item item, int slotNum) {
        this.item = item;
        this.slotNum = slotNum;
    }
}
