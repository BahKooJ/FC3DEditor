

using UnityEngine;

public class BackUVPresetsViewItem : MonoBehaviour {

    public TextureEditMode controller;

    public TexturePresetsView view;

    public void OnClick() {

        controller.currentUVPresets = controller.currentUVPresets.parent;

        view.Refresh();

    }

}