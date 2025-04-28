using TMPro;
using UnityEngine;

public class ShaderPresetsDirectoryViewItem : MonoBehaviour {

    // View refs
    public TMP_Text nameText;
    public TMP_InputField nameTextField;
    public ContextMenuHandler contextMenu;

    public ShaderPresets presets;
    public ShaderEditMode controller;
    public ShaderPresetsView view;
    public bool forceNameChange;

    void Start() {

        foreach (var gobj in transform.GetComponentsInChildren<ReceiveDragable>()) {
            gobj.expectedTransform = transform.parent;
        }

        nameText.text = presets.directoryName;

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        if (forceNameChange) {
            nameTextField.Select();
        }
        else {
            nameTextField.gameObject.SetActive(false);
        }

    }

    void Rename() {

        nameTextField.gameObject.SetActive(true);
        nameTextField.text = presets.directoryName;
        nameTextField.Select();
    }

    void Delete() {
        DialogWindowUtil.Dialog("Delete Folder", "Are you sure you want to delete folder " + presets.directoryName + "? " +
            "This will delete all presets inside", ConfirmDelete);
    }

    bool ConfirmDelete() {

        controller.currentShaderPresets.subFolders.Remove(presets);
        view.Refresh();

        return true;
    }

    public void OnStartNameType() {

        Main.ignoreAllInputs = true;

    }

    public void OnEndNameType() {

        Main.ignoreAllInputs = false;

        if (nameTextField.text == "") {
            presets.directoryName = "Shader Preset Folder";
        }
        else {
            presets.directoryName = nameTextField.text;
        }

        nameText.text = presets.directoryName;

        nameTextField.gameObject.SetActive(false);

    }

    public void OnClick() {

        if (controller.PresetPainting) {
            controller.selectedPreset = null;
        }

        controller.currentShaderPresets = presets;

        view.Refresh();

    }

    public void OnReceiverDrag() {

        if (Main.draggingElement.TryGetComponent<ShaderPresetViewItem>(out var viewItem)) {

            controller.currentShaderPresets.presets.Remove(viewItem.preset);
            presets.presets.Add(viewItem.preset);

            Destroy(viewItem.gameObject);

        }

    }

}