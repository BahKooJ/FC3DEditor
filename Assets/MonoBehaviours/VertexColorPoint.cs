

using FCopParser;
using UnityEngine;

public class VertexColorPoint : MonoBehaviour {

    public BoxCollider boxCollider;
    Material material;

    public ShaderEditMode controller;
    public ShaderMapperView mapper;
    public TileSelection selectedItem;
    public int index;

    public bool isSelected = false;

    void Start() {

        boxCollider = GetComponent<BoxCollider>();

        material = GetComponent<MeshRenderer>().material;

        var tile = selectedItem.tile;

        material.color = new Color(tile.shaders.colors[index][0], tile.shaders.colors[index][1], tile.shaders.colors[index][2]);

    }

    public void SelectOrDeselect() {

        isSelected = !isSelected;

        RefreshColors();

        if (isSelected) {
            mapper.SelectedCorner(index);
        }

    }

    public void Select() {

        isSelected = true;
        
        material.color = Color.green;

        mapper.SelectedCorner(index);

    }

    public void Deselect() {

        isSelected = false;

        var tile = selectedItem.tile;

        material.color = new Color(tile.shaders.colors[index][0], tile.shaders.colors[index][1], tile.shaders.colors[index][2]);

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

    public void ChangeValue() {

        if (mapper == null) {
            return;
        }

        var originalType = controller.FirstTile.shaders.type;

        foreach (var selection in controller.selectedItems) {

            var tile = selection.tile;

            if (tile.shaders.type != originalType) {
                tile.ChangeShader(originalType);
            }

            switch (originalType) {
                case VertexColorType.MonoChrome:

                    var shaderSolid = (MonoChromeShader)tile.shaders;

                    shaderSolid.value = mapper.solidMonoByteValue;

                    shaderSolid.Apply();

                    break;
                case VertexColorType.DynamicMonoChrome:

                    var monoShader = (DynamicMonoChromeShader)tile.shaders;

                    if (tile.verticies.Count == 4) {

                        monoShader.values[ShaderMapperView.monoDataQuadIndexes[index]] = mapper.dynamicMonoValue;

                        monoShader.Apply();

                    }
                    else {

                        monoShader.values[ShaderMapperView.monoDataTrianglesIndexes[index]] = mapper.dynamicMonoValue;

                        monoShader.Apply();

                    }

                    break;
                case VertexColorType.Color:

                    var shaderColor = (ColorShader)tile.shaders;

                    if (tile.verticies.Count == 4) {

                        shaderColor.values[ShaderMapperView.colorDataQuadIndexes[index]] = mapper.colorValue.Clone();

                        shaderColor.Apply();

                    }
                    else {

                        shaderColor.values[ShaderMapperView.colorDataTrianglesIndexes[index]] = mapper.colorValue.Clone();

                        shaderColor.Apply();

                    }

                    break;
                case VertexColorType.ColorAnimated:
                    break;

            }

        }

    }


}