


using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVPresentViewItem : MonoBehaviour {

    public Image texturePreview;
    public UVPreset preset;
    public TextureEditMode controller;

    void Start() {

        texturePreview.sprite = controller.main.bmpTextures[preset.texturePalette];

        var uvs = new List<Vector2> {
            new Vector2(
            TextureCoordinate.GetX(preset.uvs[0] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[0] + preset.texturePalette * 65536)
        ),

            new Vector2(
            TextureCoordinate.GetX(preset.uvs[1] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[1] + preset.texturePalette * 65536)
        ),

            new Vector2(
            TextureCoordinate.GetX(preset.uvs[3] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[3] + preset.texturePalette * 65536)
        ),

            new Vector2(
            TextureCoordinate.GetX(preset.uvs[2] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[2] + preset.texturePalette * 65536)
        )
        };

    }

}