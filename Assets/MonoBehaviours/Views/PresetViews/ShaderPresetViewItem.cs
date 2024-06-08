

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class ShaderPresetViewItem : MonoBehaviour {

    // Prefabs
    public GameObject shaderPreviewMesh;
    public GameObject shaderPreviewCamera;
    public RenderTexture shaderPreviewRender;

    // View refs
    public TMP_Text typeText;
    public TMP_Text nameText;
    public TMP_InputField nameTextField;
    public ContextMenuHandler contextMenu;
    public RawImage shaderPreview;

    public ShaderPreset preset;
    public ShaderEditMode controller;
    public ShaderPresetsView view;
    public bool forceNameChange;

    MeshFilter filter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    GameObject meshObj;

    void Start() {

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        nameText.text = preset.name;

        if (forceNameChange) {
            nameTextField.Select();
        }
        else {
            nameTextField.gameObject.SetActive(false);
        }

        switch(preset.shader) {

            case MonoChromeShader:
                typeText.text = "Solid Monochrome";
                break;
            case DynamicMonoChromeShader:
                typeText.text = "Monochrome";
                break;
            case ColorShader:
                typeText.text = "Color";
                break;
            case AnimatedShader:
                typeText.text = "Animated";
                break;

        }

        if (preset.meshID > 70 && preset.shader.type != VertexColorType.MonoChrome) {
            typeText.text = "(Wall) " + typeText.text;
        }

        // These are for making the texture prview.
        // I have to use this method of making the image otherwise the UI
        // wont cull the list items.
        MakeMesh();
        RenderMesh();

    }

    void MakeMesh() {

        void GenerateQuad() {

            List<Color> vertexColors = new();

            var vertices = MeshUtils.FlatScreenVerticies(MeshType.VerticiesFromID(preset.meshID), 50f);

            var triangles = new List<int> {
                0,
                2,
                1,

                1,
                2,
                3
            };

            vertexColors.Add(new Color(preset.shader.colors[0][0], preset.shader.colors[0][1], preset.shader.colors[0][2]));
            vertexColors.Add(new Color(preset.shader.colors[1][0], preset.shader.colors[1][1], preset.shader.colors[1][2]));
            vertexColors.Add(new Color(preset.shader.colors[2][0], preset.shader.colors[2][1], preset.shader.colors[2][2]));
            vertexColors.Add(new Color(preset.shader.colors[3][0], preset.shader.colors[3][1], preset.shader.colors[3][2]));


            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = vertexColors.ToArray();

            mesh.RecalculateNormals();

        }

        void GenerateTriangle() {

            List<Color> vertexColors = new();

            var vertices = MeshUtils.FlatScreenVerticies(MeshType.VerticiesFromID(preset.meshID), 50f);

            var triangles = new List<int> {
                0,
                1,
                2
            };

            vertexColors.Add(new Color(preset.shader.colors[0][0], preset.shader.colors[0][1], preset.shader.colors[0][2]));
            vertexColors.Add(new Color(preset.shader.colors[1][0], preset.shader.colors[1][1], preset.shader.colors[1][2]));
            vertexColors.Add(new Color(preset.shader.colors[2][0], preset.shader.colors[2][1], preset.shader.colors[2][2]));


            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = vertexColors.ToArray();

            mesh.RecalculateNormals();

        }

        meshObj = Instantiate(shaderPreviewMesh);

        filter = meshObj.GetComponent<MeshFilter>();
        meshRenderer = meshObj.GetComponent<MeshRenderer>();

        mesh = new Mesh();
        filter.mesh = mesh;

        if (preset.shader.isQuad) {
            GenerateQuad();
        }
        else {
            GenerateTriangle();
        }

        if (preset.shader is MonoChromeShader || preset.shader is DynamicMonoChromeShader) {
            meshRenderer.material.color = new Color(0.3f, 0.8f, 0.3f);
        }

    }

    void RenderMesh() {

        var camera = Instantiate(shaderPreviewCamera);
        ((RectTransform)camera.transform).anchoredPosition = new Vector2(25, -25);

        var clp = camera.transform.localPosition;
        clp.z = -1;
        camera.transform.localPosition = clp;

        camera.GetComponent<Camera>().Render();

        var texture = new Texture2D(shaderPreviewRender.width, shaderPreviewRender.height, shaderPreviewRender.graphicsFormat, 0, TextureCreationFlags.None);
        RenderTexture.active = shaderPreviewRender;
        texture.ReadPixels(new Rect(0, 0, shaderPreviewRender.width, shaderPreviewRender.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        shaderPreview.texture = texture;

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

        controller.currentShaderPresets.presets.Remove(preset);
        view.Refresh();

        return true;
    }

    public void OnStartNameType() {

        Main.ignoreAllInputs = true;

    }

    public void OnEndNameType() {

        Main.ignoreAllInputs = false;

        if (nameTextField.text == "") {
            preset.name = "Shader Preset";
        }
        else {
            preset.name = nameTextField.text;
        }

        nameText.text = preset.name;

        nameTextField.gameObject.SetActive(false);

    }

    public void OnClick() {

        ShaderEditMode.AddTileStateCounterAction();

        foreach (var selection in controller.selectedItems) {

            var tile = selection.tile;

            if (tile.shaders.isQuad == preset.shader.isQuad) {

                tile.shaders = preset.shader.Clone();

            }

        }

        controller.RefreshTileOverlayShader();
        controller.RefreshShaderMapper();
        controller.RefreshMeshes();

    }

}