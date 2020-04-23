using UnityEngine;

public abstract class Item : MonoBehaviour {

    public ItemSpawner spawnPoint;
    public ItemInfo itemInfo;

    // Return true if item disappears after use (consumed)
    public abstract bool Use();

    public virtual void ShowItemInfo(ItemInfoPanel infoPanel) {
        infoPanel.SetName(itemInfo.name);
        infoPanel.SetDesc(itemInfo.desc);
    }

    public void PickUp() {
        spawnPoint.spawnedItem = null;
    }

    public Transform GetPlayerTransform() {
        return transform.parent;
    }

    public FPSCharacterController GetPlayerController() {
        return GetPlayerTransform().gameObject.GetComponent<FPSCharacterController>();
    }
}
