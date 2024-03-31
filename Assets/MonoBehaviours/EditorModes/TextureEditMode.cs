using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class TextureEditMode : EditMode {

    public static bool openUVMapperByDefault = true;

    public Main main { get; set; }
    public List<Tile> selectedTiles = new();
    public TileColumn selectedColumn = null;
    public LevelMesh selectedSection = null;
    public List<TileTexturePreview> selectedTileOverlays = new();
    public GameObject selectedSectionOverlay = null;

    public TextureEditView view;

    public UVPresets currentUVPresets;

    public TextureEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {
        currentUVPresets = Presets.uvPresets;
    }

    public void OnDestroy() {
        view.CloseTextureUVMapper();
        view.CloseTexturePresetPanel();
        ClearAllSelectedItems();
    }

    public void Update() {

        if (FreeMove.looking) {
            main.TestRayOnLevelMesh();
        }

        if (Main.ignoreAllInputs) { return; }

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

        // Since frame animated tiles are a little weird it will try to only have one animated tile selected
        if (tile.GetFrameCount() > 0) {
            selectedTiles.Clear();
        }

        if (selectedTiles.Count > 0) {

            if (selectedTiles[0].GetFrameCount() > 0) {
                selectedTiles.Clear();
            }

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

                if (openUVMapperByDefault) {
                    view.OpenUVMapper();
                }

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

        if (oldColumn == null) {
            return;
        }

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

    public void DeselectAllButFirstTile() {

        if (selectedTiles.Count <= 1) { return; }

        var first = selectedTiles[0];

        selectedTiles.Clear();

        selectedTiles.Add(first);

        RefeshTileOverlay();

    }

    public void DeselectAllButLastTile() {

        if (selectedTiles.Count <= 1) { return; }

        var last = selectedTiles.Last();

        selectedTiles.Clear();

        selectedTiles.Add(last);

        RefeshTileOverlay();

    }

    void ClearAllSelectedItems() {

        if (selectedSection != null) {
            selectedSection.RefreshMesh();
        }

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
        script.section = selectedSection.section;
        selectedTileOverlays.Add(script);
        overlay.transform.SetParent(selectedSection.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    public void RefreshTileOverlayTexture() {

        foreach (var overly in selectedTileOverlays) {
            overly.Refresh();
        }

    }

    public void ReInitTileOverlayTexture() {

        foreach (var overly in selectedTileOverlays) {
            overly.Refresh(true);
        }

    }

    void ClearTileOverlays() {

        foreach (var overlay in selectedTileOverlays) {
            Object.Destroy(overlay.gameObject);
        }

        selectedTileOverlays.Clear();

    }

    public void RefreshUVMapper() {

        if (view.activeTextureUVMapper != null) {
            view.activeTextureUVMapper.GetComponent<TextureUVMapper>().RefreshView();
        }

    }

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

    public void MakeTilesOpaque() {

        foreach (var tile in selectedTiles) {
            tile.isSemiTransparent = false;
        }

        selectedSection.RefreshMesh();

    }

    public void MakeTilesTransparent() {

        foreach (var tile in selectedTiles) {
            tile.isSemiTransparent = true;
        }

        selectedSection.RefreshMesh();

    }

    // UV Presets

    public bool AddPreset() {

        if (selectedTiles.Count == 0) { return false; }

        var firstTile = selectedTiles[0];

        var potentialID = MeshType.IDFromVerticies(firstTile.verticies);

        if (potentialID == null) {
            DialogWindowUtil.Dialog("Cannot Save Preset", "Cannot save shader preset, tile mesh is invalid!");
            return false;
        }

        var uvPreset = new UVPreset(
            firstTile.uvs, 
            firstTile.texturePalette, 
            "", 
            (int)potentialID,
            firstTile.isSemiTransparent,
            firstTile.isVectorAnimated,
            firstTile.animationSpeed, 
            firstTile.animatedUVs
            );

        currentUVPresets.presets.Add(uvPreset);

        return true;

    }

    public void AddPresetsDirectory() {

        currentUVPresets.subFolders.Add(new UVPresets("", currentUVPresets));

    }

}
