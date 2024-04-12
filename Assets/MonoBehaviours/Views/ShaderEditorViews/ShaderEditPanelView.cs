

using UnityEngine;
using UnityEngine.UI;


public class ShaderEditPanelView : MonoBehaviour {

    // Prefabs
    public GameObject shaderMapper;
    public GameObject shaderPresetPanel;

    public Toggle openShaderMapperToggle;

    public ShaderEditMode controller;

    public GameObject activeShaderMapper = null;
    public GameObject activeShaderPresetPanel = null;

    void Start() {

        controller.view = this;

        activeShaderPresetPanel = Instantiate(shaderPresetPanel);

        activeShaderPresetPanel.GetComponent<ShaderPresetsView>().controller = controller;

        activeShaderPresetPanel.transform.SetParent(transform.parent, false);

        openShaderMapperToggle.isOn = ShaderEditMode.openShaderMapperByDefault;

    }

    public void OpenShaderMapper() {

        if (controller.selectedItems.Count == 0) { return; }

        if (activeShaderMapper != null) {
            CloseShaderMapper();
        }
        else {

            activeShaderMapper = Instantiate(shaderMapper);

            activeShaderMapper.GetComponent<ShaderMapperView>().controller = controller;

            activeShaderMapper.transform.SetParent(transform.parent, false);

        }

    }

    public void CloseShaderMapper() {

        Destroy(activeShaderMapper);

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

    public void OnChangeOpenMapperOnTileSelect() {
        ShaderEditMode.openShaderMapperByDefault = openShaderMapperToggle.isOn;

    }

}