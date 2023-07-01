

using UnityEngine;

public class TextureEditView: MonoBehaviour {

    public TextureEditMode controller;

    public GameObject graphicsPropertiesView;

    public GameObject activeTextureUVMapper = null;

    void Start() {

        controller.view = this;

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (activeTextureUVMapper != null) {
                CloseTextureUVMapper();
            }

        }

    }

    public void OnClickGraphicsProperitesButton() {

        if (controller.selectedTiles.Count == 0) { return; }

        if (activeTextureUVMapper != null) {
            CloseTextureUVMapper();
        }
        else {

            activeTextureUVMapper = Instantiate(graphicsPropertiesView);

            activeTextureUVMapper.GetComponent<TextureUVMapper>().controller = controller;

            activeTextureUVMapper.transform.SetParent(transform.parent, false);

        }

    }

    public void CloseTextureUVMapper() {
        Destroy(activeTextureUVMapper);
        controller.selectedSection.RefreshMesh();
    }



}