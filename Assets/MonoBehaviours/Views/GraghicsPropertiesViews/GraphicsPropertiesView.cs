

using System.Linq;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using FCopParser;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;

public class GraphicsPropertiesView : MonoBehaviour {

    public static List<int> textureCoordsClipboard = new();

    public GeometryEditMode controller;

    // --Prefabs--
    public GameObject textureOffsetItem;
    public GameObject graphicsPresetItem;
    public GameObject textureCoordinatePoint;

    // --View References--
    public GameObject texturePalletteDropdown;
    public GameObject rectangleTileToggle;
    public GameObject texturePallete;
    public GameObject texturePalleteImage;
    public GameObject textureOffsets;
    public GameObject graphicsPreset;
    public TextureCoordinatesLines textureLines;

    public int bmpID;
    public int? globalTextureCoordIndex = null;


    void Start() {

        InitView();

    }

    void Update() {

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0) {
            var scale = texturePalleteImage.transform.localScale;

            scale.x += axis * 4;
            scale.y += axis * 4;

            texturePalleteImage.transform.localScale = scale;

            var position = texturePalleteImage.transform.localPosition;

            position.x -= 265 * (axis * 2);
            position.y -= 265 * (axis * 2);

            texturePalleteImage.transform.localPosition = position;

        }

        if (Input.GetMouseButton(1)) {
            var position = texturePalleteImage.transform.localPosition;

            position.x += Input.GetAxis("Mouse X") * 18;
            position.y += Input.GetAxis("Mouse Y") * 18;

            texturePalleteImage.transform.localPosition = position;

        }

        //Vector2 pointOnPallete = Vector2.zero;

        //RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)texturePallete.transform, Input.mousePosition, Camera.main, out pointOnPallete);

        //Debug.Log(pointOnPallete);

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

            texture.LoadRawTextureData(controller.main.level.textures[graphics.number2].ConvertToRGB565());
            texture.Apply();

            texturePalleteImage.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero);
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

        texture.LoadRawTextureData(controller.main.level.textures[graphics.number2].ConvertToRGB565());
        texture.Apply();

        texturePalleteImage.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero);

        controller.main.RefreshTextures();

        controller.selectedSection.RefreshMesh();
        controller.selectedSection.RefreshTexture();

    }

    public void OnChangeTexturePalleteValue() {

        controller.ChangeTexturePallette(texturePalletteDropdown.GetComponent<TMP_Dropdown>().value);

        var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

        texturePalletteDropdown.GetComponent<TMP_Dropdown>().value = graphics.number2;

        var texture = new Texture2D(256, 256, TextureFormat.RGB565, false);

        texture.LoadRawTextureData(controller.main.level.textures[graphics.number2].ConvertToRGB565());
        texture.Apply();

        texturePalleteImage.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero);

    }

    public void OnClickAddTexture() {

        controller.selectedSection.section.textureCoordinates.Add(0);

        DestoryTextureOffsets();
        InitTextureOffsets();

    }

    public void OnClickAdd3Texture() {

        controller.selectedSection.section.textureCoordinates.Add(TextureCoordinate.SetPixel(120, 120));
        controller.selectedSection.section.textureCoordinates.Add(TextureCoordinate.SetPixel(120, 100));
        controller.selectedSection.section.textureCoordinates.Add(TextureCoordinate.SetPixel(100,100));


        DestoryTextureOffsets();
        InitTextureOffsets();

    }

    public void OnClickAdd4Texture() {

        controller.selectedSection.section.textureCoordinates.Add(TextureCoordinate.SetPixel(100, 120));
        controller.selectedSection.section.textureCoordinates.Add(TextureCoordinate.SetPixel(120, 120));
        controller.selectedSection.section.textureCoordinates.Add(TextureCoordinate.SetPixel(120, 100));
        controller.selectedSection.section.textureCoordinates.Add(TextureCoordinate.SetPixel(100, 100));


        DestoryTextureOffsets();
        InitTextureOffsets();

    }

    public void OnSetTextureCordX() {

        //var comp = textureInputX.GetComponent<TMP_InputField>();

        //try {
        //    controller.SetTextureCordX(Int32.Parse(comp.text));
        //} catch {
        //    return;
        //}

        DestoryTextureOffsets();
        InitTextureOffsets();
        textureLines.ReInit();

    }

    public void OnSetTextureCordY() {

        //var comp = textureInputY.GetComponent<TMP_InputField>();

        //try {
        //    controller.SetTextureCordY(Int32.Parse(comp.text));
        //} catch {
        //    return;
        //}

        DestoryTextureOffsets();
        InitTextureOffsets();
        textureLines.ReInit();
    }

    void InitView() {

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

            texture.LoadRawTextureData(controller.main.level.textures[graphics.number2].ConvertToRGB565());
            texture.Apply();

            texturePalleteImage.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), Vector2.zero);
        }

        InitTextureOffsets();

        InitGraphicsPreset();

    }

    public void InitTextureOffsets() {


        foreach (var index in Enumerable.Range(0, controller.selectedSection.section.textureCoordinates.Count)) {

            var item = Instantiate(textureOffsetItem);

            var script = item.GetComponent<TextureOffsetItem>();

            script.controller = controller;

            script.view = this;

            script.index = index;

            item.transform.SetParent(textureOffsets.transform);

        }

        foreach (var index in Enumerable.Range(0, textureCoordsClipboard.Count)) {

            var item = Instantiate(textureOffsetItem);

            var script = item.GetComponent<TextureOffsetItem>();

            script.controller = controller;

            script.view = this;

            script.index = index;

            script.isGlobal = true;

            item.transform.SetParent(textureOffsets.transform);

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

        foreach (Object obj in textureOffsets.transform) {
            Destroy(obj.GameObject());
        }

    }

    public void DestoryGraphicsPreset() {

        var content = graphicsPreset.transform.GetChild(0).GameObject().transform.GetChild(0);

        foreach (Object obj in content.transform) {
            Destroy(obj.GameObject());
        }

    }

    public void CloseWindow() {
        controller.selectedSection.RefreshMesh();
        Destroy(gameObject);
    }

    public void OnClickAddTileGraphics() {

        controller.selectedSection.section.tileGraphics.Add(new TileGraphics(116,1,0,1,0));

        DestoryGraphicsPreset();
        InitGraphicsPreset();

    }

    public void OnClickRotateClockwise() {

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        int index = 1;
        foreach (var point in textureLines.points) {

            var script = point.GetComponent<TextureCoordinatePoint>();

            if (index == oldPoints.Count) {

                var vector = oldPoints[0];

                script.ChangePosition((int)vector.x, (int)vector.y);

            } else {

                var vector = oldPoints[index];

                script.ChangePosition((int)vector.x, (int)vector.y);

            }

            index++;

        }

    }

    public void OnClickRotateCounterClockwise() {

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        int index = oldPoints.Count - 1;
        foreach (var point in textureLines.points) {

            var script = point.GetComponent<TextureCoordinatePoint>();

            if (index == oldPoints.Count - 1) {

                var vector = oldPoints.Last();

                script.ChangePosition((int)vector.x, (int)vector.y);

                index = -1;

            } else {

                var vector = oldPoints[index];

                script.ChangePosition((int)vector.x, (int)vector.y);

            }

            index++;

        }

    }

    public void OnClickCopyTextureCoords() {

        foreach (var point in textureLines.points) {
            controller.selectedSection.section.textureCoordinates.Add(
                TextureCoordinate.SetPixel((int)point.transform.localPosition.x, (int)point.transform.localPosition.y)
                );

        }

        DestoryTextureOffsets();
        InitTextureOffsets();

    }

    public void OnClickCopyTextureCoordsToClipboard() {

        foreach (var point in textureLines.points) {
            textureCoordsClipboard.Add(
                TextureCoordinate.SetPixel((int)point.transform.localPosition.x, (int)point.transform.localPosition.y)
                );

        }

        DestoryTextureOffsets();
        InitTextureOffsets();

    }

    public void OnClickClearClipboard() {

        textureCoordsClipboard.Clear();
        globalTextureCoordIndex = null;

        RefreshTextureOffsetsView();

    }

}