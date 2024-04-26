

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class ColorPresetViewItem : MonoBehaviour {

    // View refs
    public TMP_Text typeText;
    public TMP_Text nameText;
    public TMP_InputField nameTextField;
    public ContextMenuHandler contextMenu;
    public Image colorPreview;

    public ColorPreset preset;
    public ShaderEditMode controller;
    public ColorPresetsView view;
    public bool forceNameChange;

    void Start() {

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        nameText.text = preset.name;

        if (forceNameChange) {
            nameTextField.Select();
        }
        else {
            nameTextField.gameObject.SetActive(false);
        }

        switch (preset.type) {

            case VertexColorType.MonoChrome:
                typeText.text = "Solid Monochrome";
                break;
            case VertexColorType.DynamicMonoChrome:
                typeText.text = "Monochrome";
                break;
            case VertexColorType.Color:
                typeText.text = "Color";
                break;
            case VertexColorType.ColorAnimated:
                typeText.text = "Animated";
                break;

        }

    }

    void Rename() {

        nameTextField.gameObject.SetActive(true);
        nameTextField.text = preset.name;
        nameTextField.Select();

    }

    void Delete() {
        DialogWindowUtil.Dialog("Delete Preset", "Are you sure you want to delete preset " + preset.name + "?", ConfirmDelete);
    }

    bool ConfirmDelete() {

        controller.currentColorPresets.presets.Remove(preset);
        view.Refresh();

        return true;
    }

    public void OnStartNameType() {

        Main.ignoreAllInputs = true;

    }

    public void OnEndNameType() {

        Main.ignoreAllInputs = false;

        if (nameTextField.text == "") {
            preset.name = "Shader Preset";
        }
        else {
            preset.name = nameTextField.text;
        }

        nameText.text = preset.name;

        nameTextField.gameObject.SetActive(false);

    }

    public void OnClick() {



    }

}