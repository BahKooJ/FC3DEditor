

using FCopParser;
using UnityEditor;
using UnityEngine;

public class VertexColorPoint : MonoBehaviour {

    public BoxCollider boxCollider;
    Material material;

    public ShaderEditMode controller;
    
    public TileSelection selectedItem;
    public int index;

    void Start() {

        boxCollider = GetComponent<BoxCollider>();

        material = GetComponent<MeshRenderer>().material;

        RefreshColors();

    }



    public void Select() {
        
        material.color = Color.green;

        controller.colorPicker.SelectedVertexColor(index);

    }

    public void RefreshColors() {

        var tile = selectedItem.tile;

        material.color = new Color(tile.shaders.colors[index][0], tile.shaders.colors[index][1], tile.shaders.colors[index][2]);

    }

    public void ChangeValue() {

        if (controller.colorPicker == null) {
            return;
        }

        if (!controller.painting) {
            ManualChangeValue();
        }
        else {
            ApplyChanges(selectedItem, controller.colorPicker.colorType);

            selectedItem.section.RefreshMesh();

        }

    }

    void ApplyChanges(TileSelection selection, VertexColorType originalType) {

        var tile = selection.tile;

        if (tile.shaders.type != originalType) {
            tile.ChangeShader(originalType);
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