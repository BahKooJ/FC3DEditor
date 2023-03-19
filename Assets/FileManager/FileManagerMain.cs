using FCopParser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileManagerMain : MonoBehaviour {

    public RectTransform canvas;

    public GameObject MapOpenerView;

    public static IFFParser iffFile;
    public static FCopLevel level;
    public static string savePath = "";

    public void OpenFile() {

        var path = EditorUtility.OpenFilePanel("Open Mission File", "", "");

        if (path.Length != 0) {

            var fileContent = File.ReadAllBytes(path);

            try {
                iffFile = new IFFParser(fileContent);
            } catch (InvalidFileException) {
                EditorUtility.DisplayDialog("Select Future Cop mission File", "This file is not a mission file", "OK");
            }

            foreach (Transform child in canvas) {
                Destroy(child.gameObject);
            }

            var view = Instantiate(MapOpenerView);
            view.GetComponent<MapOpenerView>().main = this;
            view.transform.SetParent(canvas, false);

        }

    }

    public string SetSavePath() {

        var path = EditorUtility.SaveFilePanel("Save Path", "", "", "");
        savePath = path;

        return path;

    }

}
