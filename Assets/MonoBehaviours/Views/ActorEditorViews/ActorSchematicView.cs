

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class ActorSchematicView : MonoBehaviour {

    // - View Prefab -
    public GameObject SchematicItem;
    public GameObject FolderItem;

    // - View Refs -
    public Transform listContent;

    // - Parameters -
    public ActorEditMode controller;
    [HideInInspector]
    public ActorEditPanelView view;

    public ActorSchematics currentDirectory;
    List<GameObject> listGameObjects = new();

    void Start() {

        currentDirectory = Presets.actorSchematics;

        RefreshView();

    }

    FolderActorSchematicItemView CreateFolderItem(ActorSchematics folder) {

        var obj = Instantiate(FolderItem);
        obj.transform.SetParent(listContent, false);
        obj.SetActive(true);

        listGameObjects.Add(obj);

        var view = obj.GetComponent<FolderActorSchematicItemView>();
        view.controller = controller;
        view.actorSchematics = folder;
        view.view = this;

        return view;

    }

    public void RefreshView() {

        foreach (var obj in listGameObjects) {
            Destroy(obj);
        }

        listGameObjects.Clear();

        if (currentDirectory.parent != null) {
            var folder = CreateFolderItem(currentDirectory.parent);
            folder.isBack = true;
        }

        foreach (var folder in currentDirectory.subFolders) {

            CreateFolderItem(folder);

        }

        foreach (var schem in currentDirectory.schematics) {
            var obj = Instantiate(SchematicItem);
            obj.transform.SetParent(listContent, false);
            obj.SetActive(true);

            listGameObjects.Add(obj);

            var view = obj.GetComponent<ActorSchematicItemView>();
            view.controller = controller;
            view.actorSchematic = schem;
            view.view = this;

        }

    }

    public void SwitchDirectory(ActorSchematics folder) {

        currentDirectory = folder;
        RefreshView();

    }

    // - Unity Callbacks -

    public void OnClickDone() {
        view.CloseActorSchematicsView();
    }

    public void OnClickAddFolder() {

        var newFolder = new ActorSchematics(currentDirectory, "Folder");

        currentDirectory.subFolders.Add(newFolder);

        CreateFolderItem(newFolder);

    }

}