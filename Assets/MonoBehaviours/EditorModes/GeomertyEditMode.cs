
using FCopParser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GeometryEditMode : EditMode {

    public Main main { get; set; }

    public List<Tile> selectedTiles = new();
    public TileColumn selectedColumn = null;
    public LevelMesh selectedSection = null;
    public List<HeightMapChannelPoint> heightPointObjects = new();
    public List<SelectedTileOverlay> selectedTileOverlays = new();
    public GameObject selectedSectionOverlay = null;

    public GeometryEditorUI view;

    public bool IsGraphicsViewOpen() {
        return view.activeGraphicsPropertiesView != null;
    }

    FCopLevelSection tempCopySection = null;

    public void Update() {
        
        if (FreeMove.looking) {
            main.TestRayOnLevelMesh();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            ClearAllSelectedItems();
        } else if (Input.GetKeyDown(KeyCode.Delete)) {
            RemoveSelectedTiles();
        }

        if (Input.GetKeyDown(KeyCode.U)) {

            if (selectedSection != null) {
                selectedSection.section.RotateCounterClockwise();
                selectedSection.RefreshMesh();
            }

        }

        if (Input.GetKeyDown(KeyCode.I)) {

            if (selectedSection != null) {
                selectedSection.section.MirorVertically();
                selectedSection.RefreshMesh();
            }

        }

        if (Input.GetKeyDown(KeyCode.O)) {
            tempCopySection = selectedSection.section;
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            selectedSection.section.Overwrite(tempCopySection);
            selectedSection.RefreshMesh();
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

        // If shift is held then multiple tiles can be selected
        if (Input.GetKey(KeyCode.LeftShift)) {

            // Checks if the new selected tile is inside the selected Section, if it is not this method cannot continue.
            if (selectedSection != null) {
                if (selectedSection != section) {
                    return;
                }
            }

        } else {

            // Clears the selected tile(s).
            selectedTiles.Clear();

            ClearTileOverlays();

        }

        // Adds selected tile and updates remaining selected data
        selectedTiles.Add(tile);
        selectedColumn = column;
        selectedSection = section;

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
        AddHeightObjects(0);
        AddHeightObjects(1);
        AddHeightObjects(2);
        AddHeightObjects(3);


        // If the number of the height channel is held down, all HeightMapChannelPoints will be selected in the column.
        if (Input.GetKey(KeyCode.Alpha1)) {

            foreach (var obj in heightPointObjects) {

                if (obj.channel == 1) {
                    obj.Select();
                }

            }

        } else if (Input.GetKey(KeyCode.Alpha2)) {

            foreach (var obj in heightPointObjects) {

                if (obj.channel == 2) {
                    obj.Select();
                }

            }

        } else if (Input.GetKey(KeyCode.Alpha3)) {

            foreach (var obj in heightPointObjects) {

                if (obj.channel == 3) {
                    obj.Select();
                }

            }

        }

        InitTileOverlay(tile);

    }

    public void UnselectAndRefreshHeightPoints() {

        foreach (var obj in heightPointObjects) {

            Object.Destroy(obj.gameObject);

        }

        heightPointObjects.Clear();

        AddHeightObjects(0);
        AddHeightObjects(1);
        AddHeightObjects(2);
        AddHeightObjects(3);

    }

    public void RefreshSelectedOverlays() {

        foreach (var overlay in selectedTileOverlays) {

            overlay.Refresh();

        }

    }

    public void RotateTileLeft() {

        if (selectedTiles.Count == 0) { return; }

        foreach (var tile in selectedTiles) {

            if (tile.verticies.Count == 3) {

                var verticies = tile.verticies;

                bool isBottomRight =
                    (verticies[0].vertexPosition == VertexPosition.TopRight) &&
                    (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
                    (verticies[2].vertexPosition == VertexPosition.BottomRight);

                bool isBottomLeft =
                    (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
                    (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
                    (verticies[2].vertexPosition == VertexPosition.BottomRight);

                bool isTopLeft =
                    (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
                    (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
                    (verticies[2].vertexPosition == VertexPosition.TopRight);

                bool isTopRight =
                    (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
                    (verticies[1].vertexPosition == VertexPosition.BottomRight) &&
                    (verticies[2].vertexPosition == VertexPosition.TopRight);

                if (isBottomRight) {
                    verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
                    verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
                    verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.BottomRight);
                } else if (isBottomLeft) {
                    verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
                    verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
                    verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.TopRight);
                } else if (isTopLeft) {
                    verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
                    verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomRight);
                    verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.TopRight);
                } else if (isTopRight) {
                    verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopRight);
                    verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
                    verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.BottomRight);
                }

            }

        }

        selectedSection.RefreshMesh();

    }

    public void RotateTileRight() {

        if (selectedTiles.Count == 0) { return; }

        foreach (var tile in selectedTiles) {

            if (tile.verticies.Count == 3) {

                var verticies = tile.verticies;

                bool isBottomRight =
                    (verticies[0].vertexPosition == VertexPosition.TopRight) &&
                    (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
                    (verticies[2].vertexPosition == VertexPosition.BottomRight);

                bool isBottomLeft =
                    (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
                    (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
                    (verticies[2].vertexPosition == VertexPosition.BottomRight);

                bool isTopLeft =
                    (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
                    (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
                    (verticies[2].vertexPosition == VertexPosition.TopRight);

                bool isTopRight =
                    (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
                    (verticies[1].vertexPosition == VertexPosition.BottomRight) &&
                    (verticies[2].vertexPosition == VertexPosition.TopRight);

                if (isBottomRight) {
                    verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
                    verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
                    verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.TopRight);
                } else if (isBottomLeft) {
                    verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
                    verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
                    verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.TopRight);
                } else if (isTopLeft) {
                    verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
                    verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
                    verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.BottomRight);
                } else if (isTopRight) {
                    verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopRight);
                    verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
                    verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.BottomRight);
                }

            }

        }

        selectedSection.RefreshMesh();

    }

    // TODO: If shifting height for wall to high will cause MeshID error
    public void ShiftTilesHeightUp() {

        if (selectedTiles.Count == 0) { return; }

        foreach (var tile in selectedTiles) {

            foreach (var index in Enumerable.Range(0, tile.verticies.Count)) {

                var vertex = tile.verticies[index];

                if (vertex.heightChannel < 3) {
                    tile.verticies[index] = new TileVertex(vertex.heightChannel + 1, vertex.vertexPosition);
                }

            }

        }

        selectedSection.RefreshMesh();

        RefreshSelectedOverlays();

    }

    // TODO: If shifting height for wall to low will cause MeshID error
    public void ShiftTilesHeightDown() {

        if (selectedTiles.Count == 0) { return; }

        foreach (var tile in selectedTiles) {

            foreach (var index in Enumerable.Range(0, tile.verticies.Count)) {

                var vertex = tile.verticies[index];

                if (vertex.heightChannel > 1) {
                    tile.verticies[index] = new TileVertex(vertex.heightChannel - 1, vertex.vertexPosition);
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

        foreach (var tile in selectedTiles) {

            selectedColumn.tiles.Remove(tile);

        }

        selectedTiles.Clear();

        selectedSection.RefreshMesh();

        ClearAllSelectedItems();


    }

    void AddHeightObjects(int corner) {

        var worldX = selectedSection.x + selectedColumn.x;
        var worldY = -(selectedSection.y + selectedColumn.y);

        switch (corner) {
            case 1:
                worldX += 1;
                break;
            case 2:
                worldY -= 1;
                break;
            case 3:
                worldX += 1;
                worldY -= 1;
                break;
            default:
                break;
        }

        var point = Object.Instantiate(main.heightMapChannelPoint, new Vector3(worldX, selectedColumn.heights[corner].height1, worldY), Quaternion.identity);
        var script = point.GetComponent<HeightMapChannelPoint>();
        script.heightPoint = selectedColumn.heights[corner];
        script.controller = this;
        script.channel = 1;
        script.cornerWhenSelected = corner + 1;
        script.section = selectedSection;

        heightPointObjects.Add(script);

        point = Object.Instantiate(main.heightMapChannelPoint, new Vector3(worldX, selectedColumn.heights[corner].height2, worldY), Quaternion.identity);
        script = point.GetComponent<HeightMapChannelPoint>();
        script.heightPoint = selectedColumn.heights[corner];
        script.controller = this;
        script.channel = 2;
        script.cornerWhenSelected = corner + 1;
        script.section = selectedSection;

        heightPointObjects.Add(script);

        point = Object.Instantiate(main.heightMapChannelPoint, new Vector3(worldX, selectedColumn.heights[corner].height3, worldY), Quaternion.identity);
        script = point.GetComponent<HeightMapChannelPoint>();
        script.heightPoint = selectedColumn.heights[corner];
        script.controller = this;
        script.channel = 3;
        script.cornerWhenSelected = corner + 1;
        script.section = selectedSection;

        heightPointObjects.Add(script);

    }

    void InitTileOverlay(Tile tile) {

        var overlay = Object.Instantiate(main.SelectedTileOverlay);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.controller = main;
        script.tile = tile;
        script.column = selectedColumn;
        selectedTileOverlays.Add(script);
        overlay.transform.SetParent(selectedSection.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

}