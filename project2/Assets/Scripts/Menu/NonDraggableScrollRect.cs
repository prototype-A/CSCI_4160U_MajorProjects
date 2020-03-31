using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NonDraggableScrollRect : ScrollRect {
    // Only allows scrolling with mouse wheel
    public override void OnBeginDrag(PointerEventData eventData) {}
    public override void OnDrag(PointerEventData eventData) {}
    public override void OnEndDrag(PointerEventData eventData) {}
}
