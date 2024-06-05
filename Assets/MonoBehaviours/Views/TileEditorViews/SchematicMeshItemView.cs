

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

    // - View Refs -
    public RawImage meshPreview;

    SchematicMesh meshObj;

    void Start() {
        InitSchematicMeshOverlay();
        RenderMesh();
    }

    void InitSchematicMeshOverlay() {

        var obj = Instantiate(controller.main.schematicMesh);
        obj.layer = 8; // UI Mesh
        obj.transform.position = new Vector3(0, 0, 0);
        var schematicMesh = obj.GetComponent<SchematicMesh>();
        schematicMesh.controller = controller;
        schematicMesh.schematic = schematic;
        schematicMesh.ForceMake();
        meshObj = schematicMesh;

    }

    void RenderMesh() {

        var camera = Instantiate(schematicPreviewCamera);
        camera.transform.position = new Vector3(0, 0, 0);
        camera.transform.eulerAngles = new Vector3(35, 35, 0);
        camera.transform.position -= camera.transform.forward * schematic.width;

        camera.GetComponent<Camera>().Render();

        var texture = new Texture2D(levelSchematicRenderTexture.width, levelSchematicRenderTexture.height, levelSchematicRenderTexture.graphicsFormat, 0, TextureCreationFlags.None);
        RenderTexture.active = levelSchematicRenderTexture;
        texture.ReadPixels(new Rect(0, 0, levelSchematicRenderTexture.width, levelSchematicRenderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        meshPreview.texture = texture;

        //DestroyImmediate(meshObj);
        //DestroyImmediate(camera);

    }


}