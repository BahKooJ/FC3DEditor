
using System.Collections.Generic;
using UnityEngine;

public class SchematicMeshPresetsView : MonoBehaviour {

    // - View Prefab -
    public GameObject SchematicItem;

    // - View Refs -
    public Transform listContent;

    // - Parameters -
    public TileAddMode controller;
    public TileAddPanel view;

    public List<SchematicMeshItemView> schematicListItems = new();

    void Start() {

        foreach (var scem in Presets.levelSchematics) {

            var obj = Instantiate(SchematicItem);
            obj.transform.SetParent(listContent, false);
            obj.SetActive(true);

            var item = obj.GetComponent<SchematicMeshItemView>();
            item.parentView = this;
            item.view = view;
            item.controller = controller;
            item.schematic = scem;

            schematicListItems.Add(item);

        }

    }

    public void OnClickDone() {
        view.CloseTileEffectsPanel();
    }

}