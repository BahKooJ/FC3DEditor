

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileAddPanel : MonoBehaviour {

    static int selectedPresetCatagory = 0;

    public TileAddMode controller;

    // - Prefabs -
    public GameObject tilePresetPrefab;
    public GameObject schematicPanelPrefab;

    // - View refs -
    public RectTransform presetList;
    public TMP_Dropdown presetCatagory;

    public SchematicMeshPresetsView activeSchematicView = null;

    void Start() {

    }

    public void OpenSchematicsPanel() {

        CloseTileEffectsPanel();

        if (TileAddMode.selectedSchematic != null) {
            TileAddMode.selectedSchematic.transformedSchematic = null;
        }

        var obj = Instantiate(schematicPanelPrefab);
        obj.transform.SetParent(transform.parent, false);
        activeSchematicView = obj.GetComponent<SchematicMeshPresetsView>();
        activeSchematicView.controller = controller;
        activeSchematicView.view = this;

    }

    public void CloseTileEffectsPanel() {

        if (activeSchematicView != null) {

            Destroy(activeSchematicView.gameObject);

            activeSchematicView = null;

        }

    }

    public void Select(int index) {

        controller.SelectTilePreset(index);

    }

}