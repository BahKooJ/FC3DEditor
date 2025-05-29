
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableUIElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    // - Parameters -
    public List<ReceiveDragable> nestedRevieversInElement = new();
    public Action onDragCallback = () => { };

    [HideInInspector]
    public Transform originalParent;
    [HideInInspector]
    Vector2 originalAnchorMax;
    Vector2 originalAnchorMin;
    int originalSiblingIndex;
    Vector3 originalPosition;

    internal bool refuseDrag = false;

    bool started = false;
    public virtual void OnBeginDrag(PointerEventData eventData) {

        if (refuseDrag) return;

        onDragCallback();

        if (!Input.GetMouseButton(0) || started) return;

        foreach (var re in nestedRevieversInElement) {
            re.gameObject.SetActive(false);
        }


        var rectTrans = GetComponent<RectTransform>();

        originalAnchorMax = rectTrans.anchorMax;
        originalAnchorMin = rectTrans.anchorMin;
        originalSiblingIndex = transform.GetSiblingIndex();
        originalParent = transform.parent;
        originalPosition = rectTrans.anchoredPosition;

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

    public virtual void OnDrag(PointerEventData eventData) {

        if (refuseDrag) return;

        if (!Input.GetMouseButton(0) || !started) return;

        var rectTrans = GetComponent<RectTransform>();

        rectTrans.anchoredPosition = (Input.mousePosition + new Vector3((rectTrans.sizeDelta.x / 2) + 4, 0, 0)) / Main.uiScaleFactor;

    }

    public virtual void OnEndDrag(PointerEventData eventData) {

        if (refuseDrag) return;

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

        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);

        rectTrans.anchoredPosition = originalPosition;

        if (TryGetComponent<Button>(out var button)) {
            button.interactable = true;
        }

        foreach (var re in nestedRevieversInElement) {
            re.gameObject.SetActive(true);
        }

        if (results.Count > 0) {

            var resultWithReceiver = results.FirstOrDefault(result => result.gameObject.TryGetComponent<ReceiveDragable>(out var _));

            if (resultWithReceiver.gameObject != null) {
                resultWithReceiver.gameObject.GetComponent<ReceiveDragable>().ReceiveDrag(this);
            }

        }

        Main.draggingElement = null;

        started = false;

    }

}