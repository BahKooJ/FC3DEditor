

using UnityEngine;
using UnityEngine.UI;

public class ShaderEditPanelView : MonoBehaviour {

    // Prefabs
    public GameObject shaderMapper;
    public GameObject shaderPresetPanel;

    // View refs
    public Image paintTool;
    public Image clickToggle;
    public Image switchPresets;

    public Sprite showShaderPresetsSprite;
    public Sprite showColorPresetsSprite;

    public ShaderEditMode controller;

    public GameObject activeShaderMapper = null;
    public GameObject activeShaderPresetPanel = null;

    public ShaderPresetsView shaderPresetsView;
    public ColorPresetsView colorPresetsView;

    void Start() {

        controller.view = this;

        activeShaderPresetPanel = Instantiate(shaderPresetPanel);

        shaderPresetsView = activeShaderPresetPanel.GetComponent<ShaderPresetsView>();
        colorPresetsView = activeShaderPresetPanel.GetComponent<ColorPresetsView>();

        shaderPresetsView.controller = controller;
        colorPresetsView.controller = controller;

        activeShaderPresetPanel.transform.SetParent(transform.parent, false);

        if (!controller.painting) {
            paintTool.color = Color.gray;
        }
        else {
            paintTool.color = Color.white;
        }

        if (!ShaderEditMode.applyColorsOnClick) {
            clickToggle.color = Color.gray;
        }
        else {
            clickToggle.color = Color.white;
        }

    }

    public void OpenShaderMapper() {

        if (activeShaderMapper != null) {
            CloseShaderMapper();
        }
        else {

            activeShaderMapper = Instantiate(shaderMapper);

            controller.colorPicker = activeShaderMapper.GetComponent<ShaderColorPickerView>();

            activeShaderMapper.GetComponent<ShaderColorPickerView>().controller = controller;

            activeShaderMapper.transform.SetParent(transform.parent, false);

        }

    }

    public void CloseShaderMapper() {

        Destroy(activeShaderMapper);

        controller.colorPicker = null;

        //if (controller.selectedSection != null) {
        //    controller.selectedSection.RefreshMesh();
        //}

    }

    public void ClosePresetPanel() {
        Destroy(activeShaderPresetPanel);
    }

    public void OnClickDuplicateShader() {

        controller.DuplicateTileShader();

    }

    public void PaintToolButton() {

        controller.StartPainting();

        if (!controller.painting) {
            paintTool.color = Color.gray;
        } else {
            paintTool.color = Color.white;
        }

    }

    public void OnClickApplyColorOnClickToggle() {

        controller.ChangeClickToggle();

        if (!ShaderEditMode.applyColorsOnClick) {
            clickToggle.color = Color.gray;
        }
        else {
            clickToggle.color = Color.white;
        }

    }

    public void OnClickSwitchPresets() {

        ShaderEditMode.showColorPresets = !ShaderEditMode.showColorPresets;

        if (ShaderEditMode.showColorPresets) {
            shaderPresetsView.Clear();
            colorPresetsView.Init();
            switchPresets.sprite = showColorPresetsSprite;
        }
        else {
            colorPresetsView.Clear();
            shaderPresetsView.Init();
            switchPresets.sprite = showShaderPresetsSprite;
        }

    }

}