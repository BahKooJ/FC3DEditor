
using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class TileEditMode : EditMode {

    public Main main { get; set; }
    public TileEditPanel view;

    public TilePreset? selectedTilePreset = null;

    static public List<Tile> selectedTiles = new();
    static public TileColumn selectedColumn = null;
    static public LevelMesh selectedSection = null;
    public SelectedTileOverlay buildTileOverlay = null;
    public List<SelectedTileOverlay> selectedTileOverlays = new();
    public GameObject selectedSectionOverlay = null;

    public List<TileHeightMapChannelPoint> heightPointObjects = new();
    public TileHeightMapChannelPoint selectedHeight = null;

    public bool isBuildMode = false;

    public TileEditMode(Main main) {
        this.main = main;
    }

    public void Update() {

        if (FreeMove.looking) {
            //main.TestRayOnLevelMesh();
        }

        if (isBuildMode) {
            return;
        }

        if (Controls.OnUp("Select")) {
            selectedHeight = null;
        }

        TestTileHeightSelection();

        if (Controls.OnDown("Unselect")) {
            
            ClearAllSelectedItems();

        }
        else if (Controls.OnDown("Delete")) {
            RemoveSelectedTiles();
        }

    }

    public void OnCreateMode() {
        ReinitExistingSelectedItems();
    }

    public void OnDestroy() {

        ClearAllGameObjects();

        if (view.debugTilePanelView != null) {
            Object.Destroy(view.debugTilePanelView);
        }
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

        foreach (var obj in heightPointObjects) {
            
            Object.Destroy(obj.gameObject);

        }

        heightPointObjects.Clear();

        // Re-adds HeightMapChannelPoints (0 = top left, 1 = top right, 2 = bottom left, 3 = bottom right)
        AddHeightObjects(VertexPosition.TopLeft);
        AddHeightObjects(VertexPosition.TopRight);
        AddHeightObjects(VertexPosition.BottomLeft);
        AddHeightObjects(VertexPosition.BottomRight);

        RefeshTileOverlay();

    }

    void TestTileHeightSelection() {

        if (FreeMove.looking) {
            return;
        }

        if (!Controls.IsDown("Select")) {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

            foreach (var channel in heightPointObjects) {

                if (hit.colliderInstanceID == channel.boxCollider.GetInstanceID()) {
                    
                    if (Controls.IsDown("Select")) {

                        if (selectedHeight == null) {

                            var index = selectedTiles[0].verticies.FindIndex(vertex => {
                                return vertex.vertexPosition == channel.corner && vertex.heightChannel == channel.channel;
                            });

                            if (index != -1) {
                                selectedHeight = channel;
                            }

                            return;
                        }

                        if (channel.heightPoints != selectedHeight.heightPoints) {
                            return;
                        }

                        foreach (var tile in selectedTiles) {

                            var index = tile.verticies.FindIndex(vertex => {
                                return vertex.vertexPosition == selectedHeight.corner && vertex.heightChannel == selectedHeight.channel;
                            });

                            var existingVert = tile.verticies.FindIndex(vertex => {
                                return vertex.vertexPosition == channel.corner && vertex.heightChannel == channel.channel;
                            });

                            if (index == -1 || existingVert != -1) {
                                return;
                            }

                            var vertex = tile.verticies[index];
                            tile.verticies[index] = new TileVertex(channel.channel, vertex.vertexPosition);
                            
                        }
                        selectedHeight = channel;

                        selectedSection.RefreshMesh();
                        RefeshTileOverlay();

                    }

                }

            }


        }

    }

    void ReinitExistingSelectedItems() {

        if (selectedTiles.Count == 0) {
            return;
        }

        RefeshTileOverlay();

        selectedSectionOverlay = Object.Instantiate(main.SectionBoarders, new Vector3(selectedSection.x, 0, -selectedSection.y), Quaternion.identity);

        AddHeightObjects(VertexPosition.TopLeft);
        AddHeightObjects(VertexPosition.TopRight);
        AddHeightObjects(VertexPosition.BottomLeft);
        AddHeightObjects(VertexPosition.BottomRight);

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

        if (view.debugTilePanelView != null) {
            view.debugTilePanelView.GetComponent<ShaderDebug>().Refresh();
        }

    }

    void SelectRangeOfTiles(TileColumn oldColumn, TileColumn column) {

        if (oldColumn == null) { return; }

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

    void InitTileOverlay(Tile tile) {

        var overlay = Object.Instantiate(main.SelectedTileOverlay);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.controller = main;
        script.tile = tile;
        selectedTileOverlays.Add(script);
        overlay.transform.SetParent(selectedSection.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    void RefeshTileOverlay() {

        if (Main.debug) {
            return;
        }

        ClearTileOverlays();

        foreach (var tile in selectedTiles) {

            InitTileOverlay(tile);

        }

    }

    void ClearTileOverlays() {

        foreach (var overlay in selectedTileOverlays) {
            Object.Destroy(overlay.gameObject);
        }

        selectedTileOverlays.Clear();

    }

    void RemoveSelectedTiles() {

        if (selectedTiles.Count == 0) { return; }

        if (selectedColumn.tiles.Count == 1) {
            DialogWindowUtil.Dialog("Cannot Remove Tile", "At least one tile must be present in a tile column");
            return;
        }

        foreach (var tile in selectedTiles) {

            selectedColumn.tiles.Remove(tile);

        }

        selectedTiles.Clear();

        selectedSection.RefreshMesh();

        ClearAllSelectedItems();


    }

    void ClearAllGameObjects() {

        ClearTileOverlays();

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
        }

    }

    void ClearAllSelectedItems() {

        selectedTiles.Clear();
        ClearTileOverlays();

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        heightPointObjects.Clear();

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
            selectedSectionOverlay = null;
        }

        selectedColumn = null;
        selectedSection = null;

    }

    void AddHeightObjects(VertexPosition corner) {

        AddSingleHeightChannelObject(corner, 1, selectedColumn);
        AddSingleHeightChannelObject(corner, 2, selectedColumn);
        AddSingleHeightChannelObject(corner, 3, selectedColumn);

    }

    void AddSingleHeightChannelObject(VertexPosition corner, int channel, TileColumn column) {

        var existingHeightChannel = heightPointObjects.Find(obj => {
            return obj.heightPoints == column.heights[(int)corner - 1] && obj.channel == channel;
        });

        if (existingHeightChannel != null) {
            return;
        }

        var worldX = selectedSection.x + column.x;
        var worldY = -(selectedSection.y + column.y);

        switch (corner) {
            case VertexPosition.TopRight:
                worldX += 1;
                break;
            case VertexPosition.BottomLeft:
                worldY -= 1;
                break;
            case VertexPosition.BottomRight:
                worldX += 1;
                worldY -= 1;
                break;
            default:
                break;
        }

        var pos = new Vector3(worldX, column.heights[(int)corner - 1].GetPoint(channel), worldY);

        var point = Object.Instantiate(main.tileHeightMapChannelPoint, pos, Quaternion.identity);
        var script = point.GetComponent<TileHeightMapChannelPoint>();
        script.corner = corner;
        script.heightPoints = column.heights[(int)corner - 1];
        script.channel = channel;
        script.section = selectedSection;

        heightPointObjects.Add(script);

    }

    #region Callbacks

    public void ShiftTilesHeightUp() {

        if (selectedTiles.Count == 0) { return; }

        foreach (var tile in selectedTiles) {

            var previousVerticies = new List<TileVertex>(tile.verticies);
            var newVerticies = new HashSet<TileVertex>();

            foreach (var index in Enumerable.Range(0, tile.verticies.Count)) {

                var vertex = tile.verticies[index];

                if (vertex.heightChannel < 3) {

                    vertex.heightChannel += 1;

                    newVerticies.Add(vertex);

                }

                if (newVerticies.Count == previousVerticies.Count) {
                    tile.verticies = newVerticies.ToList();
                }

            }

        }

        selectedSection.RefreshMesh();

        RefeshTileOverlay();

    }

    public void ShiftTilesHeightDown() {

        if (selectedTiles.Count == 0) { return; }

        foreach (var tile in selectedTiles) {

            var previousVerticies = new List<TileVertex>(tile.verticies);
            var newVerticies = new HashSet<TileVertex>();

            foreach (var index in Enumerable.Range(0, tile.verticies.Count)) {

                var vertex = tile.verticies[index];

                if (vertex.heightChannel > 1) {

                    vertex.heightChannel -= 1;

                    newVerticies.Add(vertex);

                }

                if (newVerticies.Count == previousVerticies.Count) {
                    tile.verticies = newVerticies.ToList();
                }

            }

        }

        selectedSection.RefreshMesh();

        RefeshTileOverlay();

    }

    #endregion
}