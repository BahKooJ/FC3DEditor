

using System.Linq;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

class GraphicsPropertiesView : MonoBehaviour {

    public Main controller;

    public GameObject textureOffsetItem;
    public GameObject graphicsPresetItem;

    GameObject texturePalletteDropdown;
    GameObject rectangleTileToggle;
    GameObject texturePallete;
    GameObject textureOffsets;
    GameObject graphicsPreset;

    void Start() {

        InitView();

    }

    public void RefreshView() {

        DestoryTextureOffsets();
        DestoryGraphicsPreset();

        var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTile.graphicsIndex];

        texturePalletteDropdown.GetComponent<TMP_Dropdown>().value = graphics.number2;

        rectangleTileToggle.GetComponent<Toggle>().isOn = graphics.number4 == 1;

        var texture = new Texture2D(256, 256, TextureFormat.RGB565, false);

        texture.LoadRawTextureData(controller.level.textures[graphics.number2].ConvertToRGB565());
        texture.Apply();

        texturePallete.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero);

        InitTextureOffsets();

        InitGraphicsPreset();

    }

    public void OnClickExportTexture() {

        var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTile.graphicsIndex];

        File.WriteAllBytes("bmp" + graphics.number2.ToString() + ".bmp", controller.level.textures[graphics.number2].BitmapWithHeader());

    }

    public void OnClickImportTexture() {

        var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTile.graphicsIndex];

        controller.level.textures[graphics.number2].ImportBMP(File.ReadAllBytes("bmp" + graphics.number2.ToString() + ".bmp"));

        texturePalletteDropdown.GetComponent<TMP_Dropdown>().value = graphics.number2;

        rectangleTileToggle.GetComponent<Toggle>().isOn = graphics.number4 == 1;

        var texture = new Texture2D(256, 256, TextureFormat.RGB565, false);

        texture.LoadRawTextureData(controller.level.textures[graphics.number2].ConvertToRGB565());
        texture.Apply();

        texturePallete.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero);

        controller.RefreshTextures();

        controller.selectedSection.RefreshMesh();
        controller.selectedSection.RefreshTexture();

    }

    void InitView() {

        foreach (Object obj in transform) {

            switch (obj.GameObject().name) {

                case "Texture Pallette Dropdown":
                    texturePalletteDropdown = obj.GameObject();
                    break;
                case "Rectangle Tile Toggle":
                    rectangleTileToggle = obj.GameObject();
                    break;
                case "Texture Pallette":
                    texturePallete = obj.GameObject();
                    break;
                case "Texture Offsets":
                    textureOffsets = obj.GameObject();
                    break;
                case "Graphics Presets":
                    graphicsPreset = obj.GameObject();
                    break;

            }

        }

        var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTile.graphicsIndex];

        texturePalletteDropdown.GetComponent<TMP_Dropdown>().value = graphics.number2;

        rectangleTileToggle.GetComponent<Toggle>().isOn = graphics.number4 == 1;

        var texture = new Texture2D(256, 256, TextureFormat.RGB565, false);

        texture.LoadRawTextureData(controller.level.textures[graphics.number2].ConvertToRGB565());
        texture.Apply();

        texturePallete.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero);

        InitTextureOffsets();

        InitGraphicsPreset();

    }

    void InitTextureOffsets() {

        var content = textureOffsets.transform.GetChild(0).GameObject().transform.GetChild(0);

        foreach (var index in Enumerable.Range(0, controller.selectedSection.section.textureCoordinates.Count)) {

            var item = Instantiate(textureOffsetItem);

            var script = item.GetComponent<TextureOffsetItem>();

            script.controller = controller;

            script.view = this;

            script.index = index;

            item.transform.SetParent(content);

        }

    }

    void InitGraphicsPreset() {

        var content = graphicsPreset.transform.GetChild(0).GameObject().transform.GetChild(0);

        foreach (var index in Enumerable.Range(0, controller.selectedSection.section.tileGraphics.Count)) {

            var item = Instantiate(graphicsPresetItem);

            var script = item.GetComponent<GraphicsPresetItem>();

            script.controller = controller;

            script.view = this;

            script.index = index;

            item.transform.SetParent(content);

        }

    }

    void DestoryTextureOffsets() {
        var content = textureOffsets.transform.GetChild(0).GameObject().transform.GetChild(0);

        foreach (Object obj in content.transform) {
            Destroy(obj.GameObject());
        }

    }

    void DestoryGraphicsPreset() {

        var content = graphicsPreset.transform.GetChild(0).GameObject().transform.GetChild(0);

        foreach (Object obj in content.transform) {
            Destroy(obj.GameObject());
        }

    }

}