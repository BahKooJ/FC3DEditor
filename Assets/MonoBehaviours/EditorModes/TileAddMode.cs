

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class TileAddMode : EditMode {

    public Main main { get; set; }

    public static TilePreset? selectedTilePreset = null;

    public TileSelection hoverSelection;

    public List<TileHeightMapChannelPoint> heightPointObjects = new();
    public SelectedTileOverlay buildTileOverlay = null;


    public TileAddMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {
        
    }

    public void OnDestroy() {

        ClearBuildingOverlay();

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        heightPointObjects.Clear();

    }

    public void Update() {

        TileSelection hover;

        if (FreeMove.looking) {
            hover = main.GetTileOnLevelMesh(false);
        } else {
            hover = main.GetTileOnLevelMesh(true);
        }

        if (hover != null && selectedTilePreset != null) {

            if (hoverSelection == null) {
                hoverSelection = hover;
                PreviewTilePlacement();
            }
            else if (hoverSelection.column != hover.column) {
                hoverSelection = hover;
                PreviewTilePlacement();
            }

        }
        else if (heightPointObjects.Count != 0) {

            ClearBuildingOverlay();

            foreach (var point in heightPointObjects) {
                Object.Destroy(point.gameObject);
            }

            heightPointObjects.Clear();

            hoverSelection = null;

        }

        if (Controls.OnDown("Select") && selectedTilePreset != null) {
            AddTile((TilePreset)selectedTilePreset);
        }

    }

    public void RefreshTilePlacementOverlay() {

        if (hoverSelection == null) { return; }

        ClearBuildingOverlay();

        if (selectedTilePreset != null) {

            InitBuildTileOverlay(((TilePreset)selectedTilePreset).Create(false, hoverSelection.column));

        }

    }

    void PreviewTilePlacement() {

        ClearBuildingOverlay();

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        heightPointObjects.Clear();

        if (selectedTilePreset != null) {

            InitBuildTileOverlay(((TilePreset)selectedTilePreset).Create(false, hoverSelection.column));

            // Re-adds HeightMapChannelPoints (0 = top left, 1 = top right, 2 = bottom left, 3 = bottom right)
            AddHeightObjects(VertexPosition.TopLeft);
            AddHeightObjects(VertexPosition.TopRight);
            AddHeightObjects(VertexPosition.BottomLeft);
            AddHeightObjects(VertexPosition.BottomRight);

        }

    }

    void ClearBuildingOverlay() {

        if (buildTileOverlay != null) {
            Object.Destroy(buildTileOverlay.gameObject);
        }

        buildTileOverlay = null;

    }

    void InitBuildTileOverlay(Tile tile) {

        var overlay = Object.Instantiate(main.SelectedTileOverlay);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.controller = main;
        script.tile = tile;
        buildTileOverlay = script;
        overlay.transform.SetParent(hoverSelection.section.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    void AddTile(TilePreset preset) {

        if (selectedTilePreset == null) { return; }

        if (hoverSelection == null) { return; }

        foreach (var t in hoverSelection.column.tiles) {

            // Checks to make sure the tile doesn't already exist
            if (MeshType.IDFromVerticies(t.verticies) == preset.meshID) {
                return;
            }

        }

        var tile = preset.Create(true, hoverSelection.column);

        hoverSelection.column.tiles.Add(tile);

        hoverSelection.section.RefreshMesh();

    }

    void AddHeightObjects(VertexPosition corner) {

        AddSingleHeightChannelObject(corner, 1, hoverSelection.column);
        AddSingleHeightChannelObject(corner, 2, hoverSelection.column);
        AddSingleHeightChannelObject(corner, 3, hoverSelection.column);

    }

    void AddSingleHeightChannelObject(VertexPosition corner, int channel, TileColumn column) {

        var existingHeightChannel = heightPointObjects.Find(obj => {
            return obj.heightPoints == column.heights[(int)corner - 1] && obj.channel == channel;
        });

        if (existingHeightChannel != null) {
            return;
        }

        var worldX = hoverSelection.section.x + column.x;
        var worldY = -(hoverSelection.section.y + column.y);

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
        script.section = hoverSelection.section;

        heightPointObjects.Add(script);

    }

}