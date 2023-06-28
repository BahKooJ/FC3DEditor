
using FCopParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class GeometryEditMode : EditMode {

    public Main main { get; set; }

    public List<Tile> selectedTiles = new();
    public TileColumn selectedColumn = null;
    public LevelMesh selectedSection = null;
    public List<HeightMapChannelPoint> heightPointObjects = new();
    public List<SelectedTileOverlay> selectedTileOverlays = new();
    public GameObject selectedSectionOverlay = null;

    public GeometryEditorUI view;

    public HeightMapChannelPoint lastSelectedHeightChannel = null;

    public void Update() {
        
        if (FreeMove.looking) {
            main.TestRayOnLevelMesh();
        }

        TestHeightMapChannelSelection();

        if (Controls.OnDown("Unselect")) {

            var wasSelectedHeights = false;

            foreach (var height in heightPointObjects) {

                if (height.isSelected) {
                    wasSelectedHeights = true;
                    height.DeSelect();
                }

            }

            if (!wasSelectedHeights) {
                ClearAllSelectedItems();
            }

        } 
        else if (Controls.OnDown("Delete")) {
            RemoveSelectedTiles();
        }


    }

    public void OnCreateMode() {

    }

    public void OnDestroy() {
        ClearAllSelectedItems();
    }

    public GeometryEditMode(Main main) {
        this.main = main;
    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {
        // TODO: Add overlay when looking at tile
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

        } else {

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

        } else if (selectedTiles[0].verticies.Count == tile.verticies.Count) {

            SelectTile(tile);

        } else {
            return;
        }

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
        }

        selectedSectionOverlay = Object.Instantiate(main.SectionBoarders, new Vector3(section.x, 0, -section.y), Quaternion.identity);

        // Removes only non selected HeightMapChannelPoints.
        var oldPoints = new List<HeightMapChannelPoint>(heightPointObjects);

        heightPointObjects.Clear();

        foreach (var obj in oldPoints) {

            if (!obj.isSelected) {
                Object.Destroy(obj.gameObject);
            } else {
                heightPointObjects.Add(obj);
            }

        }

        // Re-adds HeightMapChannelPoints (0 = top left, 1 = top right, 2 = bottom left, 3 = bottom right)
        AddHeightObjects(VertexPosition.TopLeft);
        AddHeightObjects(VertexPosition.TopRight);
        AddHeightObjects(VertexPosition.BottomLeft);
        AddHeightObjects(VertexPosition.BottomRight);


        // If the number of the height channel is held down, all HeightMapChannelPoints will be selected in the column.
        if (Controls.IsDown("ModifierChannelSelect1")) {

            foreach (var obj in heightPointObjects) {

                if (obj.channel == 1) {
                    obj.Select();
                }

            }

        } 
        else if (Controls.IsDown("ModifierChannelSelect2")) {

            foreach (var obj in heightPointObjects) {

                if (obj.channel == 2) {
                    obj.Select();
                }

            }

        } 
        else if (Controls.IsDown("ModifierChannelSelect3")) {

            foreach (var obj in heightPointObjects) {

                if (obj.channel == 3) {
                    obj.Select();
                }

            }

        }

        RefeshTileOverlay();

    }

    #region Local Methods

    void TestHeightMapChannelSelection() {

        if (FreeMove.looking) {
            return;
        }

        if (!Controls.OnDown("Select") && !Controls.OnDown("Interact") && Input.GetAxis("Mouse ScrollWheel") == 0) {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

            foreach (var channel in heightPointObjects) {

                if (hit.colliderInstanceID == channel.boxCollider.GetInstanceID()) {

                    if (Controls.OnDown("Select") && Controls.IsDown("ModifierMultiSelect")) {

                        if (Controls.IsDown("ModifierAltSelect")) {
                            SelectAllHeightChannelsInSection(channel.channel);
                            return;
                        } else {
                            channel.SelectOrDeSelect();
                        }

                    } else if (Controls.OnDown("Select")) {

                        channel.Click();

                    }

                    channel.MoveTileChannelUpOrDown();

                    if (Controls.OnDown("Interact")) {

                        channel.ChangeExactHeight();

                    }
                
                }

            }


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

    // Maybe not needed
    void SelectRangeOfHeightChannels(HeightMapChannelPoint heightPoint) {

        if (lastSelectedHeightChannel == null) {
            return;
        }

        var xDif = heightPoint.tileColumn.x - lastSelectedHeightChannel.tileColumn.x;
        var yDif = heightPoint.tileColumn.y - lastSelectedHeightChannel.tileColumn.y;

        var xOrigin = lastSelectedHeightChannel.tileColumn.x;
        var yOrigin = lastSelectedHeightChannel.tileColumn.y;

        if (xDif < 0) {
            xOrigin = heightPoint.tileColumn.x;
        }
        if (yDif < 0) {
            yOrigin = heightPoint.tileColumn.y;
        }

        foreach (var y in Enumerable.Range(yOrigin, Math.Abs(yDif) + 1)) {

            foreach (var x in Enumerable.Range(xOrigin, Math.Abs(xDif) + 1)) {

                var itColumn = selectedSection.section.GetTileColumn(x, y);



            }

        }

    }

    void SelectAllHeightChannelsInSection(int channel) {

        foreach (var y in Enumerable.Range(0, 16)) {

            foreach (var x in Enumerable.Range(0, 16)) {

                var itColumn = selectedSection.section.GetTileColumn(x, y);

                AddSingleHeightChannelObject(VertexPosition.TopLeft, channel, itColumn);
                AddSingleHeightChannelObject(VertexPosition.TopRight, channel, itColumn);
                AddSingleHeightChannelObject(VertexPosition.BottomLeft, channel, itColumn);
                AddSingleHeightChannelObject(VertexPosition.BottomRight, channel, itColumn);


            }

        }

        foreach (var heightObj in heightPointObjects) {

            if (heightObj.channel == channel) {
                heightObj.Select();
            }

        }

    }

    void ClearAllSelectedItems() {

        selectedTiles.Clear();
        ClearTileOverlays();

        foreach (var obj in heightPointObjects) {

            Object.Destroy(obj.gameObject);

        }

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
            selectedSectionOverlay = null;
        }


        heightPointObjects.Clear();

        selectedColumn = null;
        selectedSection = null;

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

        var point = Object.Instantiate(main.heightMapChannelPoint, pos, Quaternion.identity);
        var script = point.GetComponent<HeightMapChannelPoint>();
        script.heightPoints = column.heights[(int)corner - 1];
        script.controller = this;
        script.channel = channel;
        script.corner = corner;
        script.tileColumn = column;
        script.section = selectedSection;

        heightPointObjects.Add(script);

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

        ClearTileOverlays();

        foreach (var tile in selectedTiles) {

            InitTileOverlay(tile);

        }

    }

    void SelectTile(Tile tile, bool deSelectDuplicate = true) {

        if (selectedTiles.Contains(tile)) {

            if (deSelectDuplicate) {
                selectedTiles.Remove(tile);
                RefeshTileOverlay();
            }

        } else {

            selectedTiles.Add(tile);

        }

    }

    #endregion

    #region Public and Callback Methods

    public void MoveAllHeights(float distance) {

        foreach (var heightObj in heightPointObjects) {

            if (heightObj.isSelected) {
                heightObj.MoveHeight(distance);
            }

        }

    }

    public void UnselectAndRefreshHeightPoints() {

        foreach (var obj in heightPointObjects) {

            Object.Destroy(obj.gameObject);

        }

        heightPointObjects.Clear();

        AddHeightObjects(VertexPosition.TopLeft);
        AddHeightObjects(VertexPosition.TopRight);
        AddHeightObjects(VertexPosition.BottomLeft);
        AddHeightObjects(VertexPosition.BottomRight);

    }

    public void RefreshSelectedOverlays() {

        foreach (var overlay in selectedTileOverlays) {

            overlay.Refresh();

        }

    }

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

        RefreshSelectedOverlays();

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

        RefreshSelectedOverlays();

    }

    public void DuplicateTileGraphics() {

        if (selectedTiles.Count < 2) { return; }

        var firstTile = selectedTiles[0];

        foreach (var tile in selectedTiles) {

            tile.textureIndex = firstTile.textureIndex;
            tile.graphicsIndex = firstTile.graphicsIndex;

        }

        selectedSection.RefreshMesh();

        ClearAllSelectedItems();

    }

    #endregion

}