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
    public Menu menu;
    private Transform itemInfoPanel;
    public GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private Vector3 origPos;

    private bool mouseEntered = false;
    public float mouseHoverDuration = 1.0f;
    private float mouseHoverTime = 0.0f;


    void Start() {
        rectTransform = GetComponent<RectTransform>();
        itemInfoPanel = menu.itemInfoPanel;
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
            // Show item info
            item.ShowItemInfo(itemInfoPanel.GetComponent<ItemInfoPanel>());

            // Position and show info panel
            float oneByOneSize = GetComponent<RectTransform>().sizeDelta.x / item.itemInfo.size.x;
            float xPos = transform.position.x + (oneByOneSize * (item.itemInfo.size.x / 2 + 2));
            float yPos = transform.position.y + (oneByOneSize * (item.itemInfo.size.y / 2 + 2)) - (30 * item.itemInfo.size.y);
            itemInfoPanel.position = new Vector3(xPos, yPos, 0);
            itemInfoPanel.gameObject.SetActive(true);

            // Force refresh to properly resize panel on first hover
            Canvas.ForceUpdateCanvases();
            itemInfoPanel.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
            itemInfoPanel.gameObject.GetComponent<VerticalLayoutGroup>().enabled = true;
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void SetItem(Item item) {
        this.item = item;
        SetSelfSize();
    }

    private void SetSelfSize() {
        // Change size if not 1x1-sized item
        if (item.itemInfo.size.x != 1 && item.itemInfo.size.x != item.itemInfo.size.y) {
            Vector2 oneByOneSize = GetComponent<RectTransform>().sizeDelta;
            GetComponent<RectTransform>().sizeDelta = new Vector2(item.itemInfo.size.x * oneByOneSize.x, item.itemInfo.size.y * oneByOneSize.y);
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
        if (menu.itemInfoPanel.gameObject.activeSelf) {
            menu.itemInfoPanel.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        pointerEventData = eventData;
        origPos = transform.position;
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
            // Stopped dragging item
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);

            if (results.Count == 0) {
                // Remove item from inventory


                Destroy(gameObject);
            } else {
                bool itemMoved = false;
                foreach (RaycastResult result in results) {
                    // Check if valid slot
                    ItemSlot slot = result.gameObject.GetComponent<ItemSlot>();
                    if (slot != null && (slot.slotType == item.itemInfo.itemType || slot.slotType == Types.ItemType.All)) {
                        // Check if any other items are in slot/nearby slots
                        bool noOtherItems = true;
                        foreach (RaycastResult otherResult in results) {
                            if (otherResult.gameObject.GetComponent<Item>() != null) {
                                noOtherItems = false;
                                break;
                            }
                        }
                        // Drop item in new slot
                        Vector3 resultSlot = result.gameObject.transform.position;
                        if (noOtherItems && item.itemInfo.size == new Vector2(1, 1)) {
                            transform.position = result.gameObject.transform.position;
                            itemMoved = true;
                        } else if (noOtherItems) {
                            transform.position = result.gameObject.transform.position;
                            itemMoved = true;
                        }
                    }
                }
                if (!itemMoved) {
                    // Return item to original position
                    transform.position = origPos;
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        pointerEventData = eventData;
        // Release mouse drag
        DragItem(false, eventData);
    }
}
