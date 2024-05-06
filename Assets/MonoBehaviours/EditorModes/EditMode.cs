
using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;

public interface EditMode {

    public Main main { get; set; }

    public void Update();

    public void OnCreateMode();

    public void OnDestroy();

}

public abstract class TileMutatingEditMode : EditMode {

    public HashSet<LevelMesh> selectedSections = new();
    public List<TileSelection> selectedItems = new();

    public Tile FirstTile {
        get => _FirstTile();
    }

    public TileSelection FirstItem {
        get => _FristItem();
    }

    public bool HasSelection {
        get => _HasSelection();
    }

    public bool IsFirstTileQuad {
        get => _IsFirstTileQuad();
    }
    public abstract Main main { get; set; }

    virtual public void MakeSelection(TileSelection selection, bool deSelectDuplicate = true) { }

    public void SelectRangeOfTiles(TileSelection tile) {

        var firstSelection = selectedItems[0];

        var firstClickColumnX = firstSelection.column.x;
        var firstClickColumnY = firstSelection.column.y;

        var lastClickColumnX = tile.column.x;
        var lastClickColumnY = tile.column.y;

        var sameMesh = firstSelection.tile.verticies.SequenceEqual(tile.tile.verticies);

        if (firstSelection.section == tile.section) {

            var startX = firstClickColumnX < lastClickColumnX ? firstClickColumnX : lastClickColumnX;
            var startY = firstClickColumnY < lastClickColumnY ? firstClickColumnY : lastClickColumnY;

            var endX = firstClickColumnX > lastClickColumnX ? firstClickColumnX : lastClickColumnX;
            var endY = firstClickColumnY > lastClickColumnY ? firstClickColumnY : lastClickColumnY;

            foreach (var y in Enumerable.Range(startY, endY - startY + 1)) {

                foreach (var x in Enumerable.Range(startX, endX - startX + 1)) {

                    var itColumn = tile.section.section.GetTileColumn(x, y);

                    foreach (var itTile in itColumn.tiles) {

                        if (sameMesh) {

                            if (itTile.verticies.SequenceEqual(firstSelection.tile.verticies)) {

                                MakeSelection(new TileSelection(itTile, itColumn, tile.section), false);

                            }

                        }
                        else {

                            MakeSelection(new TileSelection(itTile, itColumn, tile.section), false);

                        }

                    }

                }

            }

        }
        else {

            var firstClickColumnSectionX = firstSelection.column.x + firstSelection.section.arrayX * 16;
            var firstClickColumnSectionY = firstSelection.column.y + firstSelection.section.arrayY * 16;

            var lastClickColumnSectionX = tile.column.x + tile.section.arrayX * 16;
            var lastClickColumnSectionY = tile.column.y + tile.section.arrayY * 16;

            var startSectionX = firstClickColumnSectionX < lastClickColumnSectionX ? firstClickColumnSectionX : lastClickColumnSectionX;
            var startSectionY = firstClickColumnSectionY < lastClickColumnSectionY ? firstClickColumnSectionY : lastClickColumnSectionY;

            var endSectionX = firstClickColumnSectionX > lastClickColumnSectionX ? firstClickColumnSectionX : lastClickColumnSectionX;
            var endSectionY = firstClickColumnSectionY > lastClickColumnSectionY ? firstClickColumnSectionY : lastClickColumnSectionY;

            foreach (var y in Enumerable.Range(startSectionY, endSectionY - startSectionY + 1)) {

                foreach (var x in Enumerable.Range(startSectionX, endSectionX - startSectionX + 1)) {

                    var itSection = main.GetLevelMesh(x / 16, y / 16);
                    var itColumn = itSection.section.GetTileColumn(x % 16, y % 16);

                    foreach (var itTile in itColumn.tiles) {

                        if (sameMesh) {

                            if (itTile.verticies.SequenceEqual(firstSelection.tile.verticies)) {

                                MakeSelection(new TileSelection(itTile, itColumn, itSection), false);

                            }

                        }
                        else {

                            MakeSelection(new TileSelection(itTile, itColumn, itSection), false);

                        }

                    }

                }

            }

        }



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

    TileSelection _FristItem() {

        if (selectedItems.Count == 0) { return null; }

        return selectedItems[0];

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

    public abstract void Update();
    public abstract void OnCreateMode();
    public abstract void OnDestroy();

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