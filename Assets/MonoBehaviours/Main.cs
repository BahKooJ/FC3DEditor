using FCopParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

// This object is responsible for handling all callbacks from views as well as handle the IFF file.
public class Main : MonoBehaviour {

    public GameObject meshSection;
    public GameObject heightMapChannelPoint;
    public GameObject SelectedTileOverlay;
    public GameObject NavMeshPoint;
    public GameObject line3d;
    public GameObject axisControl;

    IFFParser iffFile = new(File.ReadAllBytes("C:/Users/Zewy/Desktop/Mp"));
    public FCopLevel level;

    public List<LevelMesh> sectionMeshes = new();

    public Texture2D levelTexturePallet;

    public List<Tile> selectedTiles = new();
    public TileColumn selectedColumn = null;
    public LevelMesh selectedSection = null;
    public List<HeightMapChannelPoint> heightPointObjects = new();
    public List<SelectedTileOverlay> selectedTileOverlays = new();

    public bool debug = false;

    public EditMode editMode;

    void Start() {

        Application.targetFrameRate = 60;

        level = new FCopLevel(iffFile.parsedData);

        RefreshTextures();

        RenderFullMap();

    }

    void Update() {

        editMode.Update();

        if (Input.GetKeyDown(KeyCode.Equals)) {
            Compile();
        } else if (Input.GetKeyDown(KeyCode.C)) {
            ClearAllSelectedItems();
        } else if (Input.GetKeyDown(KeyCode.Delete)) {
            RemoveSelectedTiles();
        }
    }

    public void TestRayOnLevelMesh() {

        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1)) {

            foreach (var section in sectionMeshes) {

                if (hit.colliderInstanceID == section.meshCollider.GetInstanceID()) {

                    int clickX = (int)Math.Floor(hit.point.x - section.x);
                    int clickY = (int)Math.Floor(Math.Abs(hit.point.z + section.y));

                    var column = section.section.tileColumns.First(tileColumn => {
                        return tileColumn.x == clickX && tileColumn.y == clickY;
                    });

                    if (Input.GetMouseButtonDown(0)) {

                        SelectTile(section.sortedTilesByTriangle[hit.triangleIndex], column, section);

                        section.RefreshMesh();

                    } else {

                        LookTile(section.sortedTilesByTriangle[hit.triangleIndex], column, section);

                    }

                }


            }

        }

    }

    public Vector3? CursorOnLevelMesh() {

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1)) {

            foreach (var section in sectionMeshes) {

                if (hit.colliderInstanceID == section.meshCollider.GetInstanceID()) {

                    int clickX = (int)Math.Floor(hit.point.x - section.x);
                    int clickY = (int)Math.Floor(Math.Abs(hit.point.z + section.y));

                    var column = section.section.tileColumns.First(tileColumn => {
                        return tileColumn.x == clickX && tileColumn.y == clickY;
                    });

                    return hit.point;

                }


            }

        }

        return null;

    }

    public void Compile() {

        level.Compile();

        var index = iffFile.parsedData.files.FindIndex(file => {

            return file.dataFourCC == "Ctil";

        });

        iffFile.parsedData.files.RemoveAll(file => {

            return file.dataFourCC == "Ctil";

        });

        foreach (var section in level.sections) {
            iffFile.parsedData.files.Insert(index, section.parser.rawFile);
            index++;
        }

        iffFile.Compile();

        File.WriteAllBytes("Mp MOD", iffFile.bytes);

    }

    public void ChangeEditMode(EditMode mode) {

        if (editMode != null) {
            editMode.OnDestroy();
        }

        ClearAllSelectedItems();
        editMode = mode;
        editMode.OnCreateMode();

    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {
        editMode.LookTile(tile, column, section);
    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

        editMode.SelectTile(tile, column, section);

    }

    public void UnselectAndRefreshHeightPoints() {

        foreach (var obj in heightPointObjects) {

            Destroy(obj.gameObject);

        }

        heightPointObjects.Clear();

        AddHeightObjects(0);
        AddHeightObjects(1);
        AddHeightObjects(2);
        AddHeightObjects(3);

    }

    public void ClearAllSelectedItems() {

        selectedTiles.Clear();
        ClearTileOverlays();

        foreach (var obj in heightPointObjects) {

            Destroy(obj.gameObject);

        }

        heightPointObjects.Clear();

        selectedColumn = null;
        selectedSection = null;

    }

    public void AddTile(TilePreset preset) {

        if (selectedColumn != null) {

            foreach (var t in selectedColumn.tiles) {
                t.isStartInColumnArray = false;
            }

            var tile = preset.Create(true);

            selectedColumn.tiles.Add(tile);

            selectedTiles.Clear();

            selectedTiles.Add(tile);

        }

        selectedSection.RefreshMesh();

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

    }

    public void ExportTexture(int id) {

        if (id == -1) { return; }

        var graphics = selectedSection.section.tileGraphics[id];

        File.WriteAllBytes("bmp" + graphics.number2.ToString() + ".bmp", level.textures[graphics.number2].BitmapWithHeader());

    }

    // TODO: Allow importing textures even when multiple tiles are selected
    public void ImportTexture(int id) {

        if (id == -1) { return; }

        var graphics = selectedSection.section.tileGraphics[selectedTiles[0].graphicsIndex];

        level.textures[graphics.number2].ImportBMP(File.ReadAllBytes("bmp" + graphics.number2.ToString() + ".bmp"));

    }

    public void ChangeTexturePallette(int palletteOffset) {

        foreach (var tile in selectedTiles) {
            var graphics = selectedSection.section.tileGraphics[tile.graphicsIndex];

            graphics.number2 = palletteOffset;

            selectedSection.section.tileGraphics[tile.graphicsIndex] = graphics;

        }

    }

    public void SetTextureCordX(int x) {

        if (selectedTiles.Count == 0) { return; }

        if (selectedTiles.Count > 1) {

            var textureIndex = selectedTiles[0].textureIndex;
            foreach (var tile in selectedTiles) {

                if (textureIndex != tile.textureIndex) {
                    return;
                }

            }

        }

        var index = selectedTiles[0].textureIndex;

        selectedSection.section.textureCoordinates[index] = TextureCoordinate.SetXPixel(x, selectedSection.section.textureCoordinates[index]);

    }

    public void SetTextureCordY(int y) {

        if (selectedTiles.Count == 0) { return; }

        if (selectedTiles.Count > 1) {

            var textureIndex = selectedTiles[0].textureIndex;
            foreach (var tile in selectedTiles) {

                if (textureIndex != tile.textureIndex) {
                    return;
                }

            }

        }

        var index = selectedTiles[0].textureIndex;

        selectedSection.section.textureCoordinates[index] = TextureCoordinate.SetYPixel(y, selectedSection.section.textureCoordinates[index]);

    }

    public void SetTextureIndex(int index) {

        foreach (var tile in selectedTiles) {
            tile.textureIndex = index;
        }

    }

    public void RefreshTextures() {

        var texture = new Texture2D(256, 2048, TextureFormat.RGB565, false);
        var texturePallet = new List<byte>();

        texturePallet.AddRange(level.textures[0].ConvertToRGB565());
        texturePallet.AddRange(level.textures[1].ConvertToRGB565());
        texturePallet.AddRange(level.textures[2].ConvertToRGB565());
        texturePallet.AddRange(level.textures[3].ConvertToRGB565());
        texturePallet.AddRange(level.textures[4].ConvertToRGB565());
        texturePallet.AddRange(level.textures[5].ConvertToRGB565());
        texturePallet.AddRange(level.textures[6].ConvertToRGB565());
        texturePallet.AddRange(level.textures[7].ConvertToRGB565());

        texture.LoadRawTextureData(texturePallet.ToArray());
        texture.Apply();

        levelTexturePallet = texture;

    }

    public void RemoveSelectedTiles() {

        if (selectedTiles.Count == 0) { return; }

        foreach (var tile in selectedTiles) {
            
            selectedColumn.tiles.Remove(tile);

        }

        selectedTiles.Clear();

        selectedSection.RefreshMesh();

    }

    public void RefreshSelectedOverlays() {

        foreach (var overlay in selectedTileOverlays) {

            overlay.Refresh();

        }

    }

    public void InitTileOverlay(Tile tile) {

        var overlay = Instantiate(SelectedTileOverlay);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.controller = this;
        script.tile = tile;
        script.column = selectedColumn;
        selectedTileOverlays.Add(script);
        overlay.transform.SetParent(selectedSection.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    public void ClearTileOverlays() {

        foreach (var overlay in selectedTileOverlays) {
            Destroy(overlay.gameObject);
        }

        selectedTileOverlays.Clear();

    }

    public void AddHeightObjects(int corner) {

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

        var point = Instantiate(heightMapChannelPoint, new Vector3(worldX, selectedColumn.heights[corner].height1, worldY), Quaternion.identity);
        var script = point.GetComponent<HeightMapChannelPoint>();
        script.heightPoint = selectedColumn.heights[corner];
        script.controller = this;
        script.channel = 1;
        script.section = selectedSection;

        heightPointObjects.Add(script);

        point = Instantiate(heightMapChannelPoint, new Vector3(worldX, selectedColumn.heights[corner].height2, worldY), Quaternion.identity);
        script = point.GetComponent<HeightMapChannelPoint>();
        script.heightPoint = selectedColumn.heights[corner];
        script.controller = this;
        script.channel = 2;
        script.section = selectedSection;

        heightPointObjects.Add(script);

        point = Instantiate(heightMapChannelPoint, new Vector3(worldX, selectedColumn.heights[corner].height3, worldY), Quaternion.identity);
        script = point.GetComponent<HeightMapChannelPoint>();
        script.heightPoint = selectedColumn.heights[corner];
        script.controller = this;
        script.channel = 3;
        script.section = selectedSection;

        heightPointObjects.Add(script);

    }

    void RenderSection(int id) {

        var section = Instantiate(meshSection, new Vector3(0, 0, 0), Quaternion.identity);
        var script = section.GetComponent<LevelMesh>();
        script.section = level.sections[id - 1];
        script.levelTexturePallet = levelTexturePallet;
        script.controller = this;

    }

    void RenderFullMap() {

        var x = 0;
        var y = 0;
        foreach (var row in level.layout) {

            foreach (var column in row) {

                if (column != 0) {
                    var section = Instantiate(meshSection, new Vector3(x, 0, -y), Quaternion.identity);
                    var script = section.GetComponent<LevelMesh>();
                    script.section = level.sections[column - 1];
                    script.levelTexturePallet = levelTexturePallet;
                    script.controller = this;
                    script.x = x;
                    script.y = y;
                    sectionMeshes.Add(script);
                }

                x += 16;
            }
            x = 0;
            y += 16;

        }

    }

}
