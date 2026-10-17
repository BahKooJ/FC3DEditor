

using FCopParser;
using UnityEngine;

public class VertexColorPoint : MonoBehaviour {

    public BoxCollider boxCollider;
    Material material;

    public ShaderEditMode controller;
    
    public TileSelection selectedItem;
    public int index;
    public bool isSelected = false;

    void Start() {

        boxCollider = GetComponent<BoxCollider>();

        material = GetComponent<MeshRenderer>().material;

        RefreshColors();

    }

    public float[] GetColor() {

        return selectedItem.tile.shaders.colors[index];

    }

    public void SelectOrDeselect() {

        isSelected = !isSelected;

        RefreshColors();

        if (isSelected) {
            controller.colorPicker.SelectedVertexColor(index);
        }

    }

    public void Select() {

        isSelected = true;
        
        material.color = Color.green;

        if (controller.colorPicker == null) {
            controller.view.OpenShaderMapper();
        }

        controller.colorPicker.SelectedVertexColor(index);

    }

    public void Deselect() {

        isSelected = false;

        RefreshColors();

    }

    public void RefreshColors() {

        if (isSelected) {
            material.color = Color.green;
        }
        else {

            var tile = selectedItem.tile;

            material.color = new Color(tile.shaders.colors[index][0], tile.shaders.colors[index][1], tile.shaders.colors[index][2]);

        }

    }

    public void OverrideWhite() {

        var tile = selectedItem.tile;


        switch (tile.shaders.type) {
            case VertexColorType.MonoChrome:
                break;
            case VertexColorType.DynamicMonoChrome:

                var monoShader = (DynamicMonoChromeShader)tile.shaders;

                if (tile.verticies.Count == 4) {

                    if (!(monoShader.values[ShaderColorPickerView.monoDataQuadIndexes[index]] == DynamicMonoChromeShader.white)) {
                        return;
                    }

                    monoShader.values[ShaderColorPickerView.monoDataQuadIndexes[index]] = ShaderColorPickerView.overrideWhiteDynamicMonoValue;

                    monoShader.Apply();

                }
                else {

                    if (!(monoShader.values[ShaderColorPickerView.monoDataTrianglesIndexes[index]] == DynamicMonoChromeShader.white)) {
                        return;
                    }

                    monoShader.values[ShaderColorPickerView.monoDataTrianglesIndexes[index]] = ShaderColorPickerView.overrideWhiteDynamicMonoValue;

                    monoShader.Apply();

                }

                break;
            case VertexColorType.Color:

                var shaderColor = (ColorShader)tile.shaders;

                if (tile.verticies.Count == 4) {

                    if (!shaderColor.values[ShaderColorPickerView.colorDataQuadIndexes[index]].IsWhite()) {
                        return;
                    }

                    shaderColor.values[ShaderColorPickerView.colorDataQuadIndexes[index]] = ShaderColorPickerView.overrideWhiteColorValue.Clone();

                    shaderColor.Apply();

                }
                else {

                    if (!shaderColor.values[ShaderColorPickerView.colorDataTrianglesIndexes[index]].IsWhite()) {
                        return;
                    }

                    shaderColor.values[ShaderColorPickerView.colorDataTrianglesIndexes[index]] = ShaderColorPickerView.overrideWhiteColorValue.Clone();

                    shaderColor.Apply();

                }

                break;
            case VertexColorType.ColorAnimated:
                break;

        }

    }

    public void ChangeValue() {

        if (controller.colorPicker == null) {
            return;
        }

        if (!controller.Painting) {
            ManualChangeValue();
        }
        else {
            ApplyChanges(selectedItem, controller.colorPicker.colorType);

            selectedItem.section.RefreshMesh();

        }
        RefreshColors();

    }


    void ApplyChanges(TileSelection selection, VertexColorType originalType) {

        var tile = selection.tile;

        if (tile.shaders.type != originalType) {
            tile.ChangeShader(originalType);
        }

        if (tile.verticies.Count <= index) {
            return;
        }

        switch (originalType) {
            case VertexColorType.MonoChrome:

                var shaderSolid = (MonoChromeShader)tile.shaders;

                shaderSolid.value = controller.colorPicker.solidMonoByteValue;

                shaderSolid.Apply();

                break;
            case VertexColorType.DynamicMonoChrome:

                var monoShader = (DynamicMonoChromeShader)tile.shaders;

                if (tile.verticies.Count == 4) {

                    monoShader.values[ShaderColorPickerView.monoDataQuadIndexes[index]] = controller.colorPicker.dynamicMonoValue;

                    monoShader.Apply();

                }
                else {

                    monoShader.values[ShaderColorPickerView.monoDataTrianglesIndexes[index]] = controller.colorPicker.dynamicMonoValue;

                    monoShader.Apply();

                }

                break;
            case VertexColorType.Color:

                var shaderColor = (ColorShader)tile.shaders;

                if (tile.verticies.Count == 4) {

                    shaderColor.values[ShaderColorPickerView.colorDataQuadIndexes[index]] = controller.colorPicker.colorValue.Clone();

                    shaderColor.Apply();

                }
                else {

                    shaderColor.values[ShaderColorPickerView.colorDataTrianglesIndexes[index]] = controller.colorPicker.colorValue.Clone();

                    shaderColor.Apply();

                }

                break;
            case VertexColorType.ColorAnimated:
                break;

        }

    }

    void ManualChangeValue() {

        var originalType = controller.FirstTile.shaders.type;

        foreach (var selection in controller.selectedItems) {

            ApplyChanges(selection, controller.colorPicker.colorType);

        }

    }

}