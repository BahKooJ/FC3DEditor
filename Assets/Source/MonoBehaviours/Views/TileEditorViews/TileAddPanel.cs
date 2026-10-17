using TMPro;
using UnityEngine;

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

    private void OnDestroy() {

        CloseSchematicsPanel();

    }

    public void OpenSchematicsPanel() {

        CloseSchematicsPanel();

        if (controller.selectedSchematic != null) {
            controller.selectedSchematic.transformedSchematic = null;
        }

        var obj = Instantiate(schematicPanelPrefab);
        obj.transform.SetParent(transform.parent, false);
        activeSchematicView = obj.GetComponent<SchematicMeshPresetsView>();
        activeSchematicView.controller = controller;
        activeSchematicView.view = this;

    }

    public void CloseSchematicsPanel() {

        if (activeSchematicView != null) {

            Destroy(activeSchematicView.gameObject);

            activeSchematicView = null;

        }

    }

    public void Select(int index) {

        controller.SelectTilePreset(index);

    }

}