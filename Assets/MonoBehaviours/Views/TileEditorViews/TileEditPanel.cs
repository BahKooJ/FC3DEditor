using FCopParser;
using UnityEngine;

public class TileEditPanel : MonoBehaviour {

    // - Prefabs -
    public GameObject tileEffectsView;
    public GameObject tileSplitLinePrefab;

    public GameObject debugTilePanel;
    public TileEditMode controller;

    public GameObject debugTilePanelView = null;
    public TileEffectsView activeTileEffectsView;

    TileSplitLine tileSplitLine = null;

    void Start() {
        controller.view = this;

    }

    void Update() {

    }

    void OnDestroy() {
        CloseTileEffectsPanel();

        DestroyTileSplitLine();

    }

    void DestroyTileSplitLine() {

        if (tileSplitLine != null) {
            Destroy(tileSplitLine.gameObject);
        }

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

    public void OnClickBreakApartQuadTileBottomTop() {
        controller.BreakApartQuadTileBottomTop();
    }

    public void OnClickBreakApartQuadTileTopBottom() {
        controller.BreakApartQuadTileTopBottom();
    }

    public void OnClickSaveSchematic() {

        if (controller.HasSelection) {

            foreach (var selectedItem in controller.selectedItems) {

                if (MeshType.IDFromVerticies(selectedItem.tile.verticies) == null) {
                    QuickLogHandler.Log("Selection has an invalid mesh ID!", LogSeverity.Error);
                    return;
                }

            }

            Presets.levelSchematics.Add(new Schematic(controller.selectedItems, "Level Schematic #" + Presets.levelSchematics.Count.ToString()));

            QuickLogHandler.Log("Selection saved to level schematics", LogSeverity.Success);

        } else {

            QuickLogHandler.Log("No tiles are selected!", LogSeverity.Error);

        }


    }

    public void OnClickExtrudeTiles() {
        controller.ExtrudeTiles();
    }

    public void OnHoverBreakApartQuadTileBottomTop() {
        DestroyTileSplitLine();

        if (!controller.HasSelection) {
            return;
        }

        if (controller.FirstTile.verticies.Count == 3) {
            return;
        }

        var obj = Instantiate(tileSplitLinePrefab);
        obj.transform.SetParent(controller.FirstItem.section.transform, false);

        tileSplitLine = obj.GetComponent<TileSplitLine>();
        tileSplitLine.tile = controller.FirstTile;
        tileSplitLine.topBottom = false;

    }

    public void OnEndHoverBreakApartQuadTileBottomTop() {
        DestroyTileSplitLine();

    }

    public void OnHoverBreakApartQuadTileTopBottom() {
        DestroyTileSplitLine();

        if (!controller.HasSelection) {
            return;
        }

        if (controller.FirstTile.verticies.Count == 3) {
            return;
        }

        var obj = Instantiate(tileSplitLinePrefab);
        obj.transform.SetParent(controller.FirstItem.section.transform, false);

        tileSplitLine = obj.GetComponent<TileSplitLine>();
        tileSplitLine.tile = controller.FirstTile;
        tileSplitLine.topBottom = true;

    }

    public void OnEndHoverBreakApartQuadTileTopBottom() {
        DestroyTileSplitLine();

    }

}