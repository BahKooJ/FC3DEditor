

using FCopParser;
using TMPro;
using UnityEngine;
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

        switch (preset.type) {
            case VertexColorType.MonoChrome:

                var SolidMonoColor = preset.monoValue / MonoChromeShader.white;

                if (SolidMonoColor > 1f) {
                    colorPreview.color = new Color(SolidMonoColor - 1f, 1f, SolidMonoColor - 1f);
                }
                else {
                    colorPreview.color = new Color(0f, SolidMonoColor, 0f);
                }

                break;
            case VertexColorType.DynamicMonoChrome:

                var monoColor = preset.monoValue / DynamicMonoChromeShader.white;

                if (monoColor > 1f) {
                    colorPreview.color = new Color(monoColor - 1f, 1f, monoColor - 1f);
                }
                else {
                    colorPreview.color = new Color(0f, monoColor, 0f);
                }


                break;
            case VertexColorType.Color:

                var colors = preset.colorValue.ToColors();

                colorPreview.color = new Color(colors[0], colors[1], colors[2]);

                break;
            case VertexColorType.ColorAnimated:
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
            preset.name = "Color Preset";
        }
        else {
            preset.name = nameTextField.text;
        }

        nameText.text = preset.name;

        nameTextField.gameObject.SetActive(false);

    }

    public void OnClick() {

        if (controller.colorPicker != null) {
            controller.colorPicker.SetColorFromPreset(preset);
        }

    }

}