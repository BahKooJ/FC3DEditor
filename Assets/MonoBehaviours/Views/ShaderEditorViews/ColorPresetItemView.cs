

using FCopParser;
using UnityEngine;
using UnityEngine.UI;

public class ColorPresetItemView : MonoBehaviour {

    public Image image;
    public ContextMenuHandler contextMenu;


    public ShaderMapperView view;
    public XRGB555 color;

    void Start() {

        var floatColors = color.ToColors();

        image.color = new Color(floatColors[0], floatColors[1], floatColors[2]);

        contextMenu.items = new() {
            ("Delete", Delete)
        };

    }

    public void OnClick() {

        view.SetColors(color.Clone());
        view.ApplyColorsToCorners();
        view.controller.RefreshTileOverlayShader();

    }

    void Delete() {

        Presets.colorPresets.presets.Remove(color);

        view.RefreshView();

    }

}