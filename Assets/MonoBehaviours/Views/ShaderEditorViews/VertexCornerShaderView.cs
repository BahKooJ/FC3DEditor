

using FCopParser;
using UnityEngine;
using UnityEngine.UI;

public class VertexCornerShaderView : MonoBehaviour {
    
    // View refs
    public Image image;

    // Pars
    public int index;
    public ShaderMapperView view;
    public ShaderEditMode controller;

    public bool isSelected;

    public void Init() {

        var tile = controller.selectedTiles[0];

        if (isSelected) {
            image.color = Color.green;
            return;
        }

        image.color = new Color(tile.shaders.colors[index][0], tile.shaders.colors[index][1], tile.shaders.colors[index][2]);

        //switch (tile.shaders.type) {
        //    case VertexColorType.MonoChrome:
        //        break;
        //    case VertexColorType.DynamicMonoChrome:
        //        break;
        //    case VertexColorType.Color:
        //        break;
        //    case VertexColorType.ColorAnimated:
        //        break;

        //}

    }

    public void ChangeValue() {

        var originalType = controller.selectedTiles[0].shaders.type;

        foreach (var tile in controller.selectedTiles) {

            if (tile.shaders.type != originalType) {
                tile.ChangeShader(originalType);
            }

            switch (originalType) {
                case VertexColorType.MonoChrome:

                    var shaderSolid = (MonoChromeShader)tile.shaders;

                    shaderSolid.value = view.solidMonoByteValue;

                    shaderSolid.Apply();

                    break;
                case VertexColorType.DynamicMonoChrome:

                    var monoShader = (DynamicMonoChromeShader)tile.shaders;

                    if (tile.verticies.Count == 4) {

                        monoShader.values[ShaderMapperView.monoDataQuadIndexes[index]] = view.dynamicMonoValue;

                        monoShader.Apply();

                    } else {

                        monoShader.values[ShaderMapperView.monoDataTrianglesIndexes[index]] = view.dynamicMonoValue;

                        monoShader.Apply();

                    }

                    break;
                case VertexColorType.Color:

                    var shaderColor = (ColorShader)tile.shaders;

                    if (tile.verticies.Count == 4) {

                        shaderColor.values[ShaderMapperView.colorDataQuadIndexes[index]] = view.colorValue.Clone();

                        shaderColor.Apply();

                    } else {

                        shaderColor.values[ShaderMapperView.colorDataTrianglesIndexes[index]] = view.colorValue.Clone();

                        shaderColor.Apply();

                    }

                    break;
                case VertexColorType.ColorAnimated:
                    break;

            }

        }

    }

    public void ChangeMonoValue(int value) {

        foreach (var tile in controller.selectedTiles) {

            if (tile.shaders.type != VertexColorType.DynamicMonoChrome) {
                tile.ChangeShader(VertexColorType.DynamicMonoChrome);
            }

            var monoShader = (DynamicMonoChromeShader)tile.shaders;

            if (tile.verticies.Count == 4) {

                monoShader.values[ShaderMapperView.monoDataQuadIndexes[index]] = value;

                monoShader.Apply();

            }
            else {

                monoShader.values[ShaderMapperView.monoDataTrianglesIndexes[index]] = value;

                monoShader.Apply();

            }

        }

    }

    public void ChangeColor(XRGB555 value) {

        foreach (var tile in controller.selectedTiles) {

            if (tile.shaders.type != VertexColorType.Color) {
                tile.ChangeShader(VertexColorType.Color);
            }

            var colorShader = (ColorShader)tile.shaders;

            if (tile.verticies.Count == 4) {

                colorShader.values[ShaderMapperView.colorDataQuadIndexes[index]] = value;

                colorShader.Apply();

            }
            else {

                colorShader.values[ShaderMapperView.colorDataTrianglesIndexes[index]] = value;

                colorShader.Apply();

            }

        }

    }

    public int GetMonochromeValue() {
        var monoColor = (DynamicMonoChromeShader)controller.selectedTiles[0].shaders;

        if (monoColor.isQuad) {
            return monoColor.values[ShaderMapperView.monoDataQuadIndexes[index]];
        }

        return monoColor.values[ShaderMapperView.monoDataTrianglesIndexes[index]];

    }

    public XRGB555 GetColorValue() {

        var shaderColor = (ColorShader)controller.selectedTiles[0].shaders;

        if (shaderColor.isQuad) {
            return shaderColor.values[ShaderMapperView.colorDataQuadIndexes[index]].Clone();
        }

        return shaderColor.values[ShaderMapperView.colorDataTrianglesIndexes[index]].Clone();

    }

    public void DeSelect() {

        isSelected = false;
        Init();

    }

    public void OnClick() {

        if (!Controls.IsDown("ModifierMultiSelect")) {

            foreach (var corner in view.corners) {
                corner.DeSelect();
            }

        }

        isSelected = !isSelected;
        Init();

        if (isSelected) {
            view.SelectedCorner(index);
        }

    }

}