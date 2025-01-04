


using FCopParser;
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
    public TMP_Text typeText;

    public UVPreset preset;
    public TextureEditMode controller;
    public TexturePresetsView view;
    public bool forceNameChange;

    MeshFilter filter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    GameObject meshObj;

    // TODO: This works but I really don't like the implementation. It's also really slow.
    // All this just to get it to mask in the UI?
    void Start() {


        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        nameText.text = preset.name;

        var typeString = "";

        if (preset.isSemiTransparent) {
            typeString += "(T) ";
        }

        if (preset.isVectorAnimated) {
            typeString += "Vector Animated";
        }
        else if (preset.animatedUVs.Count != 0) {
            typeString += "Frame Animated";
        }
        else {
            typeString += "Static";
        }

        typeText.text = typeString;

        if (forceNameChange) {
            nameTextField.Select();
        } else {
            nameTextField.gameObject.SetActive(false);
        }

        // These are for making the shader prview.
        // I have to use this method of making the image otherwise the UI
        // wont cull the list items.
        MakeMesh();
        RenderMesh();

    }

    void MakeMesh() {

        void GenerateQuad() {

            var vertices = MeshUtils.FlatScreenVerticies(MeshType.VerticiesFromID(preset.meshID), 50f);

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

        void GenerateTriangle() {

            var vertices = MeshUtils.FlatScreenVerticies(MeshType.VerticiesFromID(preset.meshID), 50f);

            var uvs = new List<Vector2> {
            new Vector2(
                TextureCoordinate.GetX(preset.uvs[0] + preset.texturePalette * 65536),
                TextureCoordinate.GetY(preset.uvs[0] + preset.texturePalette * 65536)
            ),

            new Vector2(
                TextureCoordinate.GetX(preset.uvs[2] + preset.texturePalette * 65536),
                TextureCoordinate.GetY(preset.uvs[2] + preset.texturePalette * 65536)
            ),

            new Vector2(
                TextureCoordinate.GetX(preset.uvs[1] + preset.texturePalette * 65536),
                TextureCoordinate.GetY(preset.uvs[1] + preset.texturePalette * 65536)
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

        meshObj = Instantiate(texturePreviewMesh);

        filter = meshObj.GetComponent<MeshFilter>();
        meshRenderer = meshObj.GetComponent<MeshRenderer>();

        meshRenderer.material.mainTexture = controller.main.levelTexturePallet;

        mesh = new Mesh();
        filter.mesh = mesh;

        if (preset.uvs.Count == 4) {
            GenerateQuad();
        }
        else {
            GenerateTriangle();
        }

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

    // This should really be in the edit mode
    public void OnClick() {

        if (controller.selectedItems.Count == 0) {
            return;
        }

        TextureEditMode.AddTileStateCounterAction();

        foreach (var selection in controller.selectedItems) {

            var tile = selection.tile;

            if (preset.animatedUVs.Count > 0) {

                tile.animationSpeed = preset.animationSpeed;
                tile.animatedUVs = new List<int>(preset.animatedUVs);
                tile.texturePalette = preset.texturePalette;
                tile.isVectorAnimated = false;
                tile.isSemiTransparent = preset.isSemiTransparent;
                
                if (tile.verticies.Count == 4) {
                    tile.uvs = new List<int>(preset.animatedUVs.GetRange(0, 4));
                } else {
                    tile.uvs = new List<int>(preset.animatedUVs.GetRange(0, 3));
                }

                return;
            }

            if (tile.uvs.Count == preset.uvs.Count) {

                tile.uvs = new List<int>(preset.uvs);

            }
            else if (tile.uvs.Count < preset.uvs.Count) {
                tile.uvs = new List<int>(preset.uvs.GetRange(0, 3));
            }
            else {
                return;
            }

            tile.texturePalette = preset.texturePalette;
            tile.isVectorAnimated = preset.isVectorAnimated;
            tile.isSemiTransparent = preset.isSemiTransparent;
            tile.animationSpeed = -1;
            tile.animatedUVs = new List<int>();

        }

        controller.RefreshTileOverlayTexture();
        controller.RefreshUVMapper();
        controller.RefreshMeshes();

    }

}