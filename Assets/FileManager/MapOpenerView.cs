using FCopParser;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapOpenerView : MonoBehaviour {

    public FileManagerMain main;

    public TMP_InputField pathString;
    public Toggle overrideMapData;
    public TMP_InputField widthField;
    public TMP_InputField heightField;

    void Start() {

        pathString.text = FileManagerMain.savePath;

    }

    public void OnValueChangedOverrideMapData() {

        widthField.interactable = overrideMapData.isOn;
        heightField.interactable = overrideMapData.isOn;

    }

    public void OnTextPathChange() {
        FileManagerMain.savePath = pathString.text;
    }

    public void OnClickOpen() {

        if (FileManagerMain.savePath == "") {
            DialogWindowUtil.Dialog("Select Save Path", "Please select a file path to save the mission file");
            return;
        }

        if (overrideMapData.isOn) {

            var width = Int32.Parse(widthField.text);
            var height = Int32.Parse(heightField.text);

            if (width != 0 && height != 0) {

                FileManagerMain.level = new FCopLevel(width, height, FileManagerMain.iffFile.parsedData);

            } else {

                FileManagerMain.level = new FCopLevel(FileManagerMain.iffFile.parsedData);

            }

        } else {

            FileManagerMain.level = new FCopLevel(FileManagerMain.iffFile.parsedData);

        }

        SceneManager.LoadScene("Scenes/LevelEditorScene", LoadSceneMode.Single);

    }


}
