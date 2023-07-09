

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UVPresetsDirectoryViewItem : MonoBehaviour {

    // View refs
    public TMP_Text nameText;
    public TMP_InputField nameTextField;

    public UVPresets presets;
    public TextureEditMode controller;
    public TexturePresetsView view;
    public bool forceNameChange;

    void Start() {

        nameText.text = presets.directoryName;

        if (forceNameChange) {
            nameTextField.Select();
        }
        else {
            nameTextField.gameObject.SetActive(false);
        }

    }

    public void OnStartNameType() {

        Main.ignoreAllInputs = true;

    }

    public void OnEndNameType() {

        Main.ignoreAllInputs = false;

        if (nameTextField.text == "") {
            presets.directoryName = "Texture Preset Folder";
        }
        else {
            presets.directoryName = nameTextField.text;
        }

        nameText.text = presets.directoryName;

        nameTextField.gameObject.SetActive(false);

    }

    public void OnClick() {

        controller.currentUVPresets = presets;

        view.Refresh();

    }


}