
using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;

public interface EditMode {

    public Main main { get; set; }

    public void Update();

    public void OnCreateMode();

    public void OnDestroy();

    public void LookTile(Tile tile, TileColumn column, LevelMesh section);

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section);

}

public class TileMutatingEditMode {

    public HashSet<LevelMesh> selectedSections = new();
    public List<TileSelection> selectedItems = new();

    public Tile FirstTile {
        get => _FirstTile();
    }

    public bool HasSelection {
        get => _HasSelection();
    }

    public bool IsFirstTileQuad {
        get => _IsFirstTileQuad();
    }

    public void SelectRangeOfTiles(TileSelection tile) {

        //if (oldColumn == null) {
        //    return;
        //}

        //var xDif = column.x - oldColumn.x;
        //var yDif = column.y - oldColumn.y;

        //var xOrigin = oldColumn.x;
        //var yOrigin = oldColumn.y;

        //if (xDif < 0) {
        //    xOrigin = column.x;
        //}
        //if (yDif < 0) {
        //    yOrigin = column.y;
        //}

        //foreach (var y in Enumerable.Range(yOrigin, Math.Abs(yDif) + 1)) {

        //    foreach (var x in Enumerable.Range(xOrigin, Math.Abs(xDif) + 1)) {

        //        var itColumn = selectedSection.section.GetTileColumn(x, y);

        //        foreach (var itTile in itColumn.tiles) {

        //            if (selectedTiles[0].verticies.Count == itTile.verticies.Count) {

        //                SelectTile(itTile, false);

        //            }

        //        }

        //    }

        //}

    }

    public void RefreshMeshes() {

        foreach (var section in selectedSections) {
            section.RefreshMesh();
        }

    }

    public bool IsSameShape(Tile tile) {

        if (selectedItems.Count == 0) { return false; }

        return FirstTile.verticies.Count == tile.verticies.Count;

    }

    public bool IsTileAlreadySelected(Tile tile) {

        foreach (var s in selectedItems) {

            if (s.tile == tile) return true;

        }

        return false;

    }

    public void RemoveTile(Tile tile) {

        selectedItems.RemoveAll(s => s.tile == tile);

    }

    Tile _FirstTile() {

        if (selectedItems.Count == 0) { return null; }

        return selectedItems[0].tile;

    }

    bool _HasSelection() { 
        return selectedItems.Count > 0; 
    }

    bool _IsFirstTileQuad() {

        if (selectedItems.Count == 0) { return false; }

        return selectedItems[0].tile.verticies.Count == 4;

    }

}

public class TileSelection {

    public Tile tile;
    public TileColumn column;
    public LevelMesh section;

    public TileSelection(Tile tile, TileColumn column, LevelMesh section) {
        this.tile = tile;
        this.column = column;
        this.section = section;
    }

}