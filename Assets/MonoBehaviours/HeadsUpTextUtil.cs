

using UnityEngine;

public abstract class HeadsUpTextUtil {

    static public GameObject prefab;
    static public GameObject canvas;

    static HeadsUpTextView activeView = null;

    public static void HeadsUp(string message) {

        if (activeView != null) {
            Object.Destroy(activeView.gameObject);
        }

        var obj = Object.Instantiate(prefab);
        obj.transform.SetParent(canvas.transform, false);

        var headsUpText = obj.GetComponent<HeadsUpTextView>();
        headsUpText.message = message;
        activeView = headsUpText;

    }

    public static void End() {

        if (activeView != null) {
            Object.Destroy(activeView.gameObject);
        }

        activeView = null;

    }

}