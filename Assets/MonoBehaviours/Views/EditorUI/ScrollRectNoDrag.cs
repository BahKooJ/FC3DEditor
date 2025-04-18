using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectNoDrag : ScrollRect {
    public override void OnBeginDrag(PointerEventData eventData) { }
    public override void OnDrag(PointerEventData eventData) { }
    public override void OnEndDrag(PointerEventData eventData) { }
}