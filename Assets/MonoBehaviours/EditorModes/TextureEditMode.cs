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
    public List<TileTexturePreview> selectedTileOverlays = new();
    public GameObject selectedSectionOverlay = null;

    public TextureEditView view;

    public TextureEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {
        
    }

    public void OnDestroy() {
        view.CloseTextureUVMapper();
        ClearAllSelectedItems();
    }

    public void Update() {

        if (FreeMove.looking) {
            main.TestRayOnLevelMesh();
        }
        if (Controls.OnDown("Unselect")) {
            
            if (view.activeTextureUVMapper != null) {
                view.CloseTextureUVMapper();
            }

            ClearAllSelectedItems();
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

            if (view.activeTextureUVMapper != null) {
                view.activeTextureUVMapper.GetComponent<TextureUVMapper>().RefreshView();
            } else {
                view.OpenUVMapper();
            }

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

                if (selectedTiles.Count == 0) {

                    if (view.activeTextureUVMapper != null) {
                        view.CloseTextureUVMapper();
                    }

                    ClearAllSelectedItems();

                }

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

        var overlay = Object.Instantiate(main.TileTexturePreview);
        var script = overlay.GetComponent<TileTexturePreview>();
        script.controller = main;
        script.tile = tile;
        selectedTileOverlays.Add(script);
        overlay.transform.SetParent(selectedSection.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    public void RefreshTileOverlayTexture() {

        foreach (var overly in selectedTileOverlays) {
            overly.Refresh();
        }

    }

    void ClearTileOverlays() {

        foreach (var overlay in selectedTileOverlays) {
            Object.Destroy(overlay.gameObject);
        }

        selectedTileOverlays.Clear();

    }

    void RefreshUVMapper() {

        if (view.activeTextureUVMapper != null) {
            view.activeTextureUVMapper.GetComponent<TextureUVMapper>().RefreshView();
        }

    }

    // TODO: Importing and exporting textures, remember that FCopTexture.ImportBMP exists

    public void ChangeTexturePallette(int palletteOffset) {

        foreach (var tile in selectedTiles) {

            tile.texturePalette = palletteOffset;

        }

    }

    public void DuplicateTileGraphics() {

        if (selectedTiles.Count < 2) return;

        var firstTile = selectedTiles[0];

        foreach (var tile in selectedTiles.Skip(1)) {

            tile.uvs = new List<int>(firstTile.uvs);
            tile.texturePalette = firstTile.texturePalette;

        }

        selectedSection.RefreshMesh();

        RefreshUVMapper();
        RefreshTileOverlayTexture();

    }

}
