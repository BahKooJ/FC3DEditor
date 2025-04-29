

using TMPro;
using UnityEngine;

public class BackUVPresetsViewItem : MonoBehaviour {

    public TextureEditMode controller;

    public TMP_Text text;

    public TexturePresetsView view;

    void Start() {

        foreach (var gobj in transform.GetComponentsInChildren<ReceiveDragable>()) {
            gobj.expectedTransform = transform.parent;
        }

        text.text = controller.currentUVPresets.parent.directoryName;

    }

    public void OnClick() {

        controller.currentUVPresets = controller.currentUVPresets.parent;

        view.Refresh();

    }

    public void OnReceiverDrag() {

        if (Main.draggingElement.TryGetComponent<UVPresentViewItem>(out var viewItem)) {

            controller.currentUVPresets.presets.Remove(viewItem.preset);
            controller.currentUVPresets.parent.presets.Add(viewItem.preset);

            Destroy(viewItem.gameObject);

        }

    }

}