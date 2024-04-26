
using TMPro;
using UnityEngine;

public class BackColorPresetsViewItem : MonoBehaviour {

    public ShaderEditMode controller;

    public TMP_Text text;

    public ColorPresetsView view;

    void Start() {

        text.text = controller.currentColorPresets.parent.directoryName;

    }

    public void OnClick() {

        controller.currentColorPresets = controller.currentColorPresets.parent;

        view.Refresh();

    }

}