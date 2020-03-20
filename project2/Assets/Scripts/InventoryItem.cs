using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour,
                            IBeginDragHandler,
                            IDragHandler,
                            IEndDragHandler,
                            IPointerClickHandler,
                            IPointerEnterHandler,
                            IPointerExitHandler {

    public Item item;
    private RectTransform rectTransform;
    public Transform infoPanel;
    public GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;

    private bool mouseEntered = false;
    public float mouseHoverDuration = 1.0f;
    private float mouseHoverTime = 0.0f;


    void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update() {
        if (mouseEntered) {
            StartCoroutine(ShowItemInfo());
        }
    }

    private IEnumerator ShowItemInfo() {
        // Coroutine to show item info panel after some time of hovering
        if ((Time.time - mouseHoverTime) >= mouseHoverDuration &&
            (pointerEventData != null || !pointerEventData.dragging)) {
            // Change text to item's text
            infoPanel.GetComponent<ItemInfoPanel>().SetName(item.name);
            infoPanel.GetComponent<ItemInfoPanel>().SetDesc(item.desc);

            // Position and show info panel
            float oneByOneSize = GetComponent<RectTransform>().sizeDelta.x / item.size.x;
            float xPos = transform.position.x + (oneByOneSize * item.size.x) - 5;
            float yPos = transform.position.y  + (oneByOneSize * item.size.y) - 30;
            infoPanel.position = new Vector3(xPos, yPos, 0);
            infoPanel.gameObject.SetActive(true);

            // Force refresh to properly resize panel on first hover
            Canvas.ForceUpdateCanvases();
            infoPanel.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
            infoPanel.gameObject.GetComponent<VerticalLayoutGroup>().enabled = true;
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void SetItem(Item item) {
        this.item = item;
        SetSelfSize();
    }

    private void SetSelfSize() {
        // Change size if not 1x1-sized item
        if (item.size.x != 1 && item.size.x != item.size.y) {
            Vector2 oneByOneSize = GetComponent<RectTransform>().sizeDelta;
            GetComponent<RectTransform>().sizeDelta = new Vector2(item.size.x * oneByOneSize.x, item.size.y * oneByOneSize.y);
        }
    }

    private void SetPositionInInventory() {

    }

    public void OnPointerClick(PointerEventData eventData) {
        pointerEventData = eventData;
        if (eventData.clickCount == 2) {
            // Double click
            Debug.Log("Double-clicked item");
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        pointerEventData = eventData;
        // Mouse hovers over item
        mouseEntered = true;
        mouseHoverTime = Time.time;
    }

    public void OnPointerExit(PointerEventData eventData) {
        pointerEventData = eventData;
        // Hide item info if it is showing
        mouseEntered = false;
        if (infoPanel.gameObject.activeSelf) {
            infoPanel.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        pointerEventData = eventData;
        // Start to move item to make it appear to start being dragged earlier
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData) {
        pointerEventData = eventData;
        // Drag inventory item
        DragItem(true, eventData);
    }

    private void DragItem(bool draggingItem, PointerEventData eventData) {
        pointerEventData = eventData;
        if (draggingItem) {
            // Dragging item
            transform.position = Input.mousePosition;
        } else {
            /*
            // Stopped dragging item
            // Move item to new position if dragged to another slot
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);
            foreach (RaycastResult result in results) {
                if (true) {
                    //InventoryItemMouseEventHandler ui = result.gameObject.GetComponent<InventoryItemMouseEventHandler>();
                    //GameData.inventoryManager.MoveItem(itemNum, ui.itemNum);
                }
            }
            */
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        pointerEventData = eventData;
        // Release mouse drag
        DragItem(false, eventData);
    }
}
