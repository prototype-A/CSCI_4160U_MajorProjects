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
    private Menu gui;
    private PointerEventData pointerEventData;
    private Vector3 origPos;

    private bool mouseEntered = false;
    public float mouseHoverDuration = 1.0f;
    private float mouseHoverTime = 0.0f;


    void Start() {
        rectTransform = GetComponent<RectTransform>();
        gui = transform.parent.parent.parent.parent.gameObject.GetComponent<Menu>();
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
            item.ShowItemInfo(gui.GetItemInfoPanel());

            // Position and show info panel
            float oneByOneSize = GetComponent<RectTransform>().sizeDelta.x / item.itemInfo.size.x;
            float xPos = transform.position.x + (oneByOneSize * (item.itemInfo.size.x / 2 + 2));
            float yPos = transform.position.y + (oneByOneSize * (item.itemInfo.size.y / 2 + 2)) - (30 * item.itemInfo.size.y);
            gui.itemInfoPanel.transform.position = new Vector3(xPos, yPos, 0);
            gui.itemInfoPanel.SetActive(true);

            // Force refresh to properly resize panel on first hover
            Canvas.ForceUpdateCanvases();
            gui.itemInfoPanel.GetComponent<VerticalLayoutGroup>().enabled = false;
            gui.itemInfoPanel.GetComponent<VerticalLayoutGroup>().enabled = true;
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

    private bool WithinInventorySpace() {
        foreach (Transform boundary in gui.inventorySpaceBoundaries) {
            // Convert rect positions to world space for correct overlap checking
            RectTransform boundaryRT = boundary.gameObject.GetComponent<RectTransform>();
            Rect boundaryRect = boundaryRT.rect;
            boundaryRect.center = boundaryRT.TransformPoint(boundaryRect.center);
            boundaryRect.size = boundaryRT.TransformVector(boundaryRect.size);
            RectTransform itemRT = gameObject.GetComponent<RectTransform>();
            Rect itemRect = itemRT.rect;
            itemRect.center = itemRT.TransformPoint(itemRect.center);
            itemRect.size = itemRT.TransformVector(itemRect.size);

            if (itemRect.Overlaps(boundaryRect)) {
                return false;
            }
        }

        return true;
    }

    private bool NotOverlappingItems() {
        foreach (Transform otherItem in gui.inventoryItems) {
            if (otherItem.gameObject != gameObject) {
                // Convert rect positions to world space for correct overlap checking
                RectTransform otherItemRT = otherItem.gameObject.GetComponent<RectTransform>();
                Rect otherItemRect = otherItemRT.rect;
                otherItemRect.center = otherItemRT.TransformPoint(otherItemRect.center);
                otherItemRect.size = otherItemRT.TransformVector(otherItemRect.size);
                RectTransform itemRT = gameObject.GetComponent<RectTransform>();
                Rect itemRect = itemRT.rect;
                itemRect.center = itemRT.TransformPoint(itemRect.center);
                itemRect.size = itemRT.TransformVector(itemRect.size);

                if (itemRect.Overlaps(otherItemRect)) {
                    Debug.Log("Overlapping with " + otherItem.gameObject.name);
                    return false;
                }
            }
        }

        return true;
    }

    public void OnPointerClick(PointerEventData eventData) {
        pointerEventData = eventData;
        if (eventData.clickCount == 2) {
            // Double click
            Debug.Log("Double-clicked item");
        } else {
            if (item.itemInfo.itemType == Types.ItemType.Gun) {
                // Show gun attachments in customization pane
                gui.gunCustomization.CustomizeGun((Gun)item);
            }
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
        if (gui.itemInfoPanel.activeSelf) {
            gui.itemInfoPanel.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("Rotated");
        }
    }

    private void DragItem(bool draggingItem, PointerEventData eventData) {
        pointerEventData = eventData;
        if (draggingItem) {
            // Dragging item
            transform.position = Input.mousePosition;
        } else {
            // Stopped dragging item
            List<RaycastResult> results = new List<RaycastResult>();
            gui.GetGraphicRaycaster().Raycast(eventData, results);

            if (results.Count == 1) {
                Debug.Log("Dropping Item");

                // Drop item on ground

                gui.gunCustomization.ClearGunCustomization();
                Destroy(gameObject);
            } else {
                // Move item in inventory
                bool itemMoved = false;
                foreach (RaycastResult result in results) {
                    // Check if dragging item to available inventory space
                    if (result.gameObject.name == "InventorySpace" && WithinInventorySpace() && NotOverlappingItems()) {
                        itemMoved = true;
                        break;
                    }
                    // Equip weapon
                    ItemSlot slot = result.gameObject.GetComponent<ItemSlot>();
                    if (slot != null && slot.slotType == Types.ItemType.Gun) {
                        itemMoved = true;
                    }
                }

                if (!itemMoved) {
                    // Return item to original position if invalid destination
                    transform.position = origPos;
                }

                /*
                bool itemMoved = false;
                foreach (RaycastResult result in results) {
                    // Check if valid slot
                    ItemSlot slot = result.gameObject.GetComponent<ItemSlot>();
                    if (slot != null && (slot.slotType == item.itemInfo.itemType || slot.slotType == Types.ItemType.All)) {
                        // Check if any other items are in slot/nearby slots
                        bool noOtherItems = true;


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
                    // Return item to original position if invalid destination
                    transform.position = origPos;
                }
                */
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        pointerEventData = eventData;
        // Release mouse drag
        DragItem(false, eventData);
    }
}
