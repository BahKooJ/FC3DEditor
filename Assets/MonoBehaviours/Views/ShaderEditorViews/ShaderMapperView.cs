

using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShaderMapperView : MonoBehaviour {

    public static int[] colorDataTrianglesIndexes = new int[] { 2, 0, 1 };
    public static int[] colorDataQuadIndexes = new int[] { 3, 1, 2, 0 };
    public static int[] monoDataTrianglesIndexes = new int[] { 0, 2, 1 };
    public static int[] monoDataQuadIndexes = new int[] { 0, 1, 3, 2 };

    const float meshSize = 230;

    public ShaderEditMode controller;

    // View Refs
    public GameObject previewMesh;
    public TMP_Dropdown shaderTypeDropdown;

    public GameObject solidMonoUI;
    public GameObject monoUI;
    public GameObject colorUI;

    // -Solid Monochrome-

    // View Refs
    public Slider solidMonoSlider;
    public TMP_InputField solidMonoValue;

    // -Dynamic Monochrome-

    // View Refs
    public Slider monoSlider;
    public TMP_InputField monoValue;

    // -Colors-

    // View Refs
    public Image colorPreview;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public TMP_InputField redInput;
    public TMP_InputField greenInput;
    public TMP_InputField blueInput;
    public Transform colorPresetsList;

    // Prefabs
    public GameObject vertexColor;
    public GameObject colorPresetItem;

    public List<ColorPresetItemView> colorPresetItemViews = new();

    public byte solidMonoByteValue;
    public int dynamicMonoValue;
    public XRGB555 colorValue;

    List<Vector3> vertices = new();
    // These are the objects that actually set values to the tiles
    public List<VertexCornerShaderView> corners = new();

    bool refuseCallbacks = false;

    Mesh mesh;
    Material material;

    void ScaleToScreen() {

        var screenWidth = Screen.width * 0.40f;
        var screenHeight = Screen.height * 0.80f;

        var multiplierWidth = screenWidth / ((RectTransform)transform).rect.width;

        var multiplierHeight = screenHeight / ((RectTransform)transform).rect.height;

        var multiplier = new float[] { multiplierWidth, multiplierHeight }.Min();

        transform.localScale = new Vector3(multiplier, multiplier, multiplier);

    }

    void Start() {

        colorValue = new XRGB555(false, 0, 0, 0);

        ScaleToScreen();

        mesh = new Mesh();
        previewMesh.GetComponent<MeshFilter>().mesh = mesh;
        material = previewMesh.GetComponent<MeshRenderer>().material;

        material.mainTexture = controller.main.levelTexturePallet;

        if (controller.selectedItems.Count != 0) {
            RefreshView();
        }

    }

    public void RefreshView() {

        refuseCallbacks = true;

        foreach (var corner in corners) {
            Destroy(corner.gameObject);
        }

        foreach (var obj in colorPresetItemViews) {
            Destroy(obj.gameObject);
        }

        colorPresetItemViews.Clear();

        corners.Clear();

        var tile = controller.FirstTile;

        shaderTypeDropdown.value = (int)tile.shaders.type;

        solidMonoUI.SetActive(false);
        monoUI.SetActive(false);
        colorUI.SetActive(false);

        switch (tile.shaders.type) {
            case VertexColorType.MonoChrome:
                solidMonoUI.SetActive(true);
                SetSolidMono(((MonoChromeShader)tile.shaders).value);
                break;
            case VertexColorType.DynamicMonoChrome:
                monoUI.SetActive(true);
                break;
            case VertexColorType.Color:
                colorUI.SetActive(true);
                InitColorView();
                break;
            case VertexColorType.ColorAnimated:
                break;

        }

        RefreshMesh();

        CreateCorners(vertices);

        refuseCallbacks = false;

    }

    void InitColorView() {

        foreach (var preset in Presets.colorPresets.presets) {

            var obj = Instantiate(colorPresetItem);

            var script = obj.GetComponent<ColorPresetItemView>();
            script.view = this;
            script.color = preset;

            obj.transform.SetParent(colorPresetsList, false);

            obj.SetActive(true);

            colorPresetItemViews.Add(script);

        }

    }

    void AddColorPreset() {

        var obj = Instantiate(colorPresetItem);

        var script = obj.GetComponent<ColorPresetItemView>();
        script.view = this;
        script.color = Presets.colorPresets.presets.Last();

        obj.transform.SetParent(colorPresetsList, false);

        obj.SetActive(true);

        colorPresetItemViews.Add(script);

    }

    public void SelectedCorner(int index) {

        switch (controller.FirstTile.shaders.type) {
            case VertexColorType.MonoChrome:
                break;
            case VertexColorType.DynamicMonoChrome:

                var monoShader = (DynamicMonoChromeShader)controller.FirstTile.shaders;

                if (controller.IsFirstTileQuad) {

                    SetMono(monoShader.values[monoDataQuadIndexes[index]]);

                } else {

                    SetMono(monoShader.values[monoDataTrianglesIndexes[index]]);

                }

                break;
            case VertexColorType.Color:

                var shader = (ColorShader)controller.FirstTile.shaders;

                if (controller.IsFirstTileQuad) {

                    SetColors(shader.values[colorDataQuadIndexes[index]]);

                } else {

                    SetColors(shader.values[colorDataTrianglesIndexes[index]]);

                }

                break;
            case VertexColorType.ColorAnimated:
                break;

        }

    }

    public void SetColors(XRGB555 rgb) {

        refuseCallbacks = true;

        redSlider.value = rgb.r;
        redInput.text = rgb.r.ToString();
        greenSlider.value = rgb.g;
        greenInput.text = rgb.g.ToString();
        blueSlider.value = rgb.b;
        blueInput.text = rgb.b.ToString();

        colorValue = rgb.Clone();

        RefreshColorPreview();

        refuseCallbacks = false;

    }

    void SetSolidMono(byte value) {

        refuseCallbacks = true;

        solidMonoSlider.value = value;
        solidMonoValue.text = value.ToString();
        solidMonoByteValue = value;

        refuseCallbacks = false;

    }

    void SetMono(int value) { 
        
        refuseCallbacks = true;

        monoSlider.value = value;
        monoValue.text = value.ToString();
        dynamicMonoValue = value;

        refuseCallbacks = false;

    }

    public void ApplyColorsToCorner() {

        if (corners.Count == 0) {
            return;
        }

        corners[0].ChangeValue();

        RefreshMesh();

    }

    public void ApplyColorsToCorners() {

        foreach (var corner in corners) {

            if (corner.isSelected) {
                corner.ChangeValue();
            }

        }

        RefreshMesh();

    }

    VertexCornerShaderView CreateCorner(int index) {

        var view = Instantiate(vertexColor);

        view.transform.SetParent(previewMesh.transform, false);

        var script = view.GetComponent<VertexCornerShaderView>();
        script.controller = controller;
        script.view = this;
        script.index = index;

        view.SetActive(true);

        return script;

    }

    void CreateCorners(List<Vector3> vertices) {

        var i = 0;
        foreach (var v in vertices) {

            var corner = CreateCorner(i);

            ((RectTransform)corner.transform).localPosition = new Vector3(v.x, v.y, -1);

            corner.Init();

            corners.Add(corner);

            i++;
        }

    }

    void RefreshMesh() {

        mesh.Clear();
        vertices.Clear();

        if (controller.IsFirstTileQuad) {
            GenerateQuad();
        }
        else {
            GenerateTriangle();
        }

    }

    void RefreshColorPreview() {

        var colors = colorValue.ToColors();

        colorPreview.color = new Color(colors[0], colors[1], colors[2]);

    }

    void GenerateQuad() {

        var tile = controller.FirstTile;

        List<Color> vertexColors = new();
        List<Vector2> textureCords = new();

        vertices = MeshUtils.FlatScreenVerticies(tile.verticies, meshSize);
        //vertices = new List<Vector3>() { new Vector3(0, 0), new Vector3(meshSize, 0), new Vector3(0, -meshSize), new Vector3(meshSize, -meshSize) };

        textureCords.Add(new Vector2(
            TextureCoordinate.GetX(tile.uvs[0] + tile.texturePalette * 65536),
            TextureCoordinate.GetY(tile.uvs[0] + tile.texturePalette * 65536)
        ));

        textureCords.Add(new Vector2(
            TextureCoordinate.GetX(tile.uvs[1] + tile.texturePalette * 65536),
            TextureCoordinate.GetY(tile.uvs[1] + tile.texturePalette * 65536)
        ));

        textureCords.Add(new Vector2(
            TextureCoordinate.GetX(tile.uvs[3] + tile.texturePalette * 65536),
            TextureCoordinate.GetY(tile.uvs[3] + tile.texturePalette * 65536)
        ));

        textureCords.Add(new Vector2(
            TextureCoordinate.GetX(tile.uvs[2] + tile.texturePalette * 65536),
            TextureCoordinate.GetY(tile.uvs[2] + tile.texturePalette * 65536)
        ));

        var triangles = new List<int> {
            0,
            2,
            1,

            1,
            2,
            3
        };

        vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
        vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
        vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));
        vertexColors.Add(new Color(tile.shaders.colors[3][0], tile.shaders.colors[3][1], tile.shaders.colors[3][2]));

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = textureCords.ToArray();
        mesh.colors = vertexColors.ToArray();

        mesh.RecalculateNormals();

    }

    void GenerateTriangle() {

        var tile = controller.FirstTile;

        List<Color> vertexColors = new();
        List<Vector2> textureCords = new();

        vertices = MeshUtils.FlatScreenVerticies(tile.verticies, meshSize);

        textureCords.Add(new Vector2(
            TextureCoordinate.GetX(tile.uvs[0] + tile.texturePalette * 65536),
            TextureCoordinate.GetY(tile.uvs[0] + tile.texturePalette * 65536)
        ));

        textureCords.Add(new Vector2(
            TextureCoordinate.GetX(tile.uvs[2] + tile.texturePalette * 65536),
            TextureCoordinate.GetY(tile.uvs[2] + tile.texturePalette * 65536)
        ));

        textureCords.Add(new Vector2(
            TextureCoordinate.GetX(tile.uvs[1] + tile.texturePalette * 65536),
            TextureCoordinate.GetY(tile.uvs[1] + tile.texturePalette * 65536)
        ));

        var triangles = new List<int> {
            0,
            1,
            2
        };

        vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
        vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
        vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = textureCords.ToArray();
        mesh.colors = vertexColors.ToArray();

        mesh.RecalculateNormals();

    }

    // Callbacks

    public void OnChangeShaderTypeDropdown() {

        if (refuseCallbacks) { return; }

        var tile = controller.FirstTile;

        tile.ChangeShader((VertexColorType)shaderTypeDropdown.value);

        RefreshView();
        controller.RefreshTileOverlayShader();

    }

    public void OnChangeSolidMono() {

        if (refuseCallbacks) { return; }

        solidMonoByteValue = (byte)solidMonoSlider.value;
        solidMonoValue.text = ((int)solidMonoSlider.value).ToString();

        ApplyColorsToCorner();
        controller.RefreshTileOverlayShader();


    }

    public void OnFinishSolidMonoType() {

        if (controller.HasSelection) {
            return;
        }

        try {

            var value = Int32.Parse(solidMonoValue.text);

            solidMonoByteValue = (byte)value;
            solidMonoSlider.value = value;

            ApplyColorsToCorner();
            controller.RefreshTileOverlayShader();

        } catch {

            OnChangeSolidMono();

        }

    }

    public void OnChangeMono() {

        if (refuseCallbacks) { return; }

        dynamicMonoValue = (int)monoSlider.value;
        monoValue.text = ((int)monoSlider.value).ToString();

        ApplyColorsToCorners();
        controller.RefreshTileOverlayShader();


    }

    public void OnFinishMonoType() {

        if (controller.HasSelection) {
            return;
        }

        try {

            var value = Int32.Parse(monoValue.text);

            if (value < 0) {
                value = 0;
            }
            if (value > 63) {
                value = 63;
            }

            dynamicMonoValue = value;
            monoSlider.value = value;

            ApplyColorsToCorner();
            controller.RefreshTileOverlayShader();

        }
        catch {

            OnChangeMono();

        }

    }

    public void OnChangeRed() {

        if (refuseCallbacks) { return; }

        colorValue.r = (int)redSlider.value;

        redInput.text = ((int)redSlider.value).ToString();

        RefreshColorPreview();
        ApplyColorsToCorners();
        controller.RefreshTileOverlayShader();

    }

    public void OnFinishRedType() {

        if (controller.HasSelection) {
            return;
        }

        try {

            var value = Int32.Parse(redInput.text);

            if (value < 0) {
                value = 0;
            }
            if (value > 31) {
                value = 31;
            }

            colorValue.r = value;
            redSlider.value = value;

            ApplyColorsToCorner();
            controller.RefreshTileOverlayShader();

        }
        catch {

            OnChangeRed();

        }

    }

    public void OnChangeGreen() {

        if (refuseCallbacks) { return; }

        colorValue.g = (int)greenSlider.value;

        greenInput.text = ((int)greenSlider.value).ToString();

        RefreshColorPreview();
        ApplyColorsToCorners();
        controller.RefreshTileOverlayShader();

    }

    public void OnFinishGreenType() {

        if (controller.HasSelection) {
            return;
        }

        try {

            var value = Int32.Parse(greenInput.text);

            if (value < 0) {
                value = 0;
            }
            if (value > 31) {
                value = 31;
            }

            colorValue.g = value;
            greenSlider.value = value;

            ApplyColorsToCorner();
            controller.RefreshTileOverlayShader();

        }
        catch {

            OnChangeGreen();

        }

    }

    public void OnChangeBlue() {

        if (refuseCallbacks) { return; }

        colorValue.b = (int)blueSlider.value;

        blueInput.text = ((int)blueSlider.value).ToString();

        RefreshColorPreview();
        ApplyColorsToCorners();
        controller.RefreshTileOverlayShader();

    }

    public void OnFinishBlueType() {

        if (controller.HasSelection) {
            return;
        }

        try {

            var value = Int32.Parse(blueInput.text);

            if (value < 0) {
                value = 0;
            }
            if (value > 31) {
                value = 31;
            }

            colorValue.b = value;
            blueSlider.value = value;

            ApplyColorsToCorner();
            controller.RefreshTileOverlayShader();

        }
        catch {

            OnChangeBlue();

        }

    }

    public void OnClickAddColorPresetButton() {

        Presets.colorPresets.presets.Add(colorValue.Clone());

        AddColorPreset();

    }

    public void OnClickClearColorPresetsButton() {
        Presets.colorPresets.presets.Clear();

        RefreshView();

    }

    public void OnClickRotateClockwise() {

        var tile = controller.FirstTile;

        if (tile.shaders.type == VertexColorType.MonoChrome || tile.shaders.type == VertexColorType.ColorAnimated) {
            return;
        }

        if (tile.shaders.type == VertexColorType.DynamicMonoChrome) {

            var mono = (DynamicMonoChromeShader)tile.shaders;

            if (mono.isQuad) {

                var cornerValues = new List<int> {
                    corners[2].GetMonochromeValue(),
                    corners[0].GetMonochromeValue(),
                    corners[3].GetMonochromeValue(),
                    corners[1].GetMonochromeValue()
                };

                corners[0].ChangeMonoValue(cornerValues[0]);
                corners[1].ChangeMonoValue(cornerValues[1]);
                corners[2].ChangeMonoValue(cornerValues[2]);
                corners[3].ChangeMonoValue(cornerValues[3]);

            }
            else {

                var cornerValues = new List<int> {
                    corners[1].GetMonochromeValue(),
                    corners[2].GetMonochromeValue(),
                    corners[0].GetMonochromeValue()
                };

                corners[0].ChangeMonoValue(cornerValues[0]);
                corners[1].ChangeMonoValue(cornerValues[1]);
                corners[2].ChangeMonoValue(cornerValues[2]);

            }

        }
        else if (tile.shaders.type == VertexColorType.Color) {

            var color = (ColorShader)tile.shaders;

            if (color.isQuad) {

                var cornerValues = new List<XRGB555> {
                    corners[2].GetColorValue(),
                    corners[0].GetColorValue(),
                    corners[3].GetColorValue(),
                    corners[1].GetColorValue()
                };

                corners[0].ChangeColor(cornerValues[0]);
                corners[1].ChangeColor(cornerValues[1]);
                corners[2].ChangeColor(cornerValues[2]);
                corners[3].ChangeColor(cornerValues[3]);

            }
            else {

                var cornerValues = new List<XRGB555> {
                    corners[1].GetColorValue(),
                    corners[2].GetColorValue(),
                    corners[0].GetColorValue()
                };

                corners[0].ChangeColor(cornerValues[0]);
                corners[1].ChangeColor(cornerValues[1]);
                corners[2].ChangeColor(cornerValues[2]);

            }

        }

        controller.RefreshTileOverlayShader();
        RefreshView();

    }

    public void OnClickRotateCounterClockwise() {

        var tile = controller.FirstTile;

        if (tile.shaders.type == VertexColorType.MonoChrome || tile.shaders.type == VertexColorType.ColorAnimated) {
            return;
        }

        if (tile.shaders.type == VertexColorType.DynamicMonoChrome) {

            var mono = (DynamicMonoChromeShader)tile.shaders;

            if (mono.isQuad) {

                var cornerValues = new List<int> {
                    corners[1].GetMonochromeValue(),
                    corners[3].GetMonochromeValue(),
                    corners[0].GetMonochromeValue(),
                    corners[2].GetMonochromeValue()
                };

                corners[0].ChangeMonoValue(cornerValues[0]);
                corners[1].ChangeMonoValue(cornerValues[1]);
                corners[2].ChangeMonoValue(cornerValues[2]);
                corners[3].ChangeMonoValue(cornerValues[3]);

            }
            else {

                var cornerValues = new List<int> {
                    corners[2].GetMonochromeValue(),
                    corners[0].GetMonochromeValue(),
                    corners[1].GetMonochromeValue()
                };

                corners[0].ChangeMonoValue(cornerValues[0]);
                corners[1].ChangeMonoValue(cornerValues[1]);
                corners[2].ChangeMonoValue(cornerValues[2]);

            }

        }
        else if (tile.shaders.type == VertexColorType.Color) {

            var color = (ColorShader)tile.shaders;

            if (color.isQuad) {

                var cornerValues = new List<XRGB555> {
                    corners[1].GetColorValue(),
                    corners[3].GetColorValue(),
                    corners[0].GetColorValue(),
                    corners[2].GetColorValue()
                };

                corners[0].ChangeColor(cornerValues[0]);
                corners[1].ChangeColor(cornerValues[1]);
                corners[2].ChangeColor(cornerValues[2]);
                corners[3].ChangeColor(cornerValues[3]);

            }
            else {

                var cornerValues = new List<XRGB555> {
                    corners[2].GetColorValue(),
                    corners[0].GetColorValue(),
                    corners[1].GetColorValue()
                };

                corners[0].ChangeColor(cornerValues[0]);
                corners[1].ChangeColor(cornerValues[1]);
                corners[2].ChangeColor(cornerValues[2]);

            }

        }

        controller.RefreshTileOverlayShader();
        RefreshView();

    }

    public void OnClickFlipVertically() {

        var tile = controller.FirstTile;

        if (tile.shaders.type == VertexColorType.MonoChrome || tile.shaders.type == VertexColorType.ColorAnimated) {
            return;
        }

        if (tile.shaders.type == VertexColorType.DynamicMonoChrome) {

            var mono = (DynamicMonoChromeShader)tile.shaders;

            if (mono.isQuad) {

                var cornerValues = new List<int> {
                    corners[2].GetMonochromeValue(),
                    corners[3].GetMonochromeValue(),
                    corners[0].GetMonochromeValue(),
                    corners[1].GetMonochromeValue()
                };

                corners[0].ChangeMonoValue(cornerValues[0]);
                corners[1].ChangeMonoValue(cornerValues[1]);
                corners[2].ChangeMonoValue(cornerValues[2]);
                corners[3].ChangeMonoValue(cornerValues[3]);

            }


        }
        else if (tile.shaders.type == VertexColorType.Color) {

            var color = (ColorShader)tile.shaders;

            if (color.isQuad) {

                var cornerValues = new List<XRGB555> {
                    corners[2].GetColorValue(),
                    corners[3].GetColorValue(),
                    corners[0].GetColorValue(),
                    corners[1].GetColorValue()
                };

                corners[0].ChangeColor(cornerValues[0]);
                corners[1].ChangeColor(cornerValues[1]);
                corners[2].ChangeColor(cornerValues[2]);
                corners[3].ChangeColor(cornerValues[3]);

            }

        }

        controller.RefreshTileOverlayShader();
        RefreshView();

    }

    public void OnClickFlipHorizontally() {

        var tile = controller.FirstTile;

        if (tile.shaders.type == VertexColorType.MonoChrome || tile.shaders.type == VertexColorType.ColorAnimated) {
            return;
        }

        if (tile.shaders.type == VertexColorType.DynamicMonoChrome) {

            var mono = (DynamicMonoChromeShader)tile.shaders;

            if (mono.isQuad) {

                var cornerValues = new List<int> {
                    corners[1].GetMonochromeValue(),
                    corners[0].GetMonochromeValue(),
                    corners[3].GetMonochromeValue(),
                    corners[2].GetMonochromeValue()
                };

                corners[0].ChangeMonoValue(cornerValues[0]);
                corners[1].ChangeMonoValue(cornerValues[1]);
                corners[2].ChangeMonoValue(cornerValues[2]);
                corners[3].ChangeMonoValue(cornerValues[3]);

            }


        }
        else if (tile.shaders.type == VertexColorType.Color) {

            var color = (ColorShader)tile.shaders;

            if (color.isQuad) {

                var cornerValues = new List<XRGB555> {
                    corners[1].GetColorValue(),
                    corners[0].GetColorValue(),
                    corners[3].GetColorValue(),
                    corners[2].GetColorValue()
                };

                corners[0].ChangeColor(cornerValues[0]);
                corners[1].ChangeColor(cornerValues[1]);
                corners[2].ChangeColor(cornerValues[2]);
                corners[3].ChangeColor(cornerValues[3]);

            }

        }

        controller.RefreshTileOverlayShader();
        RefreshView();

    }

}