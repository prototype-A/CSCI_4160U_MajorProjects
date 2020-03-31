using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public Transform player;
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

    public void ShowGunGui(bool show) {
        crosshair.SetActive(show);
        gunGui.SetActive(show);
    }

    public void ToggleCrosshair(bool show) {
        crosshair.SetActive(show);
    }

    public void ChangeCrosshair(Sprite crosshairSprite) {
        crosshair.GetComponent<Image>().sprite = crosshairSprite;
    }
}
