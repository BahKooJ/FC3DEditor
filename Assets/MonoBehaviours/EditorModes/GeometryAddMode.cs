
using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class GeometryAddMode : EditMode {

    public Main main { get; set; }

    public TilePreset? selectedTilePreset = null;

    public TileColumn selectedColumn = null;
    public LevelMesh selectedSection = null;
    public SelectedTileOverlay tileOverlay = null;

    public GeometryAddMode(Main main) {
        this.main = main;
    }

    public void Update() {

        if (FreeMove.looking) {
            main.TestRayOnLevelMesh();
        }

    }

    public void OnCreateMode() {

    }

    public void OnDestroy() {
        ClearAllSelectedItems();
    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {

        // Checks to see if a new column is being looked at, if so it clears the overlay for a new one to be added. Otherwise it returns.
        if (column != selectedColumn && selectedColumn != null) {

            ClearOverlay();

        } else if (column == selectedColumn) {
            return;
        }

        selectedColumn = column;
        selectedSection = section;

        if (selectedTilePreset != null) {

            InitTileOverlay(((TilePreset)selectedTilePreset).Create(false));

        }


    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

        if (selectedTilePreset == null) { return; }

        selectedColumn = column;
        selectedSection = section;

        AddTile((TilePreset)selectedTilePreset);

    }

    public void RefreshTilePlacementOverlay() {

        if (selectedColumn == null) { return; }

        ClearOverlay();

        if (selectedTilePreset != null) {

            InitTileOverlay(((TilePreset)selectedTilePreset).Create(false));

        }

    }

    void ClearOverlay() {

        if (tileOverlay != null) {
            Object.Destroy(tileOverlay.gameObject);
        }

        tileOverlay = null;

    }

    void InitTileOverlay(Tile tile) {

        var overlay = Object.Instantiate(main.SelectedTileOverlay);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.controller = main;
        script.tile = tile;
        script.column = selectedColumn;
        tileOverlay = script;
        overlay.transform.SetParent(selectedSection.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    void AddTile(TilePreset preset) {

        if (selectedColumn != null) {

            foreach (var t in selectedColumn.tiles) {
                t.isStartInColumnArray = false;
            }

            var tile = preset.Create(true);

            selectedColumn.tiles.Add(tile);

        }

        selectedSection.RefreshMesh();

    }

    void ClearAllSelectedItems() {

        ClearOverlay();

        selectedColumn = null;
        selectedSection = null;

    }

}