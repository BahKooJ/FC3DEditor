

using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class TextureSnippetItemView : MonoBehaviour {

    // - Prefabs -
    // These are the same prefabs used by texture presets,
    // this is a little spaghetti but it functions the same 
    public GameObject texturePreviewMesh;
    public GameObject texturePreviewCamera;
    public RenderTexture texturePreviewRender;

    // - Unity Refs -
    public ContextMenuHandler contextMenu;
    public RawImage texturePreview;
    public InfoBoxHandler infoBoxHandler;
    public TextFieldPopupHandler textFieldPopupHandler;

    // - Parameters -
    [HideInInspector]
    public Main main;
    [HideInInspector]
    public TextureSnippetsView view;
    public TextureSnippet textureSnippet;
    public FCopLevel level;

    MeshFilter filter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    GameObject meshObj;

    void Start() {

        infoBoxHandler.message = textureSnippet.name;

        contextMenu.items = new() {
            ("Rename", Rename),
            ("Delete", Delete)
        };

        textFieldPopupHandler.finishCallback = text => {

            textureSnippet.name = text;

            infoBoxHandler.message = textureSnippet.name;

        };

        MakeMesh();
        RenderMesh();

    }
    void MakeMesh() {

        meshObj = Instantiate(texturePreviewMesh);

        filter = meshObj.GetComponent<MeshFilter>();
        meshRenderer = meshObj.GetComponent<MeshRenderer>();

        meshRenderer.material.mainTexture = main.levelTexturePallet;

        mesh = new Mesh();
        filter.mesh = mesh;

        var vertices = new List<Vector3> {
            new Vector3(0,0),
            new Vector3(50,0),
            new Vector3(50,-50),
            new Vector3(0,-50),

        };
        var uvs = new List<Vector2> {
            new Vector2(textureSnippet.x / 256f, (textureSnippet.y + textureSnippet.height + (textureSnippet.texturePaletteID * 256)) / 2580f),
            new Vector2(textureSnippet.x / 256f, (textureSnippet.y + (textureSnippet.texturePaletteID * 256)) / 2580f),
            new Vector2((textureSnippet.x + textureSnippet.width) / 256f, (textureSnippet.y + (textureSnippet.texturePaletteID * 256)) / 2580f),
            new Vector2((textureSnippet.x + textureSnippet.width) / 256f, (textureSnippet.y + textureSnippet.height + (textureSnippet.texturePaletteID * 256)) / 2580f)
        };

        var triangles = new List<int> {
                0,
                1,
                2,

                0,
                2,
                3
            };

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

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

    public void Rename() {

        textFieldPopupHandler.OpenPopupTextField(textureSnippet.name);

    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete Texture Snippet", "Are you sure you would like to delete this texture snippet? " +
            "Actors that use this snippet may no longer work properly. Only delete snippets if you are sure it is unused. This cannot be undone.", () => {

                level.DeleteAsset(AssetType.TextureSnippet, textureSnippet.id);

                view.Refresh();

                return true;

            });

    }

}