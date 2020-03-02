using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryItemMouseEventHandler : MonoBehaviour,
                                            IBeginDragHandler,
                                            IDragHandler,
                                            IEndDragHandler,
                                            IPointerClickHandler,
                                            IPointerEnterHandler,
                                            IPointerExitHandler {

    public Transform infoPanel;
    public Transform draggedItem;
    public int itemNum;

    private bool mouseEntered = false;
    private static float mouseHoverDurationThreshold = 1.0f;
    private float mouseHoverTime = 0.0f;

    GraphicRaycaster raycaster;


    void Start() {
        this.raycaster = transform.parent.parent.parent.parent.gameObject.GetComponent<GraphicRaycaster>();
    }

    void Update() {
        if (mouseEntered && HasItemInSlot()) {
            StartCoroutine(ShowItemInfo());
        }
    }

    private IEnumerator ShowItemInfo() {
        // Coroutine to show item info panel after some time of hovering
        if ((Time.time - mouseHoverTime) >= mouseHoverDurationThreshold) {
            // Change text to item's text
            Item item = GameData.playerData.items[itemNum];
            infoPanel.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = item.name;
            infoPanel.Find("ItemDesc").gameObject.GetComponent<TextMeshProUGUI>().text = item.desc;

            // Position and show info panel
            float xPos = -20 + ((itemNum % 5) * 65);
            float yPos = 35 - ((itemNum / 5) * 65) - (-61 - transform.parent.parent.parent.Find("Items").gameObject.GetComponent<RectTransform>().anchoredPosition3D.y);
            infoPanel.gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(xPos, yPos, 0);
            infoPanel.gameObject.SetActive(true);

            // Force refresh to properly resize panel on first hover
            Canvas.ForceUpdateCanvases();
            infoPanel.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
            infoPanel.gameObject.GetComponent<VerticalLayoutGroup>().enabled = true;
        }

        yield return new WaitForSeconds(0.5f);
    }

    private bool HasItemInSlot() {
        return (GameData.playerData.items[itemNum] != null);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.clickCount == 2) {
            // Double click
            if (GameData.playerData.items[itemNum] != null) {
                Debug.Log("Double-clicked item: " + GameData.playerData.items[itemNum].name);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // Mouse hovers over item in inventory
        if (HasItemInSlot()) {
            mouseEntered = true;
            mouseHoverTime = Time.time;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        mouseEntered = false;
        if (infoPanel.gameObject.activeSelf) {
            infoPanel.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        // Start to move item to make it appear to start being dragged earlier
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData) {
        // Drag inventory item
        DragItem(true, eventData);
    }

    private void DragItem(bool draggingItem, PointerEventData eventData) {
        if (HasItemInSlot()) {
            Image origUIItem = GetComponent<Image>();
            TextMeshProUGUI amount = transform.Find("Amount").GetComponent<TextMeshProUGUI>();

            if (draggingItem) {
                // Dragging item
                origUIItem.enabled = false;
                amount.enabled = false;

                draggedItem.Find("Image").gameObject.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
                draggedItem.Find("Image").Find("Amount").gameObject.GetComponent<TextMeshProUGUI>().text = amount.text;
                draggedItem.position = Input.mousePosition;
                draggedItem.gameObject.SetActive(true);
            } else {
                // Stopped dragging item
                draggedItem.gameObject.SetActive(false);
                amount.enabled = true;
                origUIItem.enabled = true;

                // Move item to new slot if dragged to another slot
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(eventData, results);
                foreach (RaycastResult result in results) {
                    if (result.gameObject.name == "Image") {
                        InventoryItemMouseEventHandler ui = result.gameObject.GetComponent<InventoryItemMouseEventHandler>();
                        GameData.inventoryManager.MoveItem(itemNum, ui.itemNum);
                    }
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        // Release mouse drag
        DragItem(false, eventData);
    }
}
