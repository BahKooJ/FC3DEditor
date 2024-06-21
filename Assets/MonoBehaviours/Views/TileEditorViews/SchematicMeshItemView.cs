

using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class SchematicMeshItemView : MonoBehaviour {

    // - Prefab -
    public GameObject schematicPreviewCamera;
    public RenderTexture levelSchematicRenderTexture;

    // - Parameters -
    public SchematicMeshPresetsView parentView;
    public TileAddPanel view;
    public TileAddMode controller;
    public Schematic schematic;
    public ContextMenuHandler contextMenu;
    public InfoBoxHandler infoBoxHandler;

    // - View Refs -
    public RawImage meshPreview;

    SchematicMesh meshObj;

    void Start() {
        InitSchematicMeshOverlay();
        RenderMesh();

        infoBoxHandler.message = schematic.name;

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

    }

    void InitSchematicMeshOverlay() {

        var obj = Instantiate(controller.main.schematicMesh);
        obj.layer = 8; // UI Mesh
        obj.transform.position = new Vector3(0, 0, 0);
        var schematicMesh = obj.GetComponent<SchematicMesh>();
        schematicMesh.controller = controller;
        schematicMesh.schematic = schematic;
        schematicMesh.meshRenderer.material.color = Color.white;
        schematicMesh.ForceMake();
        meshObj = schematicMesh;

    }

    void RenderMesh() {

        var padding = 2;

        var camera = Instantiate(schematicPreviewCamera);
        camera.transform.position = new Vector3(schematic.width / 2f, 0, -(schematic.height / 2f));
        camera.transform.eulerAngles = new Vector3(35, 40, 0);
        camera.transform.position -= camera.transform.forward * (schematic.width + padding);

        camera.GetComponent<Camera>().Render();

        var texture = new Texture2D(levelSchematicRenderTexture.width, levelSchematicRenderTexture.height, levelSchematicRenderTexture.graphicsFormat, 0, TextureCreationFlags.None);
        RenderTexture.active = levelSchematicRenderTexture;
        texture.ReadPixels(new Rect(0, 0, levelSchematicRenderTexture.width, levelSchematicRenderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        meshPreview.texture = texture;

        DestroyImmediate(meshObj.gameObject);
        DestroyImmediate(camera);

    }

    void Rename() {

    }

    void Delete() {

    }

    // - Unity Callbacks -
    
    public void OnClick() {
        controller.SelectSchematic(schematic);
    }


}