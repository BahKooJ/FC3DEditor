

using TMPro;
using UnityEngine;

public class BackShaderPresetsViewItem : MonoBehaviour {

    public ShaderEditMode controller;

    public TMP_Text text;

    public ShaderPresetsView view;

    void Start() {

        foreach (var gobj in transform.GetComponentsInChildren<ReceiveDragable>()) {
            gobj.expectedTransform = transform.parent;
        }

        text.text = controller.currentShaderPresets.parent.directoryName;

    }

    public void OnClick() {

        controller.currentShaderPresets = controller.currentShaderPresets.parent;

        view.Refresh();

    }

    public void OnReceiverDrag() {

        if (Main.draggingElement.TryGetComponent<ShaderPresetViewItem>(out var viewItem)) {

            controller.currentShaderPresets.presets.Remove(viewItem.preset);
            controller.currentShaderPresets.parent.presets.Add(viewItem.preset);

            Destroy(viewItem.gameObject);

        }

        if (Main.draggingElement.TryGetComponent<ShaderPresetsDirectoryViewItem>(out var viewItem2)) {

            controller.currentShaderPresets.subFolders.Remove(viewItem2.presets);
            controller.currentShaderPresets.parent.subFolders.Add(viewItem2.presets);
            viewItem2.presets.parent = controller.currentShaderPresets.parent;

            Destroy(viewItem2.gameObject);

        }

    }

}