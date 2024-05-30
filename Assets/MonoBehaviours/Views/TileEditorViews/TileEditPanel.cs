using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileEditPanel : MonoBehaviour {

    // - Prefabs -
    public GameObject tileEffectsView;

    public GameObject debugTilePanel;
    public TileEditMode controller;

    public GameObject debugTilePanelView = null;
    public TileEffectsView activeTileEffectsView;

    void Start() {
        controller.view = this;

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            //OnClickSaveSchematic();
            OpenTileEffectsPanel();
        }

    }

    void OnDestroy() {
        CloseTileEffectsPanel();
    }

    public void RefreshTileEffectsPanel() {

        if (activeTileEffectsView != null) {
            activeTileEffectsView.Refresh();
        }

    }

    void OpenTileEffectsPanel() {

        CloseTileEffectsPanel();

        var obj = Instantiate(tileEffectsView);
        obj.transform.SetParent(transform.parent, false);
        activeTileEffectsView = obj.GetComponent<TileEffectsView>();
        activeTileEffectsView.controller = controller;

    }

    void CloseTileEffectsPanel() {

        if (activeTileEffectsView != null) {

            Destroy(activeTileEffectsView.gameObject);

            activeTileEffectsView = null;

        }

    }

    public void OnClickShiftHeightUpButton() {

        controller.ShiftTilesHeightUp();

    }

    public void OnClickShiftHeightDownButton() {

        controller.ShiftTilesHeightDown();

    }

    public void OnClickSaveSchematic() {

        var foo = new Schematic(controller.selectedItems);

        TileAddMode.selectedSchematic = foo;

    }


}