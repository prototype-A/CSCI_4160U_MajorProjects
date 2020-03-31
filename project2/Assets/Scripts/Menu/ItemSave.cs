[System.Serializable]
public class ItemSave {

    private ItemInfo itemInfo;
    private Magazine mag;

    public ItemSave(Item item) {
        this.itemInfo = item.itemInfo;

        switch(this.itemInfo.itemType) {
            case GameSystem.ItemType.Gun:
                // Need to store attachments
                this.itemInfo = item.itemInfo;
                
                break;
            case GameSystem.ItemType.Magazine:
                // Need to store ammo count
                this.mag = (Magazine)item;
                break;
        }

    }
}
