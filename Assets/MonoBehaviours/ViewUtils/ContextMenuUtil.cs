

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class ContextMenuUtil {

    const float width = 200f;

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

        // Off screen test.
        // Not really sure why but don't scale the width or height.
        var pos = Input.mousePosition / Main.uiScaleFactor;
        var scaledScreenWidth = Screen.width / Main.uiScaleFactor;

        if (pos.x + width > scaledScreenWidth) {
            var dif = pos.x + width - scaledScreenWidth;
            pos.x -= dif;
        }

        // Since it's a content size fitter, it has to process the height.
        // Until I can figure something else out here's a magic number.
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