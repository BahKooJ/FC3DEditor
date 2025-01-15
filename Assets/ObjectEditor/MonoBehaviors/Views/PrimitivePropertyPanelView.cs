

using FCopParser;
using System;
using TMPro;
using UnityEngine;
using static FCopParser.FCopObject;

public class PrimitivePropertyPanelView : MonoBehaviour {

    // - Unity View Refs -
    public TMP_Text unknown1Text;
    public TMP_Text textureEnabledText;
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
        textureEnabledText.text = primitive.textureEnabled.ToString();
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

}