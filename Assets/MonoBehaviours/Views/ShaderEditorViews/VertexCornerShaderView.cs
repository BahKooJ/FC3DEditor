

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

            switch (originalType) {
                case VertexColorType.MonoChrome:
                    break;
                case VertexColorType.DynamicMonoChrome:
                    break;
                case VertexColorType.Color:

                    var shader = (ColorShader)tile.shaders;

                    if (tile.verticies.Count == 4) {

                        shader.values[ShaderMapperView.colorDataQuadIndexes[index]] = view.colorValue.Clone();

                        shader.Apply();

                    }

                    break;
                case VertexColorType.ColorAnimated:
                    break;

            }

        }

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