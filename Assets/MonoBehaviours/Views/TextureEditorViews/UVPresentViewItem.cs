


using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVPresentViewItem : MonoBehaviour {

    public MeshFilter filter;
    public MeshRenderer meshRenderer;

    public UVPreset preset;
    public TextureEditMode controller;

    //TODO: Mesh is not masked is scrollview
    Mesh mesh;

    void Start() {

        mesh = new Mesh();
        filter.mesh = mesh;

        meshRenderer.material.mainTexture = controller.main.levelTexturePallet;

        var vertices = new List<Vector3>() { new Vector3(0,0), new Vector3(50, 0), new Vector3(0, 50), new Vector3(50, 50) };

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

        var triangles = new List<int> {
            0,
            2,
            1,

            1,
            2,
            3
        };

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

    }

}