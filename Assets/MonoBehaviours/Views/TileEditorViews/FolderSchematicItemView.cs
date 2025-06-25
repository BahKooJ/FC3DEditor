
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class FolderSchematicItemView : MonoBehaviour {

    // - Prefab -
    public GameObject schematicPreviewCamera;
    public RenderTexture levelSchematicRenderTexture;

    // - Unity Asset Refs -
    public Sprite backArrowSprite;

    // - Unity Refs -
    public SchematicMeshPresetsView parentView;
    public Image icon;
    public ContextMenuHandler contextMenu;
    public InfoBoxHandler infoBoxHandler;
    public TextFieldPopupHandler textFieldPopupHandler;
    public RawImage meshPreview;

    // - Parameters -
    public TileAddMode controller;
    public Schematics schematics;
    [HideInInspector]
    public SchematicMeshPresetsView view;
    [HideInInspector]
    public bool isBack = false;

    SchematicMesh meshObj;

    private void Start() {

        infoBoxHandler.message = schematics.directoryName;

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        textFieldPopupHandler.finishCallback = text => {

            schematics.directoryName = text;

            infoBoxHandler.message = schematics.directoryName;

        };

        if (isBack) {
            icon.sprite = backArrowSprite;
            GetComponent<DragableUIElement>().refuseDrag = true;
        }

        if (schematics.schematics.Count > 0 && !isBack) {
            meshPreview.gameObject.SetActive(true);
            InitSchematicMeshOverlay();
            RenderMesh();
        }

    }

    void InitSchematicMeshOverlay() {

        var obj = Instantiate(controller.main.schematicMesh);
        obj.layer = 8; // UI Mesh
        obj.transform.position = new Vector3(0, 0, 0);
        var schematicMesh = obj.GetComponent<SchematicMesh>();
        schematicMesh.controller = controller;
        schematicMesh.schematic = schematics.schematics[0];
        schematicMesh.meshRenderer.material.color = Color.white;
        schematicMesh.ForceMake();
        meshObj = schematicMesh;

    }

    void RenderMesh() {

        var padding = 2;

        var camera = Instantiate(schematicPreviewCamera);
        camera.transform.position = new Vector3(schematics.schematics[0].width / 2f, schematics.schematics[0].LowestHeight() / 30f, -(schematics.schematics[0].height / 2f));
        camera.transform.eulerAngles = new Vector3(35, 40, 0);
        camera.transform.position -= camera.transform.forward * (schematics.schematics[0].width + padding);

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

        textFieldPopupHandler.OpenPopupTextField(schematics.directoryName);

    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete Actor Schematic Folder", "Are you sure you would like to delete this actor schematic? " +
            "This will delete all schematics inside this folder. This cannot be undone.", () => {

                view.currentDirectory.subFolders.Remove(schematics);

                view.RefreshView();

                return true;
            });

    }

    // - Unity Callbacks -
    public void OnClick() {

        view.SwitchDirectory(schematics);

    }

    public void ReceiveDrag() {

        if (Main.draggingElement.TryGetComponent<SchematicMeshItemView>(out var viewItem)) {

            view.currentDirectory.schematics.Remove(viewItem.schematic);

            schematics.schematics.Add(viewItem.schematic);

            Destroy(viewItem.gameObject);

        }
        if (Main.draggingElement.TryGetComponent<FolderSchematicItemView>(out var viewItem2)) {

            view.currentDirectory.subFolders.Remove(viewItem2.schematics);

            schematics.subFolders.Add(viewItem2.schematics);
            viewItem2.schematics.parent = schematics;

            Destroy(viewItem2.gameObject);

        }

    }

    public void ReceiveReorderLeft() {

        if (isBack) return;

        if (Main.draggingElement.TryGetComponent<FolderSchematicItemView>(out var viewItem)) {

            var indexOfItem = parentView.currentDirectory.subFolders.IndexOf(viewItem.schematics);
            var indexOfThis = parentView.currentDirectory.subFolders.IndexOf(schematics);

            parentView.currentDirectory.subFolders.Remove(viewItem.schematics);

            if (indexOfThis > indexOfItem) {

                parentView.currentDirectory.subFolders.Insert(indexOfThis - 1, viewItem.schematics);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);

            }
            else {

                parentView.currentDirectory.subFolders.Insert(indexOfThis, viewItem.schematics);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex());

            }

        }

    }

    public void ReceiveReorderRight() {

        if (isBack) return;

        if (Main.draggingElement.TryGetComponent<FolderSchematicItemView>(out var viewItem)) {

            var indexOfItem = parentView.currentDirectory.subFolders.IndexOf(viewItem.schematics);
            var indexOfThis = parentView.currentDirectory.subFolders.IndexOf(schematics);

            parentView.currentDirectory.subFolders.Remove(viewItem.schematics);

            if (indexOfThis > indexOfItem) {

                parentView.currentDirectory.subFolders.Insert(indexOfThis, viewItem.schematics);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex());

            }
            else {

                parentView.currentDirectory.subFolders.Insert(indexOfThis + 1, viewItem.schematics);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);

            }

        }

    }

}