

using TMPro;
using UnityEngine;

public class BackShaderPresetsViewItem : MonoBehaviour {

    public ShaderEditMode controller;

    public TMP_Text text;

    public ShaderPresetsView view;

    void Start() {

        text.text = controller.currentShaderPresets.parent.directoryName;

    }

    public void OnClick() {

        controller.currentShaderPresets = controller.currentShaderPresets.parent;

        view.Refresh();

    }

}