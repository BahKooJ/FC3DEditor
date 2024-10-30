

using UnityEngine;

public abstract class LoadingScreenUtil {

    static public GameObject prefab;
    static public GameObject canvas;

    static GameObject activeView = null;

    public static void Show() {

        if (activeView != null) {
            Object.Destroy(activeView.gameObject);
        }

        var obj = Object.Instantiate(prefab);
        obj.transform.SetParent(canvas.transform, false);

        activeView = obj;

    }

    public static void End() {

        if (activeView != null) {
            Object.Destroy(activeView);
        }

        activeView = null;

    }

}