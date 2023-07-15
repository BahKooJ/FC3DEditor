

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO: Invert view if clips outside screen
public class ContextMenuHandler : MonoBehaviour, IPointerClickHandler {

    public List<(string, Action)> items;

    public void OnPointerClick(PointerEventData eventData) {

        if (eventData.button == PointerEventData.InputButton.Right) {
            OpenContextMenu();
        }

    }

    void OpenContextMenu() {

        ContextMenuUtil.CreateContextMenu(items);

    }

}