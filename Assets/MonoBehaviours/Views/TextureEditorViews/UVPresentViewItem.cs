


using FCopParser;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UVPresentViewItem : MonoBehaviour {

    // View refs
    public MeshFilter filter;
    public MeshRenderer meshRenderer;
    public TMP_Text nameText;
    public TMP_InputField nameTextField;
    public ContextMenuHandler contextMenu;

    public UVPreset preset;
    public TextureEditMode controller;
    public TexturePresetsView view;
    public bool forceNameChange;

    //TODO: Mesh is not masked is scrollview
    Mesh mesh;

    void Start() {

        mesh = new Mesh();
        filter.mesh = mesh;

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        meshRenderer.material.mainTexture = controller.main.levelTexturePallet;

        nameText.text = preset.name;

        if (preset.uvs.Count == 4) {
            GenerateQuad();
        } else {
            GenerateTriangle();
        }

        if (forceNameChange) {
            nameTextField.Select();
        } else {
            nameTextField.gameObject.SetActive(false);
        }

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

    }

}