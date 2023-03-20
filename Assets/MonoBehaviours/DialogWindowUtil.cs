

using System.Linq;
using UnityEditor;

public class DialogWindowUtil {

    public static string OpenFile(string title, string path = "", string extension = "") {

        return EditorUtility.OpenFilePanel(title, path, extension);
    }

    public static string SaveFile(string title, string path = "", string name = "", string extension = "") {
        return EditorUtility.SaveFilePanel(title, path, name, extension);
    }

    public static bool Dialog(string title, string message, string confirm, string cancel = "") {

        if (cancel.Count() == 0) {
            return EditorUtility.DisplayDialog(title, message, confirm);
        } else {
            return EditorUtility.DisplayDialog(title, message, confirm, cancel);

        }

    }

}