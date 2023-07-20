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

    public Toggle overrideMapData;
    public TMP_InputField widthField;
    public TMP_InputField heightField;

    void Start() {

    }

    public void OnValueChangedOverrideMapData() {

        widthField.interactable = overrideMapData.isOn;
        heightField.interactable = overrideMapData.isOn;

    }

    public void OnClickOpen() {

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
