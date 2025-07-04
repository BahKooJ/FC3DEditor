﻿
using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class FolderActorSchematicItemView : MonoBehaviour {

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

    // - Unity Asset Refs -
    public Sprite backArrowSprite;

    // - Unity Refs -
    public Image icon;
    public ContextMenuHandler contextMenu;
    public InfoBoxHandler infoBoxHandler;
    public TextFieldPopupHandler textFieldPopupHandler;
    public RawImage meshPreview;

    // - Parameters -
    public ActorEditMode controller;
    public ActorSchematics actorSchematics;
    [HideInInspector]
    public ActorSchematicView view;
    [HideInInspector]
    public bool isBack = false;

    FCopActor createdActor;
    ActorObject actorObject;

    private void Start() {

        infoBoxHandler.message = actorSchematics.directoryName;

        contextMenu.items = new() {
            ("Rename", Rename), ("Delete", Delete)
        };

        textFieldPopupHandler.finishCallback = text => {

            actorSchematics.directoryName = text;

            infoBoxHandler.message = actorSchematics.directoryName;

        };

        if (isBack) {
            icon.sprite = backArrowSprite;
            GetComponent<DragableUIElement>().refuseDrag = true;
        }

        if (actorSchematics.schematics.Count == 0 || isBack) return;

        meshPreview.gameObject.SetActive(true);

        if (actorSchematics.schematics[0].behavior == ActorBehavior.Teleporter) {
            meshPreview.texture = teleporterIcon;
        }
        else if (actorSchematics.schematics[0].behavior == ActorBehavior.Trigger) {
            meshPreview.texture = triggerIcon;
        }
        else if (actorSchematics.schematics[0].behavior == ActorBehavior.UniversalTrigger) {
            meshPreview.texture = universialTriggerIcon;
        }
        else if (actorSchematics.schematics[0].behavior == ActorBehavior.Weapon) {
            meshPreview.texture = weaponIcon;
        }
        else if (actorSchematics.schematics[0].behavior == ActorBehavior.PlayerWeapon) {
            meshPreview.texture = playerWeaponIcon;
        }
        else if (actorSchematics.schematics[0].behavior == ActorBehavior.MapObjectiveNodes) {
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
            }.Contains(actorSchematics.schematics[0].behavior)) {

            meshPreview.texture = effectsIcon;
        }
        else {

            createdActor = new FCopActor(new IFFDataFile(0, new(actorSchematics.schematics[0].actorData), "", 0, 0));
            InitSchematicMeshOverlay();

            if (actorObject.missingObjects) {
                DestroyImmediate(actorObject.gameObject);
                meshPreview.texture = missingObjectIcon;
            }
            else {

                RenderMesh();

            }

        }

    }

    float savedMaxY = 0f;
    void InitSchematicMeshOverlay() {

        GameObject obj;

        if (actorSchematics.schematics[0].behavior == ActorBehavior.Texture) {

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

        if (actorSchematics.schematics[0].behavior == ActorBehavior.Texture) {

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

        textFieldPopupHandler.OpenPopupTextField(actorSchematics.directoryName);

    }

    void Delete() {

        DialogWindowUtil.Dialog("Delete Actor Schematic Folder", "Are you sure you would like to delete this actor schematic? " +
            "This will delete all schematics inside this folder. This cannot be undone.", () => {

            view.currentDirectory.subFolders.Remove(actorSchematics);

            view.RefreshView();

            return true;
        });

    }

    // - Unity Callbacks -
    public void OnClick() {

        view.SwitchDirectory(actorSchematics);

    }

    public void ReceiveDrag() {

        if (Main.draggingElement.TryGetComponent<ActorSchematicItemView>(out var viewItem)) {

            view.currentDirectory.schematics.Remove(viewItem.actorSchematic);

            actorSchematics.schematics.Add(viewItem.actorSchematic);

            Destroy(viewItem.gameObject);

        }

        if (Main.draggingElement.TryGetComponent<FolderActorSchematicItemView>(out var viewItem2)) {

            view.currentDirectory.subFolders.Remove(viewItem2.actorSchematics);

            actorSchematics.subFolders.Add(viewItem2.actorSchematics);
            viewItem2.actorSchematics.parent = actorSchematics;

            Destroy(viewItem2.gameObject);

        }

    }

    public void ReceiveReorderLeft() {

        if (isBack) return;

        if (Main.draggingElement.TryGetComponent<FolderActorSchematicItemView>(out var viewItem)) {

            var indexOfItem = view.currentDirectory.subFolders.IndexOf(viewItem.actorSchematics);
            var indexOfThis = view.currentDirectory.subFolders.IndexOf(actorSchematics);

            view.currentDirectory.subFolders.Remove(viewItem.actorSchematics);

            if (indexOfThis > indexOfItem) {

                view.currentDirectory.subFolders.Insert(indexOfThis - 1, viewItem.actorSchematics);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);

            }
            else {

                view.currentDirectory.subFolders.Insert(indexOfThis, viewItem.actorSchematics);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex());

            }

        }

    }

    public void ReceiveReorderRight() {

        if (isBack) return;

        if (Main.draggingElement.TryGetComponent<FolderActorSchematicItemView>(out var viewItem)) {

            var indexOfItem = view.currentDirectory.subFolders.IndexOf(viewItem.actorSchematics);
            var indexOfThis = view.currentDirectory.subFolders.IndexOf(actorSchematics);

            view.currentDirectory.subFolders.Remove(viewItem.actorSchematics);

            if (indexOfThis > indexOfItem) {

                view.currentDirectory.subFolders.Insert(indexOfThis, viewItem.actorSchematics);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex());

            }
            else {

                view.currentDirectory.subFolders.Insert(indexOfThis + 1, viewItem.actorSchematics);

                viewItem.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);

            }

        }

    }

}