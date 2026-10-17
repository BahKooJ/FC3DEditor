
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SchematicMeshPresetsView : MonoBehaviour {

    // - View Prefab -
    public GameObject SchematicItem;
    public GameObject FolderItem;

    // - View Refs -
    public Transform listContent;
    public Toggle overwriteTilesToggle;
    public TMP_Dropdown settingsDropdown;

    // - Parameters -
    public TileAddMode controller;
    [HideInInspector]
    public TileAddPanel view;

    public Schematics currentDirectory;
    List<GameObject> listGameObjects = new();

    void Start() {

        currentDirectory = Presets.levelSchematics;

        RefreshView();

    }

    FolderSchematicItemView CreateFolderItem(Schematics folder) {

        var obj = Instantiate(FolderItem);
        obj.transform.SetParent(listContent, false);
        obj.SetActive(true);

        listGameObjects.Add(obj);

        var view = obj.GetComponent<FolderSchematicItemView>();
        view.parentView = this;
        view.controller = controller;
        view.schematics = folder;
        view.view = this;

        return view;

    }

    public void RefreshView() {

        Clear();

        if (currentDirectory.parent != null) {
            var folder = CreateFolderItem(currentDirectory.parent);
            folder.isBack = true;
        }

        foreach (var folder in currentDirectory.subFolders) {

            CreateFolderItem(folder);

        }

        foreach (var scem in currentDirectory.schematics) {

            var obj = Instantiate(SchematicItem);
            obj.transform.SetParent(listContent, false);
            obj.SetActive(true);

            var item = obj.GetComponent<SchematicMeshItemView>();
            item.parentView = this;
            item.view = view;
            item.controller = controller;
            item.schematic = scem;

            listGameObjects.Add(obj);

        }

        overwriteTilesToggle.isOn = TileAddMode.removeAllTilesOnSchematicPlacement;
        settingsDropdown.value = (int)TileAddMode.placementSetting;

    }

    void Clear() {

        foreach (var item in listGameObjects) {
            Destroy(item.gameObject);
        }

        listGameObjects.Clear();

    }

    public void SwitchDirectory(Schematics folder) {

        currentDirectory = folder;
        RefreshView();

    }

    // - Unity Callbacks -

    public void OnClickDone() {
        view.CloseSchematicsPanel();
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

    public void OnClickAddFolder() {

        var newFolder = new Schematics("Folder", currentDirectory);

        currentDirectory.subFolders.Add(newFolder);

        CreateFolderItem(newFolder);

    }

}