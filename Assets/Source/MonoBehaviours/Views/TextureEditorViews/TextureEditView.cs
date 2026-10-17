

using UnityEngine;
using UnityEngine.UI;

public class TextureEditView: MonoBehaviour {

    public TextureEditMode controller;

    public GameObject graphicsPropertiesView;
    public GameObject texturePresetPanel;
    public Toggle openUVMapperToggle;
    public Image mapperPaintingIcon;

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

        if (Main.ignoreAllInputs) {
            return;
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

    public void OnClickMapperPainting() {

        if (!controller.mapperDrawing) {
            controller.StartDrawing();
        }
        else {
            controller.mapperDrawing = false;
        }

        ChangeToggleColor(mapperPaintingIcon, controller.mapperDrawing);

    }

    void ChangeToggleColor(Image image, bool toggle) {

        if (toggle) {
            image.color = Color.white;

        }
        else {
            image.color = Color.gray;

        }

    }


}