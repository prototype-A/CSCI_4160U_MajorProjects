using UnityEngine;

public abstract class Item : MonoBehaviour {

    public ItemInfo itemInfo;

    public virtual void ShowItemInfo(ItemInfoPanel infoPanel) {
        infoPanel.SetName(itemInfo.name);
        infoPanel.SetDesc(itemInfo.desc);
    }

    public Transform GetPlayerTransform() {
        return transform.parent;
    }

    public FPSCharacterController GetPlayerController() {
        return GetPlayerTransform().gameObject.GetComponent<FPSCharacterController>();
    }
}
