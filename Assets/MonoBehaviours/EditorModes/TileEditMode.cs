

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class TileEditMode : TileMutatingEditMode, EditMode {

    // This won't cause a memory leak... right?
    public static List<TileSelection> savedSelections = new();
    public static HashSet<LevelMesh> savedSectionSelections = new();

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

    #region Selection

    void SelectLevelItems(TileSelection selection) {

        AddSelectionStateCounterAction();

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

        if (!HasSelection) {
            return;
        }

        view.RefreshTileEffectsPanel();

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
        script.section = selection.section.section;
        previewSelectionOverlay = script;
        overlay.transform.SetParent(selection.section.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    override public void MakeSelection(TileSelection selection, bool deSelectDuplicate = true) {

        if (IsTileAlreadySelected(selection.tile)) {

            if (deSelectDuplicate) {
                RemoveTile(selection.tile);
                RefeshTileOverlay();

                if (!HasSelection) {

                    ClearAllSelectedItems();

                    view.CloseTileEffectsPanel();

                }

            }

        } else {

            selectedItems.Add(selection);
            selectedSections.Add(selection.section);

        }

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


                        var didBreak = ChangeTileCornerChannel(channel);

                        if (didBreak) {
                            return true;
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

    void ClearAllSelectedItems(bool addCounterAction = true) {

        if (HasSelection && addCounterAction) {
            AddSelectionStateCounterAction();
        }

        selectedItems.Clear();
        selectedSections.Clear();
        ClearTileOverlays();

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        heightPointObjects.Clear();

        ClearSectionOverlays();

        ClearPreviewOverlay();

        view.CloseTileEffectsPanel();

    }

    #endregion

    #region GameObject Managment

    void ReinitExistingSelectedItems() {

        if (selectedItems.Count == 0) {
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

    void InitTileOverlay(TileSelection tile) {

        var overlay = Object.Instantiate(main.SelectedTileOverlay);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.controller = main;
        script.tile = tile.tile;
        script.section = tile.section.section;
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

    void RefreshPreviewOverlay() {

        if (previewSelectionOverlay != null) {
            previewSelectionOverlay.Refresh();
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

    void ClearAllGameObjects() {

        ClearTileOverlays();

        ClearPreviewOverlay();

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        heightPointObjects.Clear();

        ClearSectionOverlays();

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

    #endregion

    #region Model Mutating

    bool ChangeTileCornerChannel(TileHeightMapChannelPoint channel) {

        var didCreateCounterAction = false;

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

            if (!didCreateCounterAction) {

                AddTileStateCounterAction();

                didCreateCounterAction = true;
            }

            var vertex = item.tile.verticies[index];
            item.tile.verticies[index] = new TileVertex(channel.channel, vertex.vertexPosition);

        }

        return false;

    }

    void RemoveSelectedTiles() {

        if (!HasSelection) { return; }

        var removedItems = new List<TileSelection>();

        var showDialog = false;
        foreach (var item in selectedItems) {

            if (item.column.tiles.Count == 1) {
                showDialog = true;
                continue;
            }

            removedItems.Add(item);

            item.column.tiles.Remove(item.tile);

        }

        if (removedItems.Count > 0) {
            AddRemoveTileCounterAction(removedItems);
        }

        if (showDialog) {
            QuickLogHandler.Log("At least one tile must be present in a tile column", LogSeverity.Error);
        }

        RefreshMeshes();

        ClearAllSelectedItems();


    }

    public void ChangeTileEffectIndex(int index) {

        AddTileStateCounterAction();

        foreach (var item in selectedItems) {
            item.tile.effectIndex = index;
        }

        RefeshTileOverlay();

    }

    public void ChaneTileEffect(int channel, int value) {

        AddTileEffectChangeCounterAction(channel);

        foreach (var section in selectedSections) {
            section.section.tileEffects[channel] = (byte)value;
        }

        RefeshTileOverlay();

    }

    public void ShiftTilesHeightUp() {

        if (!HasSelection) {
            QuickLogHandler.Log("No tiles are selected", LogSeverity.Info);
            return;
        }

        // Add to Undo stack
        AddTileStateCounterAction();

        foreach (var item in selectedItems) {

            var previousVerticies = new List<TileVertex>(item.tile.verticies);
            var newVerticies = new HashSet<TileVertex>();

            foreach (var index in Enumerable.Range(0, item.tile.verticies.Count)) {

                var vertex = item.tile.verticies[index];

                if (vertex.heightChannel < 3) {

                    vertex.heightChannel += 1;

                    newVerticies.Add(vertex);

                }

            }

            if (newVerticies.Count == previousVerticies.Count) {
                item.tile.verticies = newVerticies.ToList();
            } else {
                QuickLogHandler.Log("Tile is already attatched to highest channels", LogSeverity.Error);
            }

        }

        RefreshMeshes();

        RefeshTileOverlay();

    }
    
    public void ShiftTilesHeightDown() {

        if (!HasSelection) {
            QuickLogHandler.Log("No tiles are selected", LogSeverity.Info);
            return;
        }

        AddTileStateCounterAction();

        foreach (var item in selectedItems) {

            var previousVerticies = new List<TileVertex>(item.tile.verticies);
            var newVerticies = new HashSet<TileVertex>();

            foreach (var index in Enumerable.Range(0, item.tile.verticies.Count)) {

                var vertex = item.tile.verticies[index];

                if (vertex.heightChannel > 1) {

                    vertex.heightChannel -= 1;

                    newVerticies.Add(vertex);

                }

            }

            if (newVerticies.Count == previousVerticies.Count) {
                item.tile.verticies = newVerticies.ToList();
            } else {
                QuickLogHandler.Log("Tile is already attatched to lowest channels", LogSeverity.Error);
            }

        }

        RefreshMeshes();

        RefeshTileOverlay();

    }

    public void BreakApartQuadTileBottomTop() {

        if (!HasSelection) {
            QuickLogHandler.Log("No tiles are selected", LogSeverity.Info);
            return;
        }

        var counterActions = new List<CounterAction>();

        foreach (var selection in selectedItems) {

            var newTiles = selection.tile.BreakApartQuadTileBottomTop();

            if (newTiles != null) {

                selection.column.tiles.AddRange(newTiles);

                foreach (var tile in newTiles) {
                    counterActions.Add(new PassiveAddTileCounterAction(tile, selection.column));
                }

                selection.column.tiles.Remove(selection.tile);

                counterActions.Add(new RemoveTileCounterAction(selection.tile, selection.column));

            }
            else {
                QuickLogHandler.Log("Selected tile is already a triangle", LogSeverity.Error);
            }

        }

        RefreshMeshes();

        counterActions.Add(new SelectionSaveStateCounterAction((TileEditMode)Main.editMode, () => { }));

        ClearAllSelectedItems(false);

        AddBreakApartTileCounterAction(counterActions);

    }

    public void BreakApartQuadTileTopBottom() {

        if (!HasSelection) {
            QuickLogHandler.Log("No tiles are selected", LogSeverity.Info);
            return;
        }

        var counterActions = new List<CounterAction>();

        foreach (var selection in selectedItems) {

            var newTiles = selection.tile.BreakApartQuadTileTopBottom();

            if (newTiles != null) {

                selection.column.tiles.AddRange(newTiles);

                foreach (var tile in newTiles) {
                    counterActions.Add(new PassiveAddTileCounterAction(tile, selection.column));
                }

                selection.column.tiles.Remove(selection.tile);

                counterActions.Add(new RemoveTileCounterAction(selection.tile, selection.column));

            }
            else {
                QuickLogHandler.Log("Selected tile is already a triangle", LogSeverity.Error);
            }

        }

        RefreshMeshes();

        counterActions.Add(new SelectionSaveStateCounterAction((TileEditMode)Main.editMode, () => { }));

        ClearAllSelectedItems(false);

        AddBreakApartTileCounterAction(counterActions);

    }

    public void ExtrudeTiles() {

        if (!HasSelection) {
            QuickLogHandler.Log("No tiles are selected", LogSeverity.Info);
            return;
        }

        var heightMap = new List<HeightPoints>();
        var tileColumns = new List<TileColumn>();

        int width;
        int height;

        HeightPoints GetHeightPoint(int x, int y) {
            return heightMap[(y * (width + 1)) + x];
        }

        TileColumn GetTileColumn(int x, int y) {
            return tileColumns[(y * width) + x];
        }

        var sortedSelectedItems = new List<TileSelection>(selectedItems);

        sortedSelectedItems = sortedSelectedItems.OrderBy(item => item.columnWorldY).ThenBy(item => item.columnWorldX).ToList();

        // Creates a empty grid space of the width and height of selection.
        // This is done by taking the lowest cord selected tile to the highest.

        width = (sortedSelectedItems.Max(item => item.columnWorldX) - sortedSelectedItems.Min(item => item.columnWorldX)) + 1;
        height = (sortedSelectedItems.Max(item => item.columnWorldY) - sortedSelectedItems.Min(item => item.columnWorldY)) + 1;

        foreach (var y in Enumerable.Range(0, height + 1)) {

            foreach (var x in Enumerable.Range(0, width + 1)) {

                heightMap.Add(new HeightPoints(-128, -128, -128));

            }

        }

        foreach (var y in Enumerable.Range(0, height)) {

            foreach (var x in Enumerable.Range(0, width)) {

                var heights = new List<HeightPoints>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                tileColumns.Add(new TileColumn(x, y, new(), heights));

            }

        }

        // After making an empty grid space it can now start filling the empty data with selected tiles

        var startingX = sortedSelectedItems.Min(item => item.columnWorldX);
        var startingY = sortedSelectedItems.Min(item => item.columnWorldY);

        foreach (var item in sortedSelectedItems) {

            var emptyColumn = GetTileColumn(item.columnWorldX - startingX, item.columnWorldY - startingY);

            foreach (var vert in item.tile.verticies) {

                emptyColumn.heights[(int)vert.vertexPosition - 1]
                    .SetPoint(item.column.heights[(int)vert.vertexPosition - 1].GetTruePoint(vert.heightChannel), vert.heightChannel);

            }

            // Note: A tile reference was passed, not a new tile.
            emptyColumn.tiles.Add(item.tile);

        }

        // Verify if tile selection is valid for extuding

        foreach (var column in tileColumns) {

            if (column.tiles.Count > 1) {
                QuickLogHandler.Log("Unable to extrude tiles. Only one tile per column can be selected!", LogSeverity.Error);
                return;
            }
            else if (column.tiles.Count != 0) {

                var tile = column.tiles[0];

                var nullableTileMeshID = MeshType.IDFromVerticies(tile.verticies);

                if (nullableTileMeshID == null) {
                    QuickLogHandler.Log("Unable to extrude tiles. Tile has invalid mesh ID!", LogSeverity.Error);
                    return;
                }

                var tileMeshID = (int)nullableTileMeshID;

                if (MeshType.wallMeshes.Contains(tileMeshID)) {

                    QuickLogHandler.Log("Unable to extrude tiles. Cannot extrude wall tiles.", LogSeverity.Error);
                    return;

                }

                if (tile.verticies.Any(vert => { return vert.heightChannel == 3; })) {

                    QuickLogHandler.Log("Unable to extrude tiles. Tile is already at max height channel!", LogSeverity.Error);
                    return;

                }

                if (!tile.verticies.All(vert => { return vert.heightChannel == tile.verticies[0].heightChannel; })) {

                    QuickLogHandler.Log("Unable to extrude tiles. Tile has different height channels.", LogSeverity.Error);
                    return;

                }

            }

        }

        var affectedSections = new HashSet<LevelMesh>();
        var counterActions = new List<CounterAction>();

        void ShiftTileUp(Tile tile) {

            counterActions.Add(new TileSaveStateCounterAction(tile));

            var previousVerticies = new List<TileVertex>(tile.verticies);
            var newVerticies = new HashSet<TileVertex>();

            foreach (var index in Enumerable.Range(0, tile.verticies.Count)) {

                var vertex = tile.verticies[index];

                if (vertex.heightChannel < 3) {

                    vertex.heightChannel += 1;

                    newVerticies.Add(vertex);

                }

            }

            if (newVerticies.Count == previousVerticies.Count) {
                tile.verticies = newVerticies.ToList();
            }
            else {
                QuickLogHandler.Log("Tile is already attatched to highest channels", LogSeverity.Error);
            }

        }

        void CheckAndExtrude(int x, int y, ExtrudeSide side, int heightChannel, bool ignoreCheck = false) {

            if (y > -1 && y < height && x > -1 && x < width && !ignoreCheck) {

                var virtualColumn = GetTileColumn(x, y);

                if (virtualColumn.tiles.Count != 0) {

                    if (virtualColumn.tiles[0].verticies[0].heightChannel == heightChannel) {
                        return;

                    }

                }

            }

            var worldX = startingX + x;
            var worldY = startingY + y;

            TilePreset referedPreset = null;
            LevelMesh worldSection = null;
            TileColumn worldColumn = null;


            switch (side) {
                case ExtrudeSide.Top:

                    worldY++;

                    worldSection = main.GetLevelMesh(worldX / 16, worldY / 16);
                    worldColumn = worldSection.section.GetTileColumn(worldX % 16, worldY % 16);

                    referedPreset = TileAddMode.defaultPresets[2];

                    // Because presets are classes and not structs, it needs to clean any uses transforms.
                    referedPreset.transformedTile = null;

                    if (heightChannel == 3) {
                        referedPreset.MoveHeightChannelsToNextChannel();
                    }

                    break;
                case ExtrudeSide.Left:

                    worldX++;

                    worldSection = main.GetLevelMesh(worldX / 16, worldY / 16);
                    worldColumn = worldSection.section.GetTileColumn(worldX % 16, worldY % 16);

                    referedPreset = TileAddMode.defaultPresets[2];

                    // Because presets are classes and not structs, it needs to clean any uses transforms.
                    referedPreset.transformedTile = null;

                    referedPreset.RotateCounterClockwise();

                    if (heightChannel == 3) {
                        referedPreset.MoveHeightChannelsToNextChannel();
                    }

                    break;
                case ExtrudeSide.Right:

                    // Fixme: test for world width
                    //if (worldX > ) {
                    //    QuickLogHandler.Log("Unable to extrude side, no room in grid space.", LogSeverity.Warning);
                    //}

                    worldSection = main.GetLevelMesh(worldX / 16, worldY / 16);
                    worldColumn = worldSection.section.GetTileColumn(worldX % 16, worldY % 16);

                    referedPreset = TileAddMode.defaultPresets[2];

                    // Because presets are classes and not structs, it needs to clean any uses transforms.
                    referedPreset.transformedTile = null;

                    referedPreset.RotateCounterClockwise();

                    if (heightChannel == 3) {
                        referedPreset.MoveHeightChannelsToNextChannel();
                    }

                    break;
                case ExtrudeSide.Bottom:

                    // Fixme: test for world height
                    //if (worldX > ) {
                    //    QuickLogHandler.Log("Unable to extrude side, no room in grid space.", LogSeverity.Warning);
                    //}

                    worldSection = main.GetLevelMesh(worldX / 16, worldY / 16);
                    worldColumn = worldSection.section.GetTileColumn(worldX % 16, worldY % 16);

                    referedPreset = TileAddMode.defaultPresets[2];

                    // Because presets are classes and not structs, it needs to clean any uses transforms.
                    referedPreset.transformedTile = null;

                    if (heightChannel == 3) {
                        referedPreset.MoveHeightChannelsToNextChannel();
                    }

                    break;
                case ExtrudeSide.TopLBottomRDiagnol:

                    worldSection = main.GetLevelMesh(worldX / 16, worldY / 16);
                    worldColumn = worldSection.section.GetTileColumn(worldX % 16, worldY % 16);

                    referedPreset = TileAddMode.defaultPresets[4];

                    // Because presets are classes and not structs, it needs to clean any uses transforms.
                    referedPreset.transformedTile = null;

                    referedPreset.RotateCounterClockwise();

                    if (heightChannel == 3) {
                        referedPreset.MoveHeightChannelsToNextChannel();
                    }

                    break;
                case ExtrudeSide.BottomLTopRDiagnol:

                    worldSection = main.GetLevelMesh(worldX / 16, worldY / 16);
                    worldColumn = worldSection.section.GetTileColumn(worldX % 16, worldY % 16);

                    referedPreset = TileAddMode.defaultPresets[4];

                    // Because presets are classes and not structs, it needs to clean any uses transforms.
                    referedPreset.transformedTile = null;

                    if (heightChannel == 3) {
                        referedPreset.MoveHeightChannelsToNextChannel();
                    }

                    break;
            }

            if (worldSection != null) {
                affectedSections.Add(worldSection);
            }

            foreach (var t in worldColumn.tiles) {

                // Checks to make sure the tile doesn't already exist
                if (MeshType.IDFromVerticies(t.verticies) == referedPreset.MeshID()) {
                    QuickLogHandler.Log("Tile was not added as tile aready existed.", LogSeverity.Warning);
                    return;
                }

            }

            var createdTile = referedPreset.Create(worldColumn);

            counterActions.Add(new PassiveAddTileCounterAction(createdTile, worldColumn));

            worldColumn.tiles.Add(createdTile);


        }

        // Does the shift first before anything else.
        foreach (var virtualColumn in tileColumns) {

            if (virtualColumn.tiles.Count != 0) {

                var tile = virtualColumn.tiles[0];

                ShiftTileUp(tile);

            }
        
        }

        foreach (var y in Enumerable.Range(0, height)) {

            foreach (var x in Enumerable.Range(0, width)) {

                var virtualColumn = GetTileColumn(x, y);

                if (virtualColumn.tiles.Count != 0) {

                    var tile = virtualColumn.tiles[0];

                    if (tile.verticies.Count == 4) {
                        CheckAndExtrude(x, y - 1, ExtrudeSide.Top, virtualColumn.tiles[0].verticies[0].heightChannel);
                        CheckAndExtrude(x - 1, y, ExtrudeSide.Left, virtualColumn.tiles[0].verticies[0].heightChannel);
                        CheckAndExtrude(x + 1, y, ExtrudeSide.Right, virtualColumn.tiles[0].verticies[0].heightChannel);
                        CheckAndExtrude(x, y + 1, ExtrudeSide.Bottom, virtualColumn.tiles[0].verticies[0].heightChannel);
                    }
                    else {

                        if (!tile.verticies.Any(vert => { return vert.vertexPosition == VertexPosition.TopLeft; })) {
                            CheckAndExtrude(x, y, ExtrudeSide.BottomLTopRDiagnol, virtualColumn.tiles[0].verticies[0].heightChannel, true);
                            CheckAndExtrude(x, y + 1, ExtrudeSide.Bottom, virtualColumn.tiles[0].verticies[0].heightChannel);
                            CheckAndExtrude(x + 1, y, ExtrudeSide.Right, virtualColumn.tiles[0].verticies[0].heightChannel);

                        }
                        else if (!tile.verticies.Any(vert => { return vert.vertexPosition == VertexPosition.TopRight; })) {
                            CheckAndExtrude(x, y, ExtrudeSide.TopLBottomRDiagnol, virtualColumn.tiles[0].verticies[0].heightChannel, true);
                            CheckAndExtrude(x, y + 1, ExtrudeSide.Bottom, virtualColumn.tiles[0].verticies[0].heightChannel);
                            CheckAndExtrude(x - 1, y, ExtrudeSide.Left, virtualColumn.tiles[0].verticies[0].heightChannel);

                        }
                        else if (!tile.verticies.Any(vert => { return vert.vertexPosition == VertexPosition.BottomLeft; })) {
                            CheckAndExtrude(x, y, ExtrudeSide.TopLBottomRDiagnol, virtualColumn.tiles[0].verticies[0].heightChannel, true);
                            CheckAndExtrude(x, y - 1, ExtrudeSide.Top, virtualColumn.tiles[0].verticies[0].heightChannel);
                            CheckAndExtrude(x + 1, y, ExtrudeSide.Right, virtualColumn.tiles[0].verticies[0].heightChannel);

                        }
                        else if (!tile.verticies.Any(vert => { return vert.vertexPosition == VertexPosition.BottomRight; })) {
                            CheckAndExtrude(x, y, ExtrudeSide.BottomLTopRDiagnol, virtualColumn.tiles[0].verticies[0].heightChannel, true);
                            CheckAndExtrude(x, y - 1, ExtrudeSide.Top, virtualColumn.tiles[0].verticies[0].heightChannel);
                            CheckAndExtrude(x - 1, y, ExtrudeSide.Left, virtualColumn.tiles[0].verticies[0].heightChannel);

                        }

                    }

                }


            }

        }

        AddTileExtrudeCounterAction(counterActions, affectedSections);

        foreach (var section in affectedSections) {
            section.RefreshMesh();
        }
        RefeshTileOverlay();


    }

    #endregion


    #region Counter-Actions

    // Counter-Action methods must be static
    // This is to avoid memory leaks.

    public class TileEffectChangeCounterAction : CounterAction {
        public string name { get; set; }

        FCopLevelSection modifiedSection;
        byte value;
        int index;

        public TileEffectChangeCounterAction(FCopLevelSection modifiedSection, byte value, int index) {
            this.modifiedSection = modifiedSection;
            this.value = value;
            this.index = index;

            name = "Tile Effect Change";
        }

        public void Action() {

            modifiedSection.tileEffects[index] = value;

        }

    }

    public class MultiTileEffectChangeCounterAction : CounterAction {
        public string name { get; set; }

        List<TileEffectChangeCounterAction> counterActions = new();

        public MultiTileEffectChangeCounterAction(HashSet<LevelMesh> changedSections, int index) {

            foreach (var sectionMesh in changedSections) {

                counterActions.Add(new TileEffectChangeCounterAction(
                    sectionMesh.section,
                    sectionMesh.section.tileEffects[index],
                    index
                    ));

            }

            name = "Tile Effect Changes";

        }
        public void Action() {

            foreach (var counterAction in counterActions) {
                counterAction.Action();
            }

            if (Main.editMode is not TileEditMode) {
                return;
            }

            var editMode = (TileEditMode)Main.editMode;

            editMode.RefreshMeshes();

            editMode.ClearAllGameObjects();

            editMode.ReinitExistingSelectedItems();

            editMode.RefreshPreviewOverlay();

            editMode.view.RefreshTileEffectsPanel();

        }

    }

    public class RemoveTileCounterAction : CounterAction {
        public string name { get; set; }

        Tile removedTile;
        TileColumn column;

        public RemoveTileCounterAction(Tile removedTile, TileColumn column) {
            this.removedTile = removedTile;
            this.column = column;

            name = "Tile Removed";
        }

        public void Action() {

            column.tiles.Add(removedTile);

        }


    }

    public class MultiRemoveTileCounterAction : CounterAction {

        public string name { get; set; }

        List<RemoveTileCounterAction> removedTileCounterActions = new();

        public MultiRemoveTileCounterAction(List<TileSelection> items) {

            foreach (var item in items) {

                removedTileCounterActions.Add(new RemoveTileCounterAction(item.tile, item.column));

            }

            name = "Removed Tiles";

        }

        public void Action() {
            
            foreach (var counterAction in removedTileCounterActions) {
                counterAction.Action();
            }

            if (Main.editMode is not TileEditMode) {
                return;
            }

            var editMode = (TileEditMode)Main.editMode;

            editMode.RefreshMeshes();

            editMode.ClearAllGameObjects();

            editMode.ReinitExistingSelectedItems();

            editMode.RefreshPreviewOverlay();

            editMode.view.RefreshTileEffectsPanel();

        }

    }

    public class PassiveAddTileCounterAction : CounterAction {

        public string name { get; set; }

        Tile addedTile;
        TileColumn columnAddedTo;

        public PassiveAddTileCounterAction(Tile addedTile, TileColumn columnAddedTo) {
            this.addedTile = addedTile;
            this.columnAddedTo = columnAddedTo;

            name = "Tile Added";

        }

        public void Action() {

            columnAddedTo.tiles.Remove(addedTile);

        }

    }

    public class TileExtrudeCounterAction : CounterAction {
        public string name { get; set; }

        List<CounterAction> counterActions = new();
        HashSet<LevelMesh> affectedSections;

        public TileExtrudeCounterAction(List<CounterAction> counterActions, HashSet<LevelMesh> affectedSections) {
            this.counterActions = counterActions;
            this.affectedSections = affectedSections;

            name = "Tile Extrude";

        }

        public void Action() {

            foreach (var action in counterActions) {
                action.Action();
            }

            if (Main.editMode is not TileEditMode) {
                return;
            }

            foreach (var section in affectedSections) {
                section.RefreshMesh();
            }

            var editMode = (TileEditMode)Main.editMode;

            editMode.ClearAllGameObjects();

            editMode.ReinitExistingSelectedItems();

            editMode.RefreshPreviewOverlay();

            editMode.view.RefreshTileEffectsPanel();

        }

    }

    void AddTileEffectChangeCounterAction(int index) {

        Main.AddCounterAction(new MultiTileEffectChangeCounterAction(selectedSections, index));

    }

    void AddRemoveTileCounterAction(List<TileSelection> removedItems) {

        Main.AddCounterAction(new MultiRemoveTileCounterAction(removedItems));

    }

    static void AddSelectionStateCounterAction() {

        Main.AddCounterAction(new SelectionSaveStateCounterAction(((TileEditMode)Main.editMode), () => {

            if (Main.editMode is not TileEditMode) {
                return;
            }

            var editMode = (TileEditMode)Main.editMode;

            editMode.ClearAllGameObjects();

            editMode.ReinitExistingSelectedItems();

            editMode.RefreshPreviewOverlay();

            editMode.view.RefreshTileEffectsPanel();

        }));

    }

    static void AddTileStateCounterAction() {

        Main.AddCounterAction(new MultiTileSaveStateCounterAction(((TileEditMode)Main.editMode).selectedItems, () => {

            if (Main.editMode is not TileEditMode) {
                return;
            }

            var editMode = (TileEditMode)Main.editMode;

            editMode.RefreshMeshes();

            editMode.RefeshTileOverlay();

            editMode.RefreshPreviewOverlay();

            editMode.view.RefreshTileEffectsPanel();

        }));

    }

    static void AddBreakApartTileCounterAction(List<CounterAction> counterActions) {

        Main.AddCounterAction(new MultiCounterAction(counterActions, () => {

            if (Main.editMode is not TileEditMode) {
                return;
            }

            var editMode = (TileEditMode)Main.editMode;

            editMode.RefreshMeshes();

            editMode.RefeshTileOverlay();

            editMode.RefreshPreviewOverlay();

            editMode.view.RefreshTileEffectsPanel();

        }));

    }

    static void AddTileExtrudeCounterAction(List<CounterAction> counterActions, HashSet<LevelMesh> affectedSections) {
        Main.AddCounterAction(new TileExtrudeCounterAction(counterActions, affectedSections));
    }

    #endregion


    public enum ExtrudeSide {
        Top,
        Left, 
        Right,
        Bottom,
        TopLBottomRDiagnol,
        BottomLTopRDiagnol



    }

}