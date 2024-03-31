


using FCopParser;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class FrameItemView : MonoBehaviour {

    // Prefabs
    public GameObject texturePreviewMesh;
    public GameObject texturePreviewCamera;
    public RenderTexture texturePreviewRender;

    // View Refs
    public RawImage texturePreview;

    // Pars
    public TextureUVMapper view;
    public TextureEditMode controller;
    public Tile tile;
    public int frame;

    MeshFilter filter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    GameObject meshObj;

    void Start() {

        // These are for making the shader prview.
        // I have to use this method of making the image otherwise the UI
        // wont cull the list items.
        MakeMesh();
        RenderMesh();
    }

    void MakeMesh() {

        void GenerateQuad() {

            var vertices = MeshUtils.FlatScreenVerticies(tile.verticies, 50f);

            // Seems that animation is only in quads
            if (vertices.Count == 3) {
                vertices = MeshUtils.FlatScreenVerticies(MeshType.VerticiesFromID(68), 50f);
            }

            var frameUVs = tile.animatedUVs.GetRange(frame * 4, 4);

            var uvs = new List<Vector2> {
            new Vector2(
                TextureCoordinate.GetX(frameUVs[0] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(frameUVs[0] + tile.texturePalette * 65536)
                ),

            new Vector2(
                TextureCoordinate.GetX(frameUVs[1] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(frameUVs[1] + tile.texturePalette * 65536)
                ),

            new Vector2(
                TextureCoordinate.GetX(frameUVs[3] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(frameUVs[3] + tile.texturePalette * 65536)
                ),

            new Vector2(
                TextureCoordinate.GetX(frameUVs[2] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(frameUVs[2] + tile.texturePalette * 65536)
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

        //void GenerateTriangle() {

        //    var vertices = MeshUtils.FlatScreenVerticies(tile.verticies, 50f);
        //    var uvs = new List<Vector2> {
        //    new Vector2(
        //        TextureCoordinate.GetX(preset.uvs[0] + preset.texturePalette * 65536),
        //        TextureCoordinate.GetY(preset.uvs[0] + preset.texturePalette * 65536)
        //        ),

        //    new Vector2(
        //        TextureCoordinate.GetX(preset.uvs[2] + preset.texturePalette * 65536),
        //        TextureCoordinate.GetY(preset.uvs[2] + preset.texturePalette * 65536)
        //        ),

        //    new Vector2(
        //        TextureCoordinate.GetX(preset.uvs[1] + preset.texturePalette * 65536),
        //        TextureCoordinate.GetY(preset.uvs[1] + preset.texturePalette * 65536)
        //    )};

        //    var triangles = new List<int> {
        //        0,
        //        1,
        //        2
        //    };

        //    mesh.vertices = vertices.ToArray();
        //    mesh.triangles = triangles.ToArray();
        //    mesh.uv = uvs.ToArray();

        //    mesh.RecalculateNormals();

        //}

        meshObj = Instantiate(texturePreviewMesh);

        filter = meshObj.GetComponent<MeshFilter>();
        meshRenderer = meshObj.GetComponent<MeshRenderer>();

        meshRenderer.material.mainTexture = controller.main.levelTexturePallet;

        if (view.frameSelected == frame) {
            meshRenderer.material.color = Color.green;
        }

        mesh = new Mesh();
        filter.mesh = mesh;

        //if (tile.uvs.Count == 4) {
            GenerateQuad();
        //}
        //else {
        //    GenerateTriangle();
        //}

    }

    void RenderMesh() {

        var camera = Instantiate(texturePreviewCamera);
        ((RectTransform)camera.transform).anchoredPosition = new Vector2(25, -25);

        var clp = camera.transform.localPosition;
        clp.z = -1;
        camera.transform.localPosition = clp;

        camera.GetComponent<Camera>().Render();

        var texture = new Texture2D(texturePreviewRender.width, texturePreviewRender.height, texturePreviewRender.graphicsFormat, 0, TextureCreationFlags.None);
        RenderTexture.active = texturePreviewRender;
        texture.ReadPixels(new Rect(0, 0, texturePreviewRender.width, texturePreviewRender.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        texturePreview.texture = texture;

        DestroyImmediate(meshObj);
        DestroyImmediate(camera);

    }

    public void OnClick() {
        view.ClickedFrameItem(frame);
    }


}