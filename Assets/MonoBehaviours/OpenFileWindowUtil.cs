

using System;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class OpenFileWindowUtil {

    static public GameObject prefab;
    static public GameObject canvas;

    public static void OpenFile(string directory, string defaultFileName, Action<string> openFileAction) {

        var window = Object.Instantiate(prefab);

        var script = window.GetComponent<OpenFileWindowView>();

        script.isOpen = true;
        script.directory = directory;
        script.defaultFileName = defaultFileName;
        script.confirmAction = openFileAction;

        window.transform.SetParent(canvas.transform, false);

    }

    public static void SaveFile(string directory, string defaultFileName, Action<string> saveFileAction) {

        var window = Object.Instantiate(prefab);

        var script = window.GetComponent<OpenFileWindowView>();

        script.isOpen = false;
        script.directory = directory;
        script.defaultFileName = defaultFileName;
        script.confirmAction = saveFileAction;

        window.transform.SetParent(canvas.transform, false);

    }


}