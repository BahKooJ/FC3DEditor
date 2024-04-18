

using UnityEngine;
using UnityEngine.UI;

public class ShaderEditPanelView : MonoBehaviour {

    // Prefabs
    public GameObject shaderMapper;
    public GameObject shaderPresetPanel;

    // View refs
    public Image paintTool;

    public ShaderEditMode controller;

    public GameObject activeShaderMapper = null;
    public GameObject activeShaderPresetPanel = null;

    void Start() {

        controller.view = this;

        activeShaderPresetPanel = Instantiate(shaderPresetPanel);

        activeShaderPresetPanel.GetComponent<ShaderPresetsView>().controller = controller;

        activeShaderPresetPanel.transform.SetParent(transform.parent, false);

        if (!controller.painting) {
            paintTool.color = Color.gray;
        }
        else {
            paintTool.color = Color.white;
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

}