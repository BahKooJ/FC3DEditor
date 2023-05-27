using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class DialogWindowUtil {

    static public GameObject prefab;
    static public GameObject canvas;

    public static void Dialog(string title, string message, Func<bool> confirmAction = null) {

        var window = Object.Instantiate(prefab);

        var script = window.GetComponent<DialogWindow>();

        script.title.text = title;
        script.message.text = message;

        if (confirmAction == null) {
            script.cancelButton.SetActive(false);
        } else {
            script.confirmAction = confirmAction;
        }

        window.transform.SetParent(canvas.transform, false);

    }

    public static void FileLog(string message) {

    }

    public static void FileLogError(string message) {

        throw new Exception(message);

    }

}