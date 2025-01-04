

using FCopParser;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class ActorSchematicItemView : MonoBehaviour {

    // - Prefabs -
    public GameObject objectPreviewCamera;
    public RenderTexture objectRenderTexture;

    // - Unity Refs -
    public RawImage meshPreview;
    public ContextMenuHandler contextMenu;
    public InfoBoxHandler infoBoxHandler;
    public TextFieldPopupHandler textFieldPopupHandler;

    // - Parameters -
    public ActorEditMode controller;
    public ActorSchematic actorSchematic;
    [HideInInspector]
    public ActorSchematicView view;

    FCopActor createdActor;
    ActorObject actorObject;

    private void Start() {

        createdActor = new FCopActor(new IFFDataFile(0, new(actorSchematic.actorData), "", 0, 0));
        InitSchematicMeshOverlay();
        RenderMesh();

        infoBoxHandler.message = actorSchematic.name;

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        textFieldPopupHandler.finishCallback = text => {

            actorSchematic.name = text;

            infoBoxHandler.message = actorSchematic.name;

        };

    }

    float savedMaxY = 0f;
    void InitSchematicMeshOverlay() {

        var obj = Instantiate(controller.main.BlankActor);
        obj.layer = 8; // UI Mesh
        var actorObject = obj.GetComponent<ActorObject>();
        actorObject.actor = createdActor;
        actorObject.controller = controller;

        actorObject.Create();

        foreach (var objmesh in actorObject.objects) {

            if (objmesh != null) {
                objmesh.ForceMake();
                savedMaxY = savedMaxY < objmesh.maxY ? objmesh.maxY : savedMaxY;
            }

        }

        foreach (Transform trans in actorObject.transform) {
            trans.gameObject.layer = 8; // UI Mesh
        }
        obj.transform.position = new Vector3(0, 0, 0);

        this.actorObject = actorObject;

    }

    void RenderMesh() {

        var camera = Instantiate(objectPreviewCamera);

        var height = savedMaxY / 2f;

        camera.transform.position = new Vector3(0, height, 0);
        camera.transform.eulerAngles = new Vector3(35, 320, 0);
        camera.transform.position -= camera.transform.forward * 3;

        camera.GetComponent<Camera>().Render();

        var texture = new Texture2D(objectRenderTexture.width, objectRenderTexture.height, objectRenderTexture.graphicsFormat, 0, TextureCreationFlags.None);
        RenderTexture.active = objectRenderTexture;
        texture.ReadPixels(new Rect(0, 0, objectRenderTexture.width, objectRenderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        meshPreview.texture = texture;

        DestroyImmediate(actorObject.gameObject);
        DestroyImmediate(camera);

    }

    void Rename() {

        textFieldPopupHandler.OpenPopupTextField(actorSchematic.name);

    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete Actor Schematic", "Are you sure you would like to delete this actor schematic? (This cannot be undone)", () => {

            view.currentDirectory.schematics.Remove(actorSchematic);

            view.RefreshView();

            return true;
        });

    }

    // - Unity Events -

    public void OnClick() {

        controller.StartAddSchematic(actorSchematic);

        view.OnClickDone();

    }

}