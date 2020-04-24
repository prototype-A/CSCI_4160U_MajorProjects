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
    public Image itemImage;
    public ItemSlot equippedSlot;
    private RectTransform rectTransform;
    private Menu gui;
    private PointerEventData pointerEventData;
    private Vector3 origPos;

    private bool mouseEntered = false;
    private float mouseHoverDuration = 1.0f;
    private float mouseHoverTime = 0.0f;


    void Start() {
        rectTransform = GetComponent<RectTransform>();
        gui = item.GetPlayerController().gui;
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
        itemImage.sprite = item.itemInfo.srcImg;
        itemImage.color = new Color(255, 255, 255, 255);
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
        // Check if item is within the bounds of the inventory
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
        // Check if item is overlapping with another inventory item
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

    private InventoryItem GetEquippedItem() {
        // Get the item in slot with overlap check
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
                    return otherItem.gameObject.GetComponent<InventoryItem>();
                }
            }
        }

        return null;
    }

    public void OnPointerClick(PointerEventData eventData) {
        pointerEventData = eventData;
        if (eventData.clickCount == 2) {
            // Double click
            if (item.Use()) {
                // Consumed item
                Destroy(gameObject);
            }
        } else {
            if (item.itemInfo.itemType == GameSystem.ItemType.Gun) {
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
                // Drop item
                gui.gunCustomization.ClearGunCustomization();
                Destroy(gameObject);
            } else {
                // Move item in inventory
                bool itemMoved = false;
                foreach (RaycastResult result in results) {
                    // Check if dragging item to available inventory space
                    if (result.gameObject.name == "InventorySpace" && WithinInventorySpace() && NotOverlappingItems()) {
                        // Removed from a slot
                        if (equippedSlot != null) {
                            if (item.itemInfo.itemType == GameSystem.ItemType.Gun) {
                                // Unequip gun
                                item.GetPlayerController().UnequipGun((Gun)item, equippedSlot.index);
                            }
                            equippedSlot.occupied = false;
                            equippedSlot = null;
                        }
                        itemMoved = true;
                        break;
                    }

                    // Drop item in a equip slot
                    ItemSlot slot = result.gameObject.GetComponent<ItemSlot>();
                    if (slot != null && slot.slotType == item.itemInfo.itemType) {
                        transform.position = result.gameObject.transform.position;
                        if (slot.occupied) {
                            // Swap with already-equipped item in slot
                            InventoryItem equippedItem = GetEquippedItem();
                            equippedItem.equippedSlot = null;
                            equippedItem.gameObject.transform.position = origPos;
                        }
                        equippedSlot = slot;
                        equippedSlot.occupied = true;

                        // Equip gun
                        if (slot.slotType == GameSystem.ItemType.Gun) {
                            item.GetPlayerController().EquipGun((Gun)item, equippedSlot.index);
                        }
                        itemMoved = true;
                        // Equip attachments
                        if (slot.slotType == GameSystem.ItemType.Magazine ||
                            slot.slotType == GameSystem.ItemType.Scope ||
                            slot.slotType == GameSystem.ItemType.Muzzle ||
                            slot.slotType == GameSystem.ItemType.Underbarrel) {
                            itemMoved = gui.gunCustomization.gun.AddAttachment(this);
                        }

                    }
                }

                if (!itemMoved) {
                    // Return item to original position if invalid destination
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
