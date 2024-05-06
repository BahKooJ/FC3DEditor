

using UnityEngine;
using UnityEngine.UI;

public class TextureEditView: MonoBehaviour {

    public TextureEditMode controller;

    public GameObject graphicsPropertiesView;
    public GameObject texturePresetPanel;
    public Toggle openUVMapperToggle;

    public GameObject activeTextureUVMapper = null;
    public GameObject activeTexturePresetPanel = null;


    void Start() {

        controller.view = this;

        activeTexturePresetPanel = Instantiate(texturePresetPanel);

        activeTexturePresetPanel.GetComponent<TexturePresetsView>().controller = controller;

        activeTexturePresetPanel.transform.SetParent(transform.parent, false);

        openUVMapperToggle.isOn = TextureEditMode.openUVMapperByDefault;

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (activeTextureUVMapper != null) {
                CloseTextureUVMapper();
            }

        }

        if (Controls.OnDown("DuplicateTileTextures")) {
            OnClickDuplicateTileTextures();
        }
        if (Controls.OnDown("OpenCloseUVMapper")) {
            OpenUVMapper();
        }

    }

    public void OpenUVMapper() {

        //if (controller.selectedTiles.Count == 0) { return; }

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

        controller.RefreshMeshes();

    }

    public void CloseTexturePresetPanel() {
        Destroy(activeTexturePresetPanel);
    }

    public void OnClickDuplicateTileTextures() {

        controller.DuplicateTileUVs();

    }

    public void OnChangeOpenUVMapperOnTileSelect() {
        TextureEditMode.openUVMapperByDefault = openUVMapperToggle.isOn;
    }

    public void OnClickMakeTilesOpaque() {
        controller.MakeTilesOpaque();
    }

    public void OnClickMakeTilesTransparent() {
        controller.MakeTilesTransparent();
    }


}