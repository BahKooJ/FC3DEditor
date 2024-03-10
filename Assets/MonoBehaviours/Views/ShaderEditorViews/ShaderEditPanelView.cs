

using UnityEngine;

public class ShaderEditPanelView : MonoBehaviour {

    // Prefabs
    public GameObject shaderMapper;

    public ShaderEditMode controller;

    public GameObject activeShaderMapper = null;

    void Start() {

        controller.view = this;

    }

    public void OpenShaderMapper() {

        //if (controller.selectedTiles.Count == 0) { return; }

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

}