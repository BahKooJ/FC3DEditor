
using TMPro;
using UnityEngine;

public class BackColorPresetsViewItem : MonoBehaviour {

    public ShaderEditMode controller;

    public TMP_Text text;

    public ColorPresetsView view;

    void Start() {

        foreach (var gobj in transform.GetComponentsInChildren<ReceiveDragable>()) {
            gobj.expectedTransform = transform.parent;
        }

        text.text = controller.currentColorPresets.parent.directoryName;

    }

    public void OnClick() {

        controller.currentColorPresets = controller.currentColorPresets.parent;

        view.Refresh();

    }

    public void OnReceiverDrag() {

        if (Main.draggingElement.TryGetComponent<ColorPresetViewItem>(out var viewItem)) {

            controller.currentColorPresets.presets.Remove(viewItem.preset);
            controller.currentColorPresets.parent.presets.Add(viewItem.preset);

            Destroy(viewItem.gameObject);

        }

        if (Main.draggingElement.TryGetComponent<ColorPresetsDirectoryViewItem>(out var viewItem2)) {

            controller.currentColorPresets.subFolders.Remove(viewItem2.presets);
            controller.currentColorPresets.parent.subFolders.Add(viewItem2.presets);
            viewItem2.presets.parent = controller.currentColorPresets.parent;

            Destroy(viewItem2.gameObject);

        }

    }

}