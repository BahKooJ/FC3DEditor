﻿

using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    bool insideItem = false;

    public void OnPointerEnter(PointerEventData eventData) {
        insideItem = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        insideItem = false;
    }

    void Update() {

        if (Main.draggingElement != null) {
            Destroy(gameObject);
        }

        if (Input.GetMouseButtonDown(0)) {

            if (!insideItem) {
                Destroy(gameObject);
            }

        }

    }

}