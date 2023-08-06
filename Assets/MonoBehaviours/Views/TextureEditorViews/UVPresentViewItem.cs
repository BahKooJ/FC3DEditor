


using FCopParser;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class UVPresentViewItem : MonoBehaviour {

    // Prefabs
    public GameObject texturePreviewMesh;
    public GameObject texturePreviewCamera;
    public RenderTexture texturePreviewRender;

    // View refs
    public TMP_Text nameText;
    public TMP_InputField nameTextField;
    public ContextMenuHandler contextMenu;
    public RawImage texturePreview;

    public UVPreset preset;
    public TextureEditMode controller;
    public TexturePresetsView view;
    public bool forceNameChange;

    public MeshFilter filter;
    public MeshRenderer meshRenderer;
    Mesh mesh;

    // TODO: This works but I really don't like the implementation. It's also really slow.
    // All this just to get it to mask in the UI?
    void Start() {

        var meshObj = Instantiate(texturePreviewMesh);
        //meshObj.transform.SetParent(transform, false);

        filter = meshObj.GetComponent<MeshFilter>();
        meshRenderer = meshObj.GetComponent<MeshRenderer>();

        var camera = Instantiate(texturePreviewCamera);
        //camera.transform.SetParent(transform, false);
        ((RectTransform)camera.transform).anchoredPosition = new Vector2(25,25);

        var clp = camera.transform.localPosition;
        clp.z = -1;
        camera.transform.localPosition = clp;

        mesh = new Mesh();
        filter.mesh = mesh;

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        meshRenderer.material.mainTexture = controller.main.levelTexturePallet;

        nameText.text = preset.name;

        if (preset.uvs.Count == 4) {
            GenerateQuad();
        }
        else {
            GenerateTriangle();
        }

        if (forceNameChange) {
            nameTextField.Select();
        } else {
            nameTextField.gameObject.SetActive(false);
        }

        camera.GetComponent<Camera>().Render();

        var texture = new Texture2D(texturePreviewRender.width, texturePreviewRender.height, texturePreviewRender.graphicsFormat, 0, TextureCreationFlags.None);
        RenderTexture.active = texturePreviewRender;
        texture.ReadPixels(new Rect(0, 0, texturePreviewRender.width, texturePreviewRender.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        texturePreview.texture = texture;

        Destroy(meshObj);
        Destroy(camera);

    }

    void GenerateQuad() {

        var vertices = new List<Vector3>() { new Vector3(0, 0), new Vector3(50, 0), new Vector3(0, 50), new Vector3(50, 50) };

        var uvs = new List<Vector2> {
            new Vector2(
            TextureCoordinate.GetX(preset.uvs[3] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[3] + preset.texturePalette * 65536)
        ),

            new Vector2(
            TextureCoordinate.GetX(preset.uvs[2] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[2] + preset.texturePalette * 65536)
        ),

            new Vector2(
            TextureCoordinate.GetX(preset.uvs[0] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[0] + preset.texturePalette * 65536)
        ),

            new Vector2(
            TextureCoordinate.GetX(preset.uvs[1] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[1] + preset.texturePalette * 65536)
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

    void GenerateTriangle() {

        var vertices = new List<Vector3>() { new Vector3(0, 0), new Vector3(0, 50), new Vector3(50, 50) };

        var uvs = new List<Vector2> {
            new Vector2(
            TextureCoordinate.GetX(preset.uvs[1] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[1] + preset.texturePalette * 65536)
        ),

            new Vector2(
            TextureCoordinate.GetX(preset.uvs[0] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[0] + preset.texturePalette * 65536)
        ),

            new Vector2(
            TextureCoordinate.GetX(preset.uvs[2] + preset.texturePalette * 65536),
            TextureCoordinate.GetY(preset.uvs[2] + preset.texturePalette * 65536)
        )};

        var triangles = new List<int> {
            0,
            1,
            2
        };

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

    }

    void Rename() {

        nameTextField.gameObject.SetActive(true);
        nameTextField.text = preset.name;
        nameTextField.Select();

    }

    void Delete() {
        DialogWindowUtil.Dialog("Delete Preset", "Are you sure you want to delete preset " + preset.name + "?", ConfirmDelete);
    }

    bool ConfirmDelete() {

        controller.currentUVPresets.presets.Remove(preset);
        view.Refresh();

        return true;
    }

    public void OnStartNameType() {

        Main.ignoreAllInputs = true;

    }

    public void OnEndNameType() {
        
        Main.ignoreAllInputs = false;

        if (nameTextField.text == "") {
            preset.name = "Texture Preset";
        } else {
            preset.name = nameTextField.text;
        }

        nameText.text = preset.name;

        nameTextField.gameObject.SetActive(false);

    }

    public void OnClick() {

        foreach (var tile in controller.selectedTiles) {

            if (tile.uvs.Count == preset.uvs.Count) {

                tile.uvs = preset.uvs;
                tile.texturePalette = preset.texturePalette;

            }

        }

        controller.RefreshTileOverlayTexture();
        controller.RefreshUVMapper();

    }

}