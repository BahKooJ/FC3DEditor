using FCopParser;
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
    public TextFieldPopupHandler textFieldPopupHandler;

    // - View Refs -
    public RawImage meshPreview;
    public Image background;

    SchematicMesh meshObj;

    void Start() {
        InitSchematicMeshOverlay();
        RenderMesh();

        infoBoxHandler.message = schematic.name;

        contextMenu.items = new() {
            ("Remove Shaders", RemoveShaders), ("Rename", Rename), ("Delete", Delete)
        };

        textFieldPopupHandler.finishCallback = text => {

            schematic.name = text;

            infoBoxHandler.message = schematic.name;

        };

        if (controller.selectedSchematic == schematic) {
            background.color = new Color(0.08f, 0.20f, 0.08f);
        }

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
        camera.transform.position = new Vector3(schematic.width / 2f, schematic.LowestHeight() / 30f, -(schematic.height / 2f));
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

    void RemoveShaders() {

        foreach (var column in schematic.tileColumns) {

            foreach (var tile in column.tiles) {
                tile.shaders = new MonoChromeShader(tile.isQuad);
            }

        }

        InitSchematicMeshOverlay();
        RenderMesh();

    }

    void Rename() {

        textFieldPopupHandler.OpenPopupTextField(schematic.name);

    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete Leve Schematic", "Are you sure you would like to delete this level schematic? (This cannot be undone)", () => {

            parentView.currentDirectory.schematics.Remove(schematic);

            parentView.RefreshView();

            return true;
        });

    }

    // - Unity Callbacks -
    
    public void OnClick() {
        controller.SelectSchematic(schematic);
        parentView.OnClickDone();
    }

    public void ReceiveReorderLeft() {

        if (Main.draggingElement.TryGetComponent<SchematicMeshItemView>(out var viewItem)) {

            var indexOfItem = parentView.currentDirectory.schematics.IndexOf(viewItem.schematic);
            var indexOfThis = parentView.currentDirectory.schematics.IndexOf(schematic);

            parentView.currentDirectory.schematics.Remove(viewItem.schematic);

            if (indexOfThis > indexOfItem) {

                parentView.currentDirectory.schematics.Insert(indexOfThis - 1, viewItem.schematic);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);

            }
            else {

                parentView.currentDirectory.schematics.Insert(indexOfThis, viewItem.schematic);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex());

            }

        }

    }

    public void ReceiveReorderRight() {

        if (Main.draggingElement.TryGetComponent<SchematicMeshItemView>(out var viewItem)) {

            var indexOfItem = parentView.currentDirectory.schematics.IndexOf(viewItem.schematic);
            var indexOfThis = parentView.currentDirectory.schematics.IndexOf(schematic);

            parentView.currentDirectory.schematics.Remove(viewItem.schematic);

            if (indexOfThis > indexOfItem) {

                parentView.currentDirectory.schematics.Insert(indexOfThis, viewItem.schematic);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex());

            }
            else {

                parentView.currentDirectory.schematics.Insert(indexOfThis + 1, viewItem.schematic);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);

            }

        }

    }

}