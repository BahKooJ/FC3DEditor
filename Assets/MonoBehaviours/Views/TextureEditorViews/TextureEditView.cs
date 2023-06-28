

using UnityEngine;

public class TextureEditView: MonoBehaviour {

    public TextureEditMode controller;

    public GameObject graphicsPropertiesView;

    public GameObject activeGraphicsPropertiesView = null;

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (activeGraphicsPropertiesView != null) {
                CloseGraphicsPropertyView();
            }

        }

    }

    public void OnClickGraphicsProperitesButton() {

        if (controller.selectedTiles.Count == 0) { return; }

        if (activeGraphicsPropertiesView != null) {
            CloseGraphicsPropertyView();
        }
        else {

            activeGraphicsPropertiesView = Instantiate(graphicsPropertiesView);

            activeGraphicsPropertiesView.GetComponent<TextureUVMapper>().controller = controller;

            activeGraphicsPropertiesView.transform.SetParent(transform.parent, false);

        }

    }

    void CloseGraphicsPropertyView() {
        Destroy(activeGraphicsPropertiesView);
        controller.selectedSection.RefreshMesh();
    }



}