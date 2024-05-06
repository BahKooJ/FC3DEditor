
using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class TileEditMode : TileMutatingEditMode, EditMode {

    // This won't cause a memory leak... right?
    static List<TileSelection> savedSelections = new();
    static HashSet<LevelMesh> savedSectionSelections = new();

    override public Main main { get; set; }
    public TileEditPanel view;

    public List<SelectedTileOverlay> selectedTileOverlays = new();
    public List<GameObject> selectedSectionOverlays = new();

    public SelectedTileOverlay previewSelectionOverlay = null;

    public List<TileHeightMapChannelPoint> heightPointObjects = new();
    public TileHeightMapChannelPoint selectedHeight = null;

    public TileEditMode(Main main) {
        this.main = main;
    }

    override public void Update() {

        if (Controls.OnUp("Select")) {
            selectedHeight = null;
        }

        var didHitHeight = TestTileHeightSelection();

        if (!didHitHeight) {

            if (Controls.OnDown("Select")) {
                
                var selection = main.GetTileOnLevelMesh(!FreeMove.looking);

                if (selection != null) { 
                    SelectLevelItems(selection);
                }

            }
            else {

                var previewSelection = main.GetTileOnLevelMesh(!FreeMove.looking);

                if (previewSelection != null) {
                    PreviewSelection(previewSelection);
                }
                else if (previewSelectionOverlay != null) {

                    ClearPreviewOverlay();

                }

            }

        }
        else if (previewSelectionOverlay != null) {

            ClearPreviewOverlay();

        }

        if (Controls.OnDown("Unselect")) {
            
            ClearAllSelectedItems();

        }
        else if (Controls.OnDown("Delete")) {
            RemoveSelectedTiles();
        }

    }

    override public void OnCreateMode() {
        selectedItems = savedSelections;
        selectedSections = savedSectionSelections;
        ReinitExistingSelectedItems();
    }

    override public void OnDestroy() {

        savedSelections = selectedItems;
        savedSectionSelections = selectedSections;

        ClearAllGameObjects();

        if (view.debugTilePanelView != null) {
            Object.Destroy(view.debugTilePanelView);
        }
    }

    void SelectLevelItems(TileSelection selection) {

        // If shift is held then multiple tiles can be selected
        if (!Controls.IsDown("MultiSelect")) {

            // Clears the selected tile(s).
            selectedItems.Clear();
            selectedSections.Clear();

        }

        // Selects a range of tiles if the mutli-select modifier is held.
        if (Controls.OnDown("RangeSelect")) {

            if (HasSelection) {

                SelectRangeOfTiles(selection);

            }

        }
        else {

            MakeSelection(selection);

        }

        ClearSectionOverlays();

        foreach (var iSection in selectedSections) {
            selectedSectionOverlays.Add(Object.Instantiate(main.SectionBoarders, new Vector3(iSection.x, 0, -iSection.y), Quaternion.identity));
        }

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

    void PreviewSelection(TileSelection selection) {

        if (previewSelectionOverlay != null) {

            if (previewSelectionOverlay.tile == selection.tile) {
                return;
            }

            ClearPreviewOverlay();

        }

        var overlay = Object.Instantiate(main.SelectedTileOverlay);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.controller = main;
        script.tile = selection.tile;
        previewSelectionOverlay = script;
        overlay.transform.SetParent(selection.section.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    bool TestTileHeightSelection() {

        if (FreeMove.looking) {
            return false;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

            foreach (var channel in heightPointObjects) {

                if (hit.colliderInstanceID == channel.boxCollider.GetInstanceID()) {
                    
                    if (Controls.IsDown("Select")) {

                        if (selectedHeight == null) {

                            var index = FirstTile.verticies.FindIndex(vertex => {
                                return vertex.vertexPosition == channel.corner && vertex.heightChannel == channel.channel;
                            });

                            if (index != -1) {
                                selectedHeight = channel;
                            }

                            return true;
                        }

                        if (channel.heightPoints != selectedHeight.heightPoints) {
                            return true;
                        }

                        foreach (var item in selectedItems) {

                            var index = item.tile.verticies.FindIndex(vertex => {
                                return vertex.vertexPosition == selectedHeight.corner && vertex.heightChannel == selectedHeight.channel;
                            });

                            var existingVert = item.tile.verticies.FindIndex(vertex => {
                                return vertex.vertexPosition == channel.corner && vertex.heightChannel == channel.channel;
                            });

                            if (index == -1 || existingVert != -1) {

                                selectedHeight = channel;

                                RefreshMeshes();
                                RefeshTileOverlay();
                                return true;
                            }

                            var vertex = item.tile.verticies[index];
                            item.tile.verticies[index] = new TileVertex(channel.channel, vertex.vertexPosition);
                            
                        }
                        selectedHeight = channel;

                        RefreshMeshes();
                        RefeshTileOverlay();

                    }
                    else {
                        return true;
                    }

                }

            }

        }

        return false;

    }

    void ReinitExistingSelectedItems() {

        if (savedSelections.Count == 0) {
            return;
        }

        RefeshTileOverlay();

        foreach (var iSection in selectedSections) {
            selectedSectionOverlays.Add(Object.Instantiate(main.SectionBoarders, new Vector3(iSection.x, 0, -iSection.y), Quaternion.identity));
        }

        AddHeightObjects(VertexPosition.TopLeft);
        AddHeightObjects(VertexPosition.TopRight);
        AddHeightObjects(VertexPosition.BottomLeft);
        AddHeightObjects(VertexPosition.BottomRight);

    }

    override public void MakeSelection(TileSelection selection, bool deSelectDuplicate = true) {

        if (IsTileAlreadySelected(selection.tile)) {

            if (deSelectDuplicate) {
                RemoveTile(selection.tile);
                RefeshTileOverlay();

                if (!HasSelection) {

                    ClearAllSelectedItems();

                }

            }

        }
        else {

            selectedItems.Add(selection);
            selectedSections.Add(selection.section);

        }

    }

    void InitTileOverlay(TileSelection tile) {

        var overlay = Object.Instantiate(main.SelectedTileOverlay);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.controller = main;
        script.tile = tile.tile;
        selectedTileOverlays.Add(script);
        overlay.transform.SetParent(tile.section.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    void RefeshTileOverlay() {

        if (Main.debug) {
            return;
        }

        ClearTileOverlays();

        foreach (var item in selectedItems) {

            InitTileOverlay(item);

        }

    }

    void ClearTileOverlays() {

        foreach (var overlay in selectedTileOverlays) {
            Object.Destroy(overlay.gameObject);
        }

        selectedTileOverlays.Clear();

    }

    void ClearPreviewOverlay() {

        if (previewSelectionOverlay != null) {
            Object.Destroy(previewSelectionOverlay.gameObject);
            previewSelectionOverlay = null;
        }

    }

    void ClearSectionOverlays() {

        foreach (var selectedSectionOverlay in selectedSectionOverlays) {
            Object.Destroy(selectedSectionOverlay);
        }

        selectedSectionOverlays.Clear();

    }

    void RemoveSelectedTiles() {

        if (!HasSelection) { return; }

        var showDialog = false;
        foreach (var item in selectedItems) {

            if (item.column.tiles.Count == 1) {
                showDialog = true;
                continue;
            }

            item.column.tiles.Remove(item.tile);

        }

        if (showDialog) {
            DialogWindowUtil.Dialog("Cannot Remove Tile", "At least one tile must be present in a tile column");
        }

        RefreshMeshes();

        ClearAllSelectedItems();


    }

    void ClearAllGameObjects() {

        ClearTileOverlays();

        ClearPreviewOverlay();

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        foreach (var selectedSectionOverlay in selectedSectionOverlays) {
            Object.Destroy(selectedSectionOverlay);
        }

    }

    void ClearAllSelectedItems() {

        selectedItems.Clear();
        selectedSections.Clear();
        ClearTileOverlays();

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        heightPointObjects.Clear();

        ClearSectionOverlays();

        ClearPreviewOverlay();

    }

    void AddHeightObjects(VertexPosition corner) {

        AddSingleHeightChannelObject(corner, 1, FirstItem.column);
        AddSingleHeightChannelObject(corner, 2, FirstItem.column);
        AddSingleHeightChannelObject(corner, 3, FirstItem.column);

    }

    void AddSingleHeightChannelObject(VertexPosition corner, int channel, TileColumn column) {

        var existingHeightChannel = heightPointObjects.Find(obj => {
            return obj.heightPoints == column.heights[(int)corner - 1] && obj.channel == channel;
        });

        if (existingHeightChannel != null) {
            return;
        }

        var worldX = FirstItem.section.x + column.x;
        var worldY = -(FirstItem.section.y + column.y);

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
        script.section = FirstItem.section;

        heightPointObjects.Add(script);

    }

    #region Callbacks

    public void ShiftTilesHeightUp() {

        if (!HasSelection) { return; }

        foreach (var item in selectedItems) {

            var previousVerticies = new List<TileVertex>(item.tile.verticies);
            var newVerticies = new HashSet<TileVertex>();

            foreach (var index in Enumerable.Range(0, item.tile.verticies.Count)) {

                var vertex = item.tile.verticies[index];

                if (vertex.heightChannel < 3) {

                    vertex.heightChannel += 1;

                    newVerticies.Add(vertex);

                }

                if (newVerticies.Count == previousVerticies.Count) {
                    item.tile.verticies = newVerticies.ToList();
                }

            }

        }

        RefreshMeshes();

        RefeshTileOverlay();

    }

    public void ShiftTilesHeightDown() {

        if (!HasSelection) { return; }

        foreach (var item in selectedItems) {

            var previousVerticies = new List<TileVertex>(item.tile.verticies);
            var newVerticies = new HashSet<TileVertex>();

            foreach (var index in Enumerable.Range(0, item.tile.verticies.Count)) {

                var vertex = item.tile.verticies[index];

                if (vertex.heightChannel > 1) {

                    vertex.heightChannel -= 1;

                    newVerticies.Add(vertex);

                }

                if (newVerticies.Count == previousVerticies.Count) {
                    item.tile.verticies = newVerticies.ToList();
                }

            }

        }

        RefreshMeshes();

        RefeshTileOverlay();

    }

    #endregion
}