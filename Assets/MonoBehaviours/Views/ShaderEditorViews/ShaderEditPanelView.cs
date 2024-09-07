

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
    public Image presetPaintTool;

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

        GrayOutAllTools();

    }

    void Update() {

        if (Main.ignoreAllInputs) {
            return;
        }

        if (Controls.OnDown("DuplicateTileShaders")) {
            OnClickDuplicateShader();
        }
        if (Controls.OnDown("OpenCloseColorPicker")) {
            OpenShaderMapper();
        }
        if (Controls.OnDown("PaintTool")) {
            PaintToolButton();
        }
        if (Controls.OnDown("ApplyColorOnClick")) {
            OnClickApplyColorOnClickToggle();
        }
        if (Controls.OnDown("SwitchPresets")) {
            OnClickSwitchPresets();
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

        controller.SelectTool(2);

        GrayOutAllTools();

        if (!controller.Default) {
            paintTool.color = Color.white;
        }


    }

    public void PresetPaintToolButton() {

        controller.SelectTool(3);

        GrayOutAllTools();

        if (!controller.Default) {
            presetPaintTool.color = Color.white;
        }


    }

    public void OnClickApplyColorOnClickToggle() {

        controller.SelectTool(1);

        GrayOutAllTools();

        if (!controller.Default) {
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

    void ChangeToggleColor(Image image, bool toggle) {

        if (toggle) {
            image.color = Color.white;

        }
        else {
            image.color = Color.gray;

        }

    }

    void GrayOutAllTools() {
        paintTool.color = Color.gray;
        clickToggle.color = Color.gray;
        presetPaintTool.color = Color.gray;
    }


}