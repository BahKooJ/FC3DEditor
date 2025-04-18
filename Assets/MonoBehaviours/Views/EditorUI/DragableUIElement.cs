
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableUIElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    // - Parameters -
    public List<ReceiveDragable> nestedRevieversInElement = new();

    [HideInInspector]
    public Transform originalParent;
    [HideInInspector]
    Vector2 originalAnchorMax;
    Vector2 originalAnchorMin;
    int originalSiblingIndex;

    bool started = false;
    public void OnBeginDrag(PointerEventData eventData) {

        if (!Input.GetMouseButton(0) || started) return;

        foreach (var re in nestedRevieversInElement) {
            re.gameObject.SetActive(false);
        }


        var rectTrans = GetComponent<RectTransform>();

        originalAnchorMax = rectTrans.anchorMax;
        originalAnchorMin = rectTrans.anchorMin;
        originalSiblingIndex = transform.GetSiblingIndex();
        originalParent = transform.parent;

        rectTrans.anchorMax = Vector2.zero;
        rectTrans.anchorMin = Vector2.zero;

        transform.SetParent(transform.root);

        rectTrans.anchoredPosition = (Input.mousePosition + new Vector3((rectTrans.sizeDelta.x / 2) + 4, 0, 0)) / Main.uiScaleFactor;

        if (TryGetComponent<Button>(out var button)) {
            button.interactable = false;
        }

        Main.draggingElement = this;

        started = true;
    }

    public void OnDrag(PointerEventData eventData) {

        if (!Input.GetMouseButton(0) || !started) return;

        var rectTrans = GetComponent<RectTransform>();

        rectTrans.anchoredPosition = (Input.mousePosition + new Vector3((rectTrans.sizeDelta.x / 2) + 4, 0, 0)) / Main.uiScaleFactor;

    }

    public void OnEndDrag(PointerEventData eventData) {

        if (!started) return;

        if (originalParent == null) {
            Destroy(gameObject); 
            return;
        }

        var pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        var rectTrans = GetComponent<RectTransform>();

        rectTrans.anchorMax = originalAnchorMax;
        rectTrans.anchorMin = originalAnchorMin;

        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);

        if (TryGetComponent<Button>(out var button)) {
            button.interactable = true;
        }

        foreach (var re in nestedRevieversInElement) {
            re.gameObject.SetActive(true);
        }

        if (results.Count > 0) {

            if (results[0].gameObject.TryGetComponent<ReceiveDragable>(out var receiver)) {

                receiver.ReceiveDrag(this);

            }

        }

        Main.draggingElement = null;

        started = false;

    }

}