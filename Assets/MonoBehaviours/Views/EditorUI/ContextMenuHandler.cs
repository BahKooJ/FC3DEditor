

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO: Invert view if clips outside screen
public class ContextMenuHandler : MonoBehaviour, IPointerClickHandler {

    public List<(string, Action)> items;
    public bool isRightClick = true;

    public void OnPointerClick(PointerEventData eventData) {

        if (isRightClick) {
            if (eventData.button == PointerEventData.InputButton.Right) {
                OpenContextMenu();
            }
        } else {

            if (eventData.button == PointerEventData.InputButton.Left) {
                OpenContextMenu();
            }

        }



    }

    void OpenContextMenu() {

        ContextMenuUtil.CreateContextMenu(items);

    }

}