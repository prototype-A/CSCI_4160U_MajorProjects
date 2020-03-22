[System.Serializable]
public class ItemSave {

    private ItemInfo itemInfo;
    private int ammoCount;

    public ItemSave(Magazine mag) {
        this.itemInfo = mag.itemInfo;
        this.ammoCount = mag.ammoCount;
    }

    public ItemSave (Gun gun) {
        this.itemInfo = gun.itemInfo;

    }
}
