

using System.Linq;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using FCopParser;
using System;
using Object = UnityEngine.Object;

public class GraphicsPropertiesView : MonoBehaviour {

    public Main controller;

    public GameObject textureOffsetItem;
    public GameObject graphicsPresetItem;

    public GameObject texturePalletteDropdown;
    public GameObject rectangleTileToggle;
    public GameObject texturePallete;
    public GameObject textureOffsets;
    public GameObject graphicsPreset;
    public GameObject textureInputX;
    public GameObject textureInputY;

    public TextureCoordinatesLines textureLines;

    public int bmpID;

    void Start() {

        InitView();

    }

    public void RefreshView() {

        DestoryTextureOffsets();
        DestoryGraphicsPreset();

        if (controller.selectedTiles.Count == 1) { 
            var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

            rectangleTileToggle.GetComponent<Toggle>().isOn = graphics.number4 == 1;
        }

        var sameTexture = true;

        bmpID = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex].number2; ;
        foreach (var tile in controller.selectedTiles) {

            if (bmpID != controller.selectedSection.section.tileGraphics[tile.graphicsIndex].number2) {
                sameTexture = false;
                bmpID = -1;
                break;
            }

        }

        if (sameTexture) {
            var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

            texturePalletteDropdown.GetComponent<TMP_Dropdown>().value = graphics.number2;

            var texture = new Texture2D(256, 256, TextureFormat.RGB565, false);

            texture.LoadRawTextureData(controller.level.textures[graphics.number2].ConvertToRGB565());
            texture.Apply();

            texturePallete.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero);
        }

        InitTextureOffsets();

        InitGraphicsPreset();

    }

    public void RefreshTextureOffsetsView() {
        DestoryTextureOffsets();
        InitTextureOffsets();
    }

    public void OnClickExportTexture() {

        controller.ExportTexture(this.bmpID);

    }

    public void OnClickImportTexture() {

        controller.ImportTexture(bmpID);

        var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

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

    public void OnChangeTexturePalleteValue() {

        controller.ChangeTexturePallette(texturePalletteDropdown.GetComponent<TMP_Dropdown>().value);

    }

    public void OnClickAddTexture() {

        controller.selectedSection.section.textureCoordinates.Add(0);

        DestoryTextureOffsets();
        InitTextureOffsets();

    }

    public void OnSetTextureCordX() {

        var comp = textureInputX.GetComponent<TMP_InputField>();

        try {
            controller.SetTextureCordX(Int32.Parse(comp.text));
        } catch {
            return;
        }

        DestoryTextureOffsets();
        InitTextureOffsets();
        textureLines.Refresh();

    }

    public void OnSetTextureCordY() {

        var comp = textureInputY.GetComponent<TMP_InputField>();

        try {
            controller.SetTextureCordY(Int32.Parse(comp.text));
        } catch {
            return;
        }

        DestoryTextureOffsets();
        InitTextureOffsets();
        textureLines.Refresh();
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
                case "Texture Input X":
                    textureInputX = obj.GameObject();
                    break;
                case "Texture Input Y":
                    textureInputY = obj.GameObject();
                    break;

            }

        }

        if (controller.selectedTiles.Count == 1) {
            var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

            rectangleTileToggle.GetComponent<Toggle>().isOn = graphics.number4 == 1;
        }

        var sameTexture = true;

        bmpID = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex].number2; ;
        foreach (var tile in controller.selectedTiles) {

            if (bmpID != controller.selectedSection.section.tileGraphics[tile.graphicsIndex].number2) {
                sameTexture = false;
                bmpID = -1;
                break;
            }

        }

        if (sameTexture) {
            var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

            texturePalletteDropdown.GetComponent<TMP_Dropdown>().value = graphics.number2;

            var texture = new Texture2D(256, 256, TextureFormat.RGB565, false);

            texture.LoadRawTextureData(controller.level.textures[graphics.number2].ConvertToRGB565());
            texture.Apply();

            texturePallete.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero);
        }

        InitTextureOffsets();

        InitGraphicsPreset();

    }

    public void InitTextureOffsets() {

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

    public void InitGraphicsPreset() {

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

    public void DestoryTextureOffsets() {
        var content = textureOffsets.transform.GetChild(0).GameObject().transform.GetChild(0);

        foreach (Object obj in content.transform) {
            Destroy(obj.GameObject());
        }

    }

    public void DestoryGraphicsPreset() {

        var content = graphicsPreset.transform.GetChild(0).GameObject().transform.GetChild(0);

        foreach (Object obj in content.transform) {
            Destroy(obj.GameObject());
        }

    }

}