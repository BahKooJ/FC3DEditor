

using FCopParser;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static FCopParser.FCopObject;

public class PrimitivePropertyPanelView : MonoBehaviour {

    // - Unity View Refs -
    public TMP_Text unknown1Text;
    public TMP_Text textureEnabledText;
    public TMP_Text unknown2Text;
    public TMP_Text isReflectiveText;
    public TMP_Text shadingText;
    public TMP_Text vertexColorModeText;
    public TMP_Text visabilityModeText;
    public TMP_Text vertexColorSemiTransText;
    public TMP_Dropdown materialDropdown;
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

    bool refuseCallback = false;

    public void Refresh() {

        refuseCallback = true;

        surface = ObjectEditorMain.fCopObject.surfaceByCompiledOffset[primitive.surfaceIndex];

        unknown1Text.text = primitive.unknown1.ToString();
        textureEnabledText.text = primitive.textureEnabled.ToString();
        unknown2Text.text = primitive.unknown2.ToString();
        isReflectiveText.text = primitive.isReflective.ToString();
        shadingText.text = primitive.Material.shading.ToString();
        vertexColorModeText.text = primitive.Material.colorMode.ToString();
        visabilityModeText.text = primitive.Material.visabilityMode.ToString();
        vertexColorSemiTransText.text = primitive.Material.vertexColorSemiTransparent.ToString();

        RefreshMaterialDropdown();

        redInput.text = surface.red.ToString();
        greenInput.text = surface.green.ToString();
        blueInput.text = surface.blue.ToString();
        surfaceTypeInput.text = ((int)surface.type).ToString();
        uvValueText.text = (surface.uvMap != null).ToString();

        refuseCallback = false;

    }

    void RefreshMaterialDropdown() {

        var cases = Enum.GetNames(typeof(FCopObjectMaterial.MaterialEnum));

        materialDropdown.ClearOptions();

        var spacedCases = new List<string>();
        foreach (var c in cases) {
            spacedCases.Add(Utils.AddSpacesToString(c));
        }

        materialDropdown.AddOptions(spacedCases);

        materialDropdown.value = primitive.materialID;

    }

    public void OnFinishRed() {

        if (refuseCallback) return;

        surface.red = Int32.Parse(redInput.text);

    }

    public void OnFinishGreen() {

        if (refuseCallback) return;

        surface.green = Int32.Parse(greenInput.text);

    }

    public void OnFinishBlue() {

        if (refuseCallback) return;

        surface.blue = Int32.Parse(blueInput.text);

    }

    public void OnFinishType() {

        if (refuseCallback) return;

        surface.type = (SurfaceType)Int32.Parse(surfaceTypeInput.text);

    }

    public void RemoveUVs() {

        if (refuseCallback) return;

        surface.uvMap = null;
    }

    public void OnChangeMaterialDropdown() {

        if (refuseCallback) return;

        main.ChangePrimitiveMaterial(materialDropdown.value);

        Refresh();

    }

}