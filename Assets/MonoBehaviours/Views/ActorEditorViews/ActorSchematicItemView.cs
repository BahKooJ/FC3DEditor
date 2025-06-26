

using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class ActorSchematicItemView : MonoBehaviour {

    // - Unity Asset Refs -
    public Texture2D missingObjectIcon;
    public Texture2D teleporterIcon;
    public Texture2D triggerIcon;
    public Texture2D universialTriggerIcon;
    public Texture2D effectsIcon;
    public Texture2D weaponIcon;
    public Texture2D playerWeaponIcon;
    public Texture2D mapNodesIcon;


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

        if (actorSchematic.behavior == ActorBehavior.Teleporter) {
            meshPreview.texture = teleporterIcon;
        }
        else if (actorSchematic.behavior == ActorBehavior.Trigger) {
            meshPreview.texture = triggerIcon;
        }
        else if (actorSchematic.behavior == ActorBehavior.UniversalTrigger) {
            meshPreview.texture = universialTriggerIcon;
        }
        else if (actorSchematic.behavior == ActorBehavior.Weapon) {
            meshPreview.texture = weaponIcon;
        }
        else if (actorSchematic.behavior == ActorBehavior.PlayerWeapon) {
            meshPreview.texture = playerWeaponIcon;
        }
        else if (actorSchematic.behavior == ActorBehavior.MapObjectiveNodes) {
            meshPreview.texture = mapNodesIcon;
        }
        else if (new List<ActorBehavior>() { 
            ActorBehavior.VisualEffects87, 
            ActorBehavior.VisualEffects88, 
            ActorBehavior.VisualEffects89,
            ActorBehavior.VisualEffects90,
            ActorBehavior.ActorExplosion,
            ActorBehavior.VisualEffects92,
            ActorBehavior.ParticleEmitter,
            ActorBehavior.VisualEffects94,
            }.Contains(actorSchematic.behavior)) {

            meshPreview.texture = effectsIcon;
        }
        else {

            createdActor = new FCopActor(new IFFDataFile(0, new(actorSchematic.actorData), "", 0, 0));
            InitSchematicMeshOverlay();

            if (actorObject.missingObjects) {
                DestroyImmediate(actorObject.gameObject);
                meshPreview.texture = missingObjectIcon;
            }
            else {

                RenderMesh();

            }

        }

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

        GameObject obj;

        if (actorSchematic.behavior == ActorBehavior.Texture) {

            obj = Instantiate(controller.main.TextureActorFab);

        }
        else {

            obj = Instantiate(controller.main.BlankActor);

        }

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

            foreach (Transform nestTrans in trans.transform) {
                nestTrans.gameObject.layer = 8;
            }

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

        if (actorSchematic.behavior == ActorBehavior.Texture) {

            actorObject.transform.GetChild(0).LookAt(camera.transform.position);
            actorObject.transform.GetChild(0).Rotate(new Vector3(90, 0, 0));
        }

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

    public void ReceiveReorderLeft() {

        if (Main.draggingElement.TryGetComponent<ActorSchematicItemView>(out var viewItem)) {

            var indexOfItem = view.currentDirectory.schematics.IndexOf(viewItem.actorSchematic);
            var indexOfThis = view.currentDirectory.schematics.IndexOf(actorSchematic);

            view.currentDirectory.schematics.Remove(viewItem.actorSchematic);

            if (indexOfThis > indexOfItem) {

                view.currentDirectory.schematics.Insert(indexOfThis - 1, viewItem.actorSchematic);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);

            }
            else {

                view.currentDirectory.schematics.Insert(indexOfThis, viewItem.actorSchematic);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex());

            }

        }

    }

    public void ReceiveReorderRight() {

        if (Main.draggingElement.TryGetComponent<ActorSchematicItemView>(out var viewItem)) {

            var indexOfItem = view.currentDirectory.schematics.IndexOf(viewItem.actorSchematic);
            var indexOfThis = view.currentDirectory.schematics.IndexOf(actorSchematic);

            view.currentDirectory.schematics.Remove(viewItem.actorSchematic);

            if (indexOfThis > indexOfItem) {

                view.currentDirectory.schematics.Insert(indexOfThis, viewItem.actorSchematic);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex());

            }
            else {

                view.currentDirectory.schematics.Insert(indexOfThis + 1, viewItem.actorSchematic);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);

            }

        }

    }

}