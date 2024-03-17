

using TMPro;
using UnityEngine;

public class BackUVPresetsViewItem : MonoBehaviour {

    public TextureEditMode controller;

    public TMP_Text text;

    public TexturePresetsView view;

    void Start() {

        text.text = controller.currentUVPresets.parent.directoryName;

    }

    public void OnClick() {

        controller.currentUVPresets = controller.currentUVPresets.parent;

        view.Refresh();

    }

}