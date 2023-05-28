
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
using UnityEditor;

public class GraphicsPropertiesView : MonoBehaviour {

    public static List<int> textureCoordsClipboard = new();

    public GeometryEditMode controller;

    // --Prefabs--
    public GameObject textureOffsetItem;
    public GameObject graphicsPresetItem;
    public GameObject textureCoordinatePoint;

    // --View References--
    public GameObject texturePaletteDropdown;
    public Toggle rectangleTileToggle;
    public GameObject texturePalette;
    public GameObject texturePaletteImage;
    public GameObject textureOffsets;
    public ScrollRect textureScrollView;
    public GameObject graphicsPreset;
    public TextureCoordinatesLines textureLines;
    public GameObject textureTilePreview;

    public TileTexturePreview tilePreview;

    public int bmpID;
    public int? globalTextureCoordIndex = null;

    void ScaleToScreen() {

        var screenWidth = Screen.width - 32;
        var screenHeight = Screen.height - 32;

        var multiplierWidth = screenWidth / ((RectTransform)transform).rect.width;

        var multiplierHeight = screenHeight / ((RectTransform)transform).rect.height;

        var multiplier = new float[] { multiplierWidth, multiplierHeight }.Min();

        transform.localScale = new Vector3(multiplier, multiplier, multiplier);

    }


    void Start() {

        InitView();
        InitTilePreview();

        ScaleToScreen();

    }

    void Update() {

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0) {

            if (IsCursorInTexturePallete()) {

                var scale = texturePaletteImage.transform.localScale;

                scale.x += axis * 4;
                scale.y += axis * 4;

                texturePaletteImage.transform.localScale = scale;

                var position = texturePaletteImage.transform.localPosition;

                position.x -= 265 * (axis * 2);
                position.y -= 265 * (axis * 2);

                texturePaletteImage.transform.localPosition = position;

            }
            else if (IsCursorInTilePreview()) {

                var scale = tilePreview.transform.localScale;

                scale.x -= axis * 32;
                scale.y += axis * 32;
                scale.z += axis * 32;

                tilePreview.transform.localScale = scale;

            }


        }

        if (Controls.IsDown("DragPalette")) {

            if (IsCursorInTexturePallete()) {

                var position = texturePaletteImage.transform.localPosition;

                position.x += Input.GetAxis("Mouse X") * 18;
                position.y += Input.GetAxis("Mouse Y") * 18;

                texturePaletteImage.transform.localPosition = position;

            }
            else if (IsCursorInTilePreview()) {

                var rotateY = Input.GetAxis("Mouse X") * 800 * Mathf.Deg2Rad;
                var rotateZ = Input.GetAxis("Mouse Y") * 400 * Mathf.Deg2Rad;

                //TODO: Only rotates correctly from left to right
                tilePreview.transform.Rotate(Vector3.left, rotateZ);

                var rotation = tilePreview.transform.localEulerAngles;

                rotation.y += rotateY;

                tilePreview.transform.localEulerAngles = rotation;

            }

        }

    }

    bool IsCursorInTexturePallete() {

        Vector2 pointOnPallete = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)texturePalette.transform, Input.mousePosition, Camera.main, out pointOnPallete);

        var palleteRect = (RectTransform)texturePalette.transform;

        return pointOnPallete.x < palleteRect.rect.width && pointOnPallete.y < palleteRect.rect.height;

    }

    bool IsCursorInTilePreview() {

        Vector2 pointOnPallete = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)textureTilePreview.transform, Input.mousePosition, Camera.main, out pointOnPallete);

        var rect = (RectTransform)textureTilePreview.transform;

        return pointOnPallete.x < rect.rect.width && pointOnPallete.y < rect.rect.height;

    }

    void InitTexturePallette() {

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

            texturePaletteDropdown.GetComponent<TMP_Dropdown>().value = graphics.number2;

            texturePaletteImage.GetComponent<Image>().sprite = controller.main.bmpTextures[graphics.number2];

        }

    }

    void InitView() {

        if (controller.selectedTiles.Count > 0) {
            var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

            rectangleTileToggle.GetComponent<Toggle>().isOn = graphics.number4 == 1;
        }

        InitTexturePallette();

        InitTextureOffsets();

        InitGraphicsPreset();

    }

    public void RefreshView() {

        DestoryTextureOffsets();
        DestoryGraphicsPreset();

        if (controller.selectedTiles.Count > 0) { 
            var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

            rectangleTileToggle.GetComponent<Toggle>().isOn = graphics.number4 == 1;
        }

        InitTexturePallette();

        InitTextureOffsets();

        InitGraphicsPreset();

        tilePreview.Refresh();

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

        //var itemCount = textureOffsets.transform.childCount;
        //var scrollDistance = (controller.selectedTiles[0].textureIndex) / (itemCount);

        //textureScrollView.verticalNormalizedPosition = 1 - scrollDistance;

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

    public void RefreshTextureOffsetsView() {
        DestoryTextureOffsets();
        InitTextureOffsets();
    }

    public void ChangeTextureIndex(int index, bool isGlobal) {

        var verticieCount = controller.selectedTiles[0].verticies.Count;
        if (controller.selectedSection.section.textureCoordinates.Count - index < verticieCount) {
            DialogWindowUtil.Dialog("Invalid Texture Count", verticieCount + " or more texture coordinates need to be available");
            return;
        }

        if (isGlobal) {
            globalTextureCoordIndex = index;
        } else {
            globalTextureCoordIndex = null;
            controller.SetTextureIndex(index);
        }

        textureLines.ReInit();
        tilePreview.Refresh();
        RefreshTextureOffsetsView();

    }

    void InitTilePreview() {

        var overlay = Instantiate(controller.main.TileTexturePreview);
        var script = overlay.GetComponent<TileTexturePreview>();
        script.controller = controller.main;
        script.tile = controller.selectedTiles[0];
        script.column = controller.selectedColumn;
        script.section = controller.selectedSection.section;
        overlay.transform.SetParent(textureTilePreview.transform, false);

        tilePreview = script;

    }

    // --Event Handlers--

    public void CloseWindow() {
        controller.selectedSection.RefreshMesh();
        Destroy(gameObject);
    }

    public void OnClickExportTexture() {

        controller.ExportTexture(this.bmpID);

    }

    public void OnClickImportTexture() {

        controller.ImportTexture(bmpID);

        var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

        texturePaletteDropdown.GetComponent<TMP_Dropdown>().value = graphics.number2;

        rectangleTileToggle.GetComponent<Toggle>().isOn = graphics.number4 == 1;

        controller.main.RefreshTextures();

        InitTexturePallette();

        controller.selectedSection.RefreshMesh();
        controller.selectedSection.RefreshTexture();

    }

    public void OnChangeTexturePalleteValue() {

        controller.ChangeTexturePallette(texturePaletteDropdown.GetComponent<TMP_Dropdown>().value);

        var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

        texturePaletteDropdown.GetComponent<TMP_Dropdown>().value = graphics.number2;

        InitTexturePallette();

    }

    public void OnClickAddTexture() {

        controller.selectedSection.section.textureCoordinates.Add(0);

        DestoryTextureOffsets();
        InitTextureOffsets();

    }

    public void OnClickAdd3Texture() {

        controller.selectedSection.section.textureCoordinates.Add(TextureCoordinate.SetPixel(120, 120));
        controller.selectedSection.section.textureCoordinates.Add(TextureCoordinate.SetPixel(120, 100));
        controller.selectedSection.section.textureCoordinates.Add(TextureCoordinate.SetPixel(100, 100));


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

    public void OnFlipTextureCoordsVertically() {

        var minX = textureLines.points.Min(obj => {
            return obj.transform.localPosition.x;
        });

        var maxX = textureLines.points.Max(obj => {
            return obj.transform.localPosition.x;
        });

        var width = maxX - minX;
        var center = width / 2;

        foreach (var point in textureLines.points) {

            var localX = point.transform.localPosition.x - minX;

            var distanceFromCenter = localX - center;

            var vFlippedX = center - distanceFromCenter;

            var script = point.GetComponent<TextureCoordinatePoint>();

            script.ChangePosition((int)(minX + vFlippedX), (int)point.transform.localPosition.y);

        }

    }

    public void OnFlipTextureCoordsHorizontally() {

        var minY = textureLines.points.Min(obj => {
            return obj.transform.localPosition.y;
        });

        var maxY = textureLines.points.Max(obj => {
            return obj.transform.localPosition.y;
        });

        var height = maxY - minY;
        var center = height / 2;

        foreach (var point in textureLines.points) {

            var localY = point.transform.localPosition.y - minY;

            var distanceFromCenter = localY - center;

            var vFlippedY = center - distanceFromCenter;

            var script = point.GetComponent<TextureCoordinatePoint>();

            script.ChangePosition((int)point.transform.localPosition.x, (int)(minY + vFlippedY));

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

    public void OnValueChangedRectTile() {

        if (controller == null) {
            return;
        }

        if (controller.selectedTiles.Count == 0) {
            return;
        }

        var graphics = controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex];

        graphics.number4 = rectangleTileToggle.isOn ? 1 : 0;

        controller.selectedSection.section.tileGraphics[controller.selectedTiles[0].graphicsIndex] = graphics;

    }

}