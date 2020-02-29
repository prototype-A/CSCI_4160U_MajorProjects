using UnityEngine;
using UnityEngine.EventSystems;

public class UIDropHandler : MonoBehaviour, IDropHandler {

    public void OnDrop(PointerEventData eventData) {
        RectTransform inventoryPanel = transform as RectTransform;

        if (!RectTransformUtility.RectangleContainsScreenPoint(inventoryPanel, Input.mousePosition)) {
            Debug.Log("");
        }
    }
}
