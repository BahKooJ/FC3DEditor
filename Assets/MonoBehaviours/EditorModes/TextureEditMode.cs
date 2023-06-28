using FCopParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class TextureEditMode : EditMode {

    public Main main { get; set; }
    public List<Tile> selectedTiles = new();
    public TileColumn selectedColumn = null;
    public LevelMesh selectedSection = null;
    public List<SelectedTileOverlay> selectedTileOverlays = new();
    public GameObject selectedSectionOverlay = null;

    public TextureEditView view;

    public bool IsGraphicsViewOpen() {

        if (view != null) {
            return view.activeGraphicsPropertiesView != null;
        }

        return false;

    }

    public TextureEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {
        
    }

    public void OnDestroy() {
        ClearAllSelectedItems();
    }

    public void Update() {

        if (FreeMove.looking) {
            main.TestRayOnLevelMesh();
        }

    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {

    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

        var oldColumn = selectedColumn;

        // If shift is held then multiple tiles can be selected
        if (Controls.IsDown("ModifierMultiSelect")) {

            // Checks if the new selected tile is inside the selected Section, if it is not this method cannot continue.
            if (selectedSection != null) {
                if (selectedSection != section) {
                    return;
                }
            }

        }
        else {

            // Clears the selected tile(s).
            selectedTiles.Clear();

        }

        // Updates the remaining data
        selectedColumn = column;
        selectedSection = section;

        // Selects a range of tiles if the mutli-select modifier is held.
        if (Controls.IsDown("ModifierAltSelect") && Controls.IsDown("ModifierMultiSelect")) {

            SelectRangeOfTiles(oldColumn, column);

        }

        // Checks to see if the tiles vertex count is the same as the first selected tile
        // This needs to be done because there are many differences in triangle tiles and rect tiles 
        else if (selectedTiles.Count == 0) {

            SelectTile(tile);

        }
        else if (selectedTiles[0].verticies.Count == tile.verticies.Count) {

            SelectTile(tile);

        }
        else {
            return;
        }

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
        }

        selectedSectionOverlay = Object.Instantiate(main.SectionBoarders, new Vector3(section.x, 0, -section.y), Quaternion.identity);

        RefeshTileOverlay();

    }

    void SelectTile(Tile tile, bool deSelectDuplicate = true) {

        if (selectedTiles.Contains(tile)) {

            if (deSelectDuplicate) {
                selectedTiles.Remove(tile);
                RefeshTileOverlay();
            }

        }
        else {

            selectedTiles.Add(tile);

        }

    }

    void SelectRangeOfTiles(TileColumn oldColumn, TileColumn column) {

        var xDif = column.x - oldColumn.x;
        var yDif = column.y - oldColumn.y;

        var xOrigin = oldColumn.x;
        var yOrigin = oldColumn.y;

        if (xDif < 0) {
            xOrigin = column.x;
        }
        if (yDif < 0) {
            yOrigin = column.y;
        }

        foreach (var y in Enumerable.Range(yOrigin, Math.Abs(yDif) + 1)) {

            foreach (var x in Enumerable.Range(xOrigin, Math.Abs(xDif) + 1)) {

                var itColumn = selectedSection.section.GetTileColumn(x, y);

                foreach (var itTile in itColumn.tiles) {

                    if (selectedTiles[0].verticies.Count == itTile.verticies.Count) {

                        SelectTile(itTile, false);

                    }

                }

            }

        }

    }

    void ClearAllSelectedItems() {

        selectedTiles.Clear();
        ClearTileOverlays();

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
            selectedSectionOverlay = null;
        }

        selectedColumn = null;
        selectedSection = null;

    }

    void RefeshTileOverlay() {

        ClearTileOverlays();

        foreach (var tile in selectedTiles) {

            InitTileOverlay(tile);

        }

    }

    void InitTileOverlay(Tile tile) {

        var overlay = Object.Instantiate(main.SelectedTileOverlay);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.controller = main;
        script.tile = tile;
        selectedTileOverlays.Add(script);
        overlay.transform.SetParent(selectedSection.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    void ClearTileOverlays() {

        foreach (var overlay in selectedTileOverlays) {
            Object.Destroy(overlay.gameObject);
        }

        selectedTileOverlays.Clear();

    }

    public void ExportTexture(int id) {

        if (id == -1) { return; }

        var graphics = selectedSection.section.tileGraphics[id];

        File.WriteAllBytes("bmp" + graphics.number2.ToString() + ".bmp", main.level.textures[graphics.number2].BitmapWithHeader());

    }

    // TODO: Allow importing textures even when multiple tiles are selected
    public void ImportTexture(int id) {

        if (id == -1) { return; }

        var graphics = selectedSection.section.tileGraphics[selectedTiles[0].graphicsIndex];

        main.level.textures[graphics.number2].ImportBMP(File.ReadAllBytes("bmp" + graphics.number2.ToString() + ".bmp"));

    }

    public void ChangeTexturePallette(int palletteOffset) {

        foreach (var tile in selectedTiles) {
            var graphics = selectedSection.section.tileGraphics[tile.graphicsIndex];

            graphics.number2 = palletteOffset;

            selectedSection.section.tileGraphics[tile.graphicsIndex] = graphics;

        }

    }

    public void SetTextureIndex(int index) {

        foreach (var tile in selectedTiles) {
            tile.textureIndex = index;
        }

    }

}
