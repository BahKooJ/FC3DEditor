

using FCopParser;
using System;
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
    public GameObject overrideWhiteUI;

    public Toggle overrideWhiteToggle;
    public TMP_Text overrideWhiteText;
    public Image overrideWhiteImage;

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

    public VertexColorType colorType = VertexColorType.MonoChrome;

    // These are the values that are applied to tiles
    public byte solidMonoByteValue;
    public int dynamicMonoValue;
    public XRGB555 colorValue;

    public static int overrideWhiteDynamicMonoValue = (int)DynamicMonoChromeShader.white;
    public static XRGB555 overrideWhiteColorValue = new XRGB555(false, 31, 31, 31);

    public  static bool overrideWhite = false;

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
        solidMonoByteValue = (byte)MonoChromeShader.white;
        dynamicMonoValue = (int)DynamicMonoChromeShader.white;

        overrideWhiteToggle.isOn = overrideWhite;

        if (overrideWhite) {

            overrideWhiteText.color = Color.white;

            RefreshOverrideWhiteImage();

        }
        else {

            overrideWhiteText.color = Color.gray;
            overrideWhiteImage.color = Color.white;

        }

        RefreshView();

    }

    public void RefreshView() {

        refuseCallbacks = true;

        shaderTypeDropdown.value = (int)colorType;

        solidMonoUI.SetActive(false);
        monoUI.SetActive(false);
        colorUI.SetActive(false);

        colorValue ??= new XRGB555(false, 0, 0, 0);

        switch (colorType) {
            case VertexColorType.MonoChrome:
                solidMonoUI.SetActive(true);
                overrideWhiteUI.SetActive(false);
                break;
            case VertexColorType.DynamicMonoChrome:
                monoUI.SetActive(true);
                overrideWhiteUI.SetActive(true);
                break;
            case VertexColorType.Color:
                colorUI.SetActive(true);
                overrideWhiteUI.SetActive(true);
                break;
            case VertexColorType.ColorAnimated:
                overrideWhiteUI.SetActive(false);
                break;

        }

        RefreshColorPreview();
        RefreshOverrideWhiteImage();

        refuseCallbacks = false;

    }

    public void SelectedVertexColor(int index) {

        switch (controller.FirstTile.shaders.type) {
            case VertexColorType.MonoChrome:

                colorType = VertexColorType.MonoChrome;

                var solidMonoShader = (MonoChromeShader)controller.FirstTile.shaders;

                SetSolidMono(solidMonoShader.value);

                break;
            case VertexColorType.DynamicMonoChrome:

                colorType = VertexColorType.DynamicMonoChrome;


                var monoShader = (DynamicMonoChromeShader)controller.FirstTile.shaders;

                if (controller.IsFirstTileQuad) {

                    SetMono(monoShader.values[monoDataQuadIndexes[index]]);

                } else {

                    SetMono(monoShader.values[monoDataTrianglesIndexes[index]]);

                }

                break;
            case VertexColorType.Color:

                colorType = VertexColorType.Color;

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

        RefreshView();

    }

    public void SetColorFromPreset(ColorPreset preset) {

        switch (preset.type) {
            case VertexColorType.MonoChrome:

                colorType = VertexColorType.MonoChrome;

                SetSolidMono((byte)preset.monoValue);

                break;
            case VertexColorType.DynamicMonoChrome:

                colorType = VertexColorType.DynamicMonoChrome;
                
                SetMono(preset.monoValue);

                break;
            case VertexColorType.Color:

                colorType = VertexColorType.Color;
                
                SetColors(preset.colorValue);

                break;
            case VertexColorType.ColorAnimated:
                break;

        }

        RefreshView();

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

        switch (colorType) {
            case VertexColorType.MonoChrome:

                var SolidMonoColor = solidMonoByteValue / MonoChromeShader.white;

                if (SolidMonoColor > 1f) {
                    colorPreview.color = new Color(SolidMonoColor - 1f, 1f, SolidMonoColor - 1f);
                }
                else {
                    colorPreview.color = new Color(0f, SolidMonoColor, 0f);
                }

                break;
            case VertexColorType.DynamicMonoChrome:

                var monoColor = dynamicMonoValue / DynamicMonoChromeShader.white;

                if (monoColor > 1f) {
                    colorPreview.color = new Color(monoColor - 1f, 1f, monoColor - 1f);
                }
                else {
                    colorPreview.color = new Color(0f, monoColor, 0f);
                }


                break;
            case VertexColorType.Color:

                var colors = colorValue.ToColors();

                colorPreview.color = new Color(colors[0], colors[1], colors[2]);

                break;
            case VertexColorType.ColorAnimated:
                break;

        }



    }

    void RefreshOverrideWhiteImage() {

        if (colorType == VertexColorType.DynamicMonoChrome) {

            var monoColor = overrideWhiteDynamicMonoValue / DynamicMonoChromeShader.white;

            if (monoColor > 1f) {
                overrideWhiteImage.color = new Color(monoColor - 1f, 1f, monoColor - 1f);
            }
            else {
                overrideWhiteImage.color = new Color(0f, monoColor, 0f);
            }

        }
        else if (colorType == VertexColorType.Color) {

            var colors = overrideWhiteColorValue.ToColors();

            overrideWhiteImage.color = new Color(colors[0], colors[1], colors[2]);

        }

    }


    // Callbacks

    public void OnChangeShaderTypeDropdown() {

        if (refuseCallbacks) { return; }

        if (!ShaderEditMode.applyColorsOnClick && controller.HasSelection) {

            var tile = controller.FirstTile;

            tile.ChangeShader((VertexColorType)shaderTypeDropdown.value);

            controller.RefreshTileOverlayShader();

        }

        colorType = (VertexColorType)shaderTypeDropdown.value;

        controller.RefreshVertexColors();

        RefreshView();

    }

    public void OnChangeSolidMono() {

        if (refuseCallbacks) { return; }

        solidMonoByteValue = (byte)solidMonoSlider.value;
        solidMonoValue.text = ((int)solidMonoSlider.value).ToString();

        controller.ApplySolidMonoToTile();
        RefreshColorPreview();

    }

    public void OnFinishSolidMonoType() {

        try {

            var value = Int32.Parse(solidMonoValue.text);

            solidMonoByteValue = (byte)value;
            solidMonoSlider.value = value;
            controller.ApplySolidMonoToTile();

        }
        catch {

            OnChangeSolidMono();

        }

    }

    public void OnChangeMono() {

        if (refuseCallbacks) { return; }

        dynamicMonoValue = (int)monoSlider.value;
        monoValue.text = ((int)monoSlider.value).ToString();

        controller.ApplyColorsToVertexColorCorners();
        RefreshColorPreview();

    }

    public void OnFinishMonoType() {

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
            controller.ApplyColorsToVertexColorCorners();

        }
        catch {

            OnChangeMono();

        }

    }

    public void OnChangeRed() {

        if (refuseCallbacks) { return; }

        colorValue.r = (int)redSlider.value;

        redInput.text = ((int)redSlider.value).ToString();

        controller.ApplyColorsToVertexColorCorners();

        RefreshColorPreview();

    }

    public void OnFinishRedType() {

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
            controller.ApplyColorsToVertexColorCorners();

        }
        catch {

            OnChangeRed();

        }

    }

    public void OnChangeGreen() {

        if (refuseCallbacks) { return; }

        colorValue.g = (int)greenSlider.value;

        greenInput.text = ((int)greenSlider.value).ToString();

        controller.ApplyColorsToVertexColorCorners();

        RefreshColorPreview();

    }

    public void OnFinishGreenType() {

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

            controller.ApplyColorsToVertexColorCorners();

        }
        catch {

            OnChangeGreen();

        }

    }

    public void OnChangeBlue() {

        if (refuseCallbacks) { return; }

        colorValue.b = (int)blueSlider.value;

        blueInput.text = ((int)blueSlider.value).ToString();

        controller.ApplyColorsToVertexColorCorners();

        RefreshColorPreview();

    }

    public void OnFinishBlueType() {

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
            controller.ApplyColorsToVertexColorCorners();

        }
        catch {

            OnChangeBlue();

        }

    }

    public void OnToggleOverrideWhite() {

        overrideWhite = overrideWhiteToggle.isOn;

        if (overrideWhite) {

            overrideWhiteText.color = Color.white;

            RefreshOverrideWhiteImage();

        }
        else {

            overrideWhiteText.color = Color.gray;
            overrideWhiteImage.color = Color.white;

        }

    }

    public void OnClickOverrideWhiteColor() {

        if (!overrideWhite) {
            return;
        }

        if (colorType == VertexColorType.DynamicMonoChrome) {
            overrideWhiteDynamicMonoValue = dynamicMonoValue;
            RefreshOverrideWhiteImage();
        }
        else if (colorType == VertexColorType.Color) {
            overrideWhiteColorValue = colorValue.Clone();
            RefreshOverrideWhiteImage();
        }

    }

}