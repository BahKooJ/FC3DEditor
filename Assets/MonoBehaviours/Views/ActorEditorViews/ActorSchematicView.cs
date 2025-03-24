

using System.Collections.Generic;
using UnityEngine;

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

    public void RefreshView() {

        foreach (var obj in listGameObjects) {
            Destroy(obj);
        }

        listGameObjects.Clear();

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

    // - Unity Callbacks -

    public void OnClickDone() {
        view.CloseActorSchematicsView();
    }

}