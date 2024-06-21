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

        if (!controller.HasSelection) {
            QuickLogHandler.Log("No tiles are selected", LogSeverity.Info);
            return;
        }

        var obj = Instantiate(tileEffectsView);
        obj.transform.SetParent(transform.parent, false);
        activeTileEffectsView = obj.GetComponent<TileEffectsView>();
        activeTileEffectsView.controller = controller;

    }

    public void CloseTileEffectsPanel() {

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

    public void OnClickOpenTileEffectWindowButton() {

        if (activeTileEffectsView != null) {
            CloseTileEffectsPanel();
        }
        else {
            OpenTileEffectsPanel();
        }

    }

    public void OnClickSaveSchematic() {

        if (controller.HasSelection) {

            Presets.levelSchematics.Add(new Schematic(controller.selectedItems, "Level Schematic #" + Presets.levelSchematics.Count.ToString()));

            QuickLogHandler.Log("Selection saved to level schematics", LogSeverity.Success);

        } else {

            QuickLogHandler.Log("No tiles are selected!", LogSeverity.Error);

        }


    }


}