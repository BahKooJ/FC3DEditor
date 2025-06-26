

using TMPro;
using UnityEditor.Presets;
using UnityEngine;

public class UVPresetsDirectoryViewItem : MonoBehaviour {

    // View refs
    public TMP_Text nameText;
    public TMP_InputField nameTextField;
    public ContextMenuHandler contextMenu;

    public UVPresets presets;
    public TextureEditMode controller;
    public TexturePresetsView view;
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

        controller.currentUVPresets.subFolders.Remove(presets);
        view.Refresh();

        return true;
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

    public void OnReceiverDrag() {

        if (Main.draggingElement.TryGetComponent<UVPresentViewItem>(out var viewItem)) {

            controller.currentUVPresets.presets.Remove(viewItem.preset);
            presets.presets.Add(viewItem.preset);

            Destroy(viewItem.gameObject);

        }

        if (Main.draggingElement.TryGetComponent<UVPresetsDirectoryViewItem>(out var viewItem2)) {

            controller.currentUVPresets.subFolders.Remove(viewItem2.presets);
            presets.subFolders.Add(viewItem2.presets);
            viewItem2.presets.parent = presets;

            Destroy(viewItem2.gameObject);

        }

    }

    public void OnReceiverDragInsert() {

        if (Main.draggingElement.TryGetComponent<UVPresetsDirectoryViewItem>(out var viewItem)) {

            var indexOfItem = controller.currentUVPresets.subFolders.IndexOf(viewItem.presets);
            var indexOfThis = controller.currentUVPresets.subFolders.IndexOf(presets);

            controller.currentUVPresets.subFolders.Remove(viewItem.presets);

            if (indexOfThis > indexOfItem) {

                controller.currentUVPresets.subFolders.Insert(indexOfThis - 1, viewItem.presets);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);

            }
            else {

                controller.currentUVPresets.subFolders.Insert(indexOfThis, viewItem.presets);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex());

            }

        }

    }


}