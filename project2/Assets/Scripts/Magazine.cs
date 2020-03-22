public class Magazine : Item {

    public int ammoCount;
    public int capacity;

    public override void ShowItemInfo(ItemInfoPanel infoPanel) {
        base.ShowItemInfo(infoPanel);
        infoPanel.SetInfo("Ammo: " + ammoCount);
    }
}
