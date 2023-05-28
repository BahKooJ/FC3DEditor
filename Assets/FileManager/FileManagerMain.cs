using FCopParser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileManagerMain : MonoBehaviour {

    public RectTransform canvas;

    public GameObject MapOpenerView;
    public GameObject DialogWindow;


    public static IFFParser iffFile;
    public static FCopLevel level;
    public static string savePath = "Output/";

    void Start() {

        DialogWindowUtil.prefab = DialogWindow;
        DialogWindowUtil.canvas = canvas.gameObject;

        SettingsManager.ParseSettings();

    }

    public void OpenFile(string path) {

        var fileContent = File.ReadAllBytes(path);

        try {
            iffFile = new IFFParser(fileContent);
        } catch (InvalidFileException) {
            DialogWindowUtil.Dialog("Select Future Cop mission File", "This file is not a mission file");
        }

        foreach (Transform child in canvas) {
            Destroy(child.gameObject);
        }

        var view = Instantiate(MapOpenerView);
        view.GetComponent<MapOpenerView>().main = this;
        view.transform.SetParent(canvas, false);

    }


}
