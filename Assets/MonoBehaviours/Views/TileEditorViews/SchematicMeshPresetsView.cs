
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SchematicMeshPresetsView : MonoBehaviour {

    // - View Prefab -
    public GameObject SchematicItem;

    // - View Refs -
    public Transform listContent;
    public Toggle overwriteTilesToggle;
    public TMP_Dropdown settingsDropdown;

    // - Parameters -
    public TileAddMode controller;
    public TileAddPanel view;

    public List<SchematicMeshItemView> schematicListItems = new();

    void Start() {

        RefreshView();

    }

    public void RefreshView() {

        Clear();

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

        overwriteTilesToggle.isOn = TileAddMode.removeAllTilesOnSchematicPlacement;
        settingsDropdown.value = (int)TileAddMode.placementSetting;

    }

    void Clear() {

        foreach (var item in schematicListItems) {
            Destroy(item.gameObject);
        }

        schematicListItems.Clear();

    }

    // - Unity Callbacks -

    public void OnClickDone() {
        view.CloseTileEffectsPanel();
    }

    public void OnToggleOverwriteTiles() {
        TileAddMode.removeAllTilesOnSchematicPlacement = overwriteTilesToggle.isOn;
    }

    public void OnChangeSettingDropdown() {
        TileAddMode.placementSetting = (TileAddMode.SchematicPlacementSetting)settingsDropdown.value;

        if (controller.schematicBuildOverlay != null) {
            controller.schematicBuildOverlay.RefreshPreviewColumns();
            controller.schematicBuildOverlay.RefreshMesh();

        }

    }

}