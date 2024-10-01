

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class ContextMenuUtil {

    // Prefabs
    public static GameObject container;
    public static GameObject contextMenuItem;

    static public GameObject canvas;

    public static void CreateContextMenu(List<(string, Action)> items) {

        var existingContextMenus = GameObject.FindObjectsByType<ContextMenuContainer>(FindObjectsSortMode.None);

        foreach (var menu in existingContextMenus) {

            Object.DestroyImmediate(menu.gameObject);

        }

        var containerObj = Object.Instantiate(container);

        containerObj.transform.SetParent(canvas.transform, false);

        var rectTrans = (RectTransform)containerObj.transform;

        var pos = Input.mousePosition / Main.uiScaleFactor;

        if (pos.y - 100 < 0) {
            pos.y -= pos.y - 100;

        }

        rectTrans.anchoredPosition = pos;


        foreach (var item in items) {

            var menuItem = Object.Instantiate(contextMenuItem);

            var script = menuItem.GetComponent<ContextMenuItemView>();

            script.title = item.Item1;
            script.clickAction = item.Item2;
            script.container = containerObj;

            menuItem.transform.SetParent(containerObj.transform.GetChild(0), false);

        }

    }

}