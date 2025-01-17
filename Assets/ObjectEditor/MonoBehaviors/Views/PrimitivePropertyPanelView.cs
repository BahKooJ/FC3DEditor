

using FCopParser;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FCopParser.FCopObject;

public class PrimitivePropertyPanelView : MonoBehaviour {

    // - Unity View Refs -
    public TMP_Text unknown1Text;
    public Toggle textureEnabledToggle;
    public TMP_Text unknown2Text;
    public TMP_Text isReflectiveText;
    public TMP_Text gouraudShadingText;
    public TMP_Text vertexColorModeText;
    public TMP_Text visabilityModeText;
    public TMP_Text vertexColorSemiTransText;
    public TMP_InputField materialIDInput;
    public TMP_InputField redInput;
    public TMP_InputField greenInput;
    public TMP_InputField blueInput;
    public TMP_InputField surfaceTypeInput;
    public TMP_Text uvValueText;

    // - Parameters -
    [HideInInspector]
    public ObjectEditorMain main;
    public FCopObject.Primitive primitive;

    FCopObject.Surface surface;

    private void Start() {
        
        Refresh();
    }

    public void Refresh() {

        surface = ObjectEditorMain.fCopObject.surfaceByCompiledOffset[primitive.surfaceIndex];

        unknown1Text.text = primitive.unknown1.ToString();
        textureEnabledToggle.isOn = primitive.textureEnabled;
        unknown2Text.text = primitive.unknown2.ToString();
        isReflectiveText.text = primitive.isReflective.ToString();
        gouraudShadingText.text = primitive.material.gouraudShading.ToString();
        vertexColorModeText.text = primitive.material.colorMode.ToString();
        visabilityModeText.text = primitive.material.visabilityMode.ToString();
        vertexColorSemiTransText.text = primitive.material.vertexColorSemiTransparent.ToString();
        materialIDInput.text = primitive.materialID.ToString();

        redInput.text = surface.red.ToString();
        greenInput.text = surface.green.ToString();
        blueInput.text = surface.blue.ToString();
        surfaceTypeInput.text = ((int)surface.type).ToString();
        uvValueText.text = (surface.uvMap != null).ToString();

    }

    public void OnEnterMaterialId() {

        primitive.materialID = Int32.Parse(materialIDInput.text);

    }

    public void OnFinishRed() {

        surface.red = Int32.Parse(redInput.text);

    }

    public void OnFinishGreen() {

        surface.green = Int32.Parse(greenInput.text);

    }

    public void OnFinishBlue() {

        surface.blue = Int32.Parse(blueInput.text);

    }

    public void OnFinishType() {

        surface.type = (SurfaceType)Int32.Parse(surfaceTypeInput.text);

    }

    public void RemoveUVs() {
        surface.uvMap = null;
    }

    public void OnToggleTextureEnabled() {
        primitive.textureEnabled = textureEnabledToggle.isOn;
    }

}