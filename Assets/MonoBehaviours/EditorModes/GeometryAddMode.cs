
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class GeometryAddMode : EditMode {

    public Main main { get; set; }

    public TilePreset? selectedTilePreset = null;

    public TileColumn selectedColumn = null;
    public LevelMesh selectedSection = null;
    public SelectedTileOverlay tileOverlay = null;

    public List<HeightMapChannelPoint> heightPointObjects = new();

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

            foreach (var point in heightPointObjects) {
                Object.Destroy(point.gameObject);
            }

            heightPointObjects.Clear();

        } else if (column == selectedColumn) {
            return;
        }

        selectedColumn = column;
        selectedSection = section;

        if (selectedTilePreset != null) {

            InitTileOverlay(((TilePreset)selectedTilePreset).Create(false, selectedColumn, selectedSection.section));

            // Re-adds HeightMapChannelPoints (0 = top left, 1 = top right, 2 = bottom left, 3 = bottom right)
            AddHeightObjects(VertexPosition.TopLeft);
            AddHeightObjects(VertexPosition.TopRight);
            AddHeightObjects(VertexPosition.BottomLeft);
            AddHeightObjects(VertexPosition.BottomRight);

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

            InitTileOverlay(((TilePreset)selectedTilePreset).Create(false, selectedColumn, selectedSection.section));

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
        tileOverlay = script;
        overlay.transform.SetParent(selectedSection.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    void AddTile(TilePreset preset) {

        if (selectedColumn != null) {

            foreach (var t in selectedColumn.tiles) {
                t.isEndInColumnArray = false;
            }

            var tile = preset.Create(true, selectedColumn, selectedSection.section);

            selectedColumn.tiles.Add(tile);

        }

        selectedSection.RefreshMesh();

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
        script.isStatic = true;
        script.channel = channel;
        script.corner = corner;
        script.tileColumn = column;
        script.section = selectedSection;

        heightPointObjects.Add(script);

    }

    void ClearAllSelectedItems() {

        ClearOverlay();

        selectedColumn = null;
        selectedSection = null;

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        heightPointObjects.Clear();

    }



}