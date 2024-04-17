

using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShaderColorPickerView : MonoBehaviour {

    public static int[] colorDataTrianglesIndexes = new int[] { 2, 0, 1 };
    public static int[] colorDataQuadIndexes = new int[] { 3, 1, 2, 0 };
    public static int[] monoDataTrianglesIndexes = new int[] { 0, 2, 1 };
    public static int[] monoDataQuadIndexes = new int[] { 0, 1, 3, 2 };

    public ShaderEditMode controller;

    // View Refs
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

    public VertexColorType colorType = VertexColorType.MonoChrome;

    public byte solidMonoByteValue;
    public int dynamicMonoValue;
    public XRGB555 colorValue;

    bool refuseCallbacks = false;

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

        if (controller.selectedItems.Count != 0) {
            RefreshView();
        }

    }

    public void RefreshView() {

        refuseCallbacks = true;

        foreach (var obj in colorPresetItemViews) {
            Destroy(obj.gameObject);
        }

        colorPresetItemViews.Clear();

        solidMonoUI.SetActive(false);
        monoUI.SetActive(false);
        colorUI.SetActive(false);

        switch (colorType) {
            case VertexColorType.MonoChrome:
                solidMonoUI.SetActive(true);
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

    public void SelectedVertexColor(int index) {

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


    void RefreshColorPreview() {

        var colors = colorValue.ToColors();

        colorPreview.color = new Color(colors[0], colors[1], colors[2]);

    }


    // Callbacks

    public void OnChangeShaderTypeDropdown() {

        if (refuseCallbacks) { return; }

        colorType = (VertexColorType)shaderTypeDropdown.value;

        RefreshView();

    }

    public void OnChangeSolidMono() {

        if (refuseCallbacks) { return; }

        solidMonoByteValue = (byte)solidMonoSlider.value;
        solidMonoValue.text = ((int)solidMonoSlider.value).ToString();

    }

    public void OnFinishSolidMonoType() {

        if (!controller.HasSelection) {
            return;
        }

        try {

            var value = Int32.Parse(solidMonoValue.text);

            solidMonoByteValue = (byte)value;
            solidMonoSlider.value = value;

        } catch {

            OnChangeSolidMono();

        }

    }

    public void OnChangeMono() {

        if (refuseCallbacks) { return; }

        dynamicMonoValue = (int)monoSlider.value;
        monoValue.text = ((int)monoSlider.value).ToString();


    }

    public void OnFinishMonoType() {

        if (!controller.HasSelection) {
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

    }

    public void OnFinishRedType() {

        if (!controller.HasSelection) {
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

    }

    public void OnFinishGreenType() {

        if (!controller.HasSelection) {
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

    }

    public void OnFinishBlueType() {

        if (!controller.HasSelection) {
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

}