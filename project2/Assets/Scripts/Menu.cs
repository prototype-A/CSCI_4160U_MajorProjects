using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public GameObject crosshair;
    public GameObject killConfirm;
    public GameObject gunGui;
    public GameObject menu;
    public GameObject itemInfoPanel;
    public GunCustomization gunCustomization;
    public GameObject playerStatus;
    public GameObject inventoryItemPrefab;
    public Transform inventorySpaceBoundaries;
    public Transform inventoryItems;

    public ItemInfoPanel GetItemInfoPanel() {
        return itemInfoPanel.GetComponent<ItemInfoPanel>();
    }

    public GraphicRaycaster GetGraphicRaycaster() {
        return GetComponent<GraphicRaycaster>();
    }
}
