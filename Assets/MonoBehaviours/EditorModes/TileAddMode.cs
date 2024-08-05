

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Presets;
using UnityEngine;

public class TileAddMode : EditMode {

    public Main main { get; set; }

    // =Settings=
    public enum SchematicPlacementSetting {
        Exact = 0,
        Relative = 1,
        Keep = 2
    }
    public static SchematicPlacementSetting placementSetting = SchematicPlacementSetting.Exact;
    public static bool removeAllTilesOnSchematicPlacement = false;

    public static readonly List<TilePreset> defaultPresets = new() {
        new TilePreset(68, 0),  // Quad
        new TilePreset(0, 0),   // Triangle
        new TilePreset(108, 3), // Quad Wall
        new TilePreset(83, 3),  // Triangle Wall
        new TilePreset(103, 3),  // Diagonal Quad Wall
        new TilePreset(91, 3),  // Diagonal Triangle Wall

    };

    public static TilePreset selectedTilePreset = null;
    public static Schematic selectedSchematic = null;

    public TileSelection hoverSelection;

    public List<TileHeightMapChannelPoint> heightPointObjects = new();
    public SelectedTileOverlay buildTileOverlay = null;
    public List<SelectedTileOverlay> rangeBuildOverlay = new();
    public SchematicMesh schematicBuildOverlay = null;


    public TileAddMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {
        
        if (selectedSchematic != null) {
            InitSchematicMeshOverlay();
        }

        foreach (var preset in defaultPresets) {
            preset.transformedTile = null;
        }

    }

    public void OnDestroy() {

        ClearBuildingOverlay();

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        heightPointObjects.Clear();

    }

    TileSelection startSelection = null;
    TileSelection endSelection = null;

    public void Update() {

        TileSelection hover;

        if (FreeMove.looking) {
            hover = main.GetTileOnLevelMesh(false);
        } else {
            hover = main.GetTileOnLevelMesh(true);
        }

        hoverSelection = hover;

        if (hoverSelection != null && schematicBuildOverlay != null) {

            PreviewSchematicPlacement();

        } 
        if (hover != null && selectedTilePreset != null) {
            
            if (!Controls.IsDown("Select")) {
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

        if (Main.ignoreAllInputs) {
            return;
        }

        if (selectedSchematic != null) {

            if (Controls.OnDown("ResetLevelSchematic")) {

                selectedSchematic.transformedSchematic = null;

                schematicBuildOverlay.RefreshPreviewColumns();
                schematicBuildOverlay.RefreshMesh();

            }

            if (Controls.IsDown("EnableRotateLevelSchematic")) {

                if (Input.GetMouseButtonDown(0)) {

                    selectedSchematic.transformedSchematic ??= selectedSchematic.Clone();

                    selectedSchematic.transformedSchematic.RotateClockwise();
                    schematicBuildOverlay.RefreshPreviewColumns();
                    schematicBuildOverlay.RefreshMesh();

                }
                else if (Input.GetMouseButtonDown(1)) {

                    selectedSchematic.transformedSchematic ??= selectedSchematic.Clone();

                    selectedSchematic.transformedSchematic.RotateCounterClockwise();
                    schematicBuildOverlay.RefreshPreviewColumns();
                    schematicBuildOverlay.RefreshMesh();

                }

            }
            else if (Controls.IsDown("EnableMirrorLevelSchematic")) {

                if (Input.GetMouseButtonDown(0)) {

                    selectedSchematic.transformedSchematic ??= selectedSchematic.Clone();

                    selectedSchematic.transformedSchematic.MirrorVertically();
                    schematicBuildOverlay.RefreshPreviewColumns();
                    schematicBuildOverlay.RefreshMesh();

                }
                else if (Input.GetMouseButtonDown(1)) {

                    selectedSchematic.transformedSchematic ??= selectedSchematic.Clone();

                    selectedSchematic.transformedSchematic.MirrorHorizontally();
                    schematicBuildOverlay.RefreshPreviewColumns();
                    schematicBuildOverlay.RefreshMesh();

                }

            }
            else if (Controls.OnDown("Select")) {
                
                PlaceSchematic();

            }

        }
        else if (selectedTilePreset != null) {

            float axis = Input.GetAxis("Mouse ScrollWheel");

            if (axis > 0) {
                selectedTilePreset.RotateClockwise();
                RefreshTilePlacementOverlay();
            }
            else if (axis < 0) {
                selectedTilePreset.RotateCounterClockwise();
                RefreshTilePlacementOverlay();
            }

            if (Input.GetMouseButtonDown(1)) {
                selectedTilePreset.MoveHeightChannelsToNextChannel();
                RefreshTilePlacementOverlay();
            }

            if (Controls.OnDown("Select") && hoverSelection != null) {
                startSelection = hoverSelection;
            }

            if (Controls.IsDown("Select") && hoverSelection != null) {
                PreviewRangeTilePlacement();
            }

            if (Controls.OnUp("Select") && hoverSelection != null && startSelection != null) {
                endSelection = hoverSelection;

                if (startSelection.tile == endSelection.tile) {

                    AddTile(selectedTilePreset);

                } else {

                    RangeBuild(startSelection, endSelection, selectedTilePreset);
                    ClearBuildingOverlay();

                }

            }


        }

    }

    public void SelectSchematic(Schematic schematic) {
        ClearBuildingOverlay();

        selectedTilePreset = null;

        if (selectedSchematic != null) {
            selectedSchematic.transformedSchematic = null;
        }

        selectedSchematic = schematic;
        InitSchematicMeshOverlay();
    }

    public void SelectTilePreset(int index) {
        ClearBuildingOverlay();

        selectedSchematic = null;

        selectedTilePreset = defaultPresets[index];
    }

    #region GameObject Managment

    public void RefreshTilePlacementOverlay() {

        if (hoverSelection == null) { return; }

        ClearBuildingOverlay();

        if (selectedTilePreset != null) {

            InitBuildTileOverlay(selectedTilePreset.Create(hoverSelection.column));

        }

    }

    void InitSchematicMeshOverlay() {

        if (schematicBuildOverlay != null) {
            Object.Destroy(schematicBuildOverlay.gameObject);
        }

        schematicBuildOverlay = null;

        var obj = Object.Instantiate(main.schematicMesh);
        var schematicMesh = obj.GetComponent<SchematicMesh>();
        schematicMesh.controller = this;
        schematicMesh.schematic = selectedSchematic;
        schematicBuildOverlay = schematicMesh;

    }

    void PreviewTilePlacement() {

        ClearBuildingOverlay();

        foreach (var point in heightPointObjects) {
            Object.Destroy(point.gameObject);
        }

        heightPointObjects.Clear();

        if (selectedTilePreset != null) {

            InitBuildTileOverlay(selectedTilePreset.Create(hoverSelection.column));

            // Re-adds HeightMapChannelPoints (0 = top left, 1 = top right, 2 = bottom left, 3 = bottom right)
            AddHeightObjects(VertexPosition.TopLeft);
            AddHeightObjects(VertexPosition.TopRight);
            AddHeightObjects(VertexPosition.BottomLeft);
            AddHeightObjects(VertexPosition.BottomRight);

        }

    }

    void PreviewSchematicPlacement() {

        var prevX = hoverSelection.columnWorldX;
        var prevY = hoverSelection.columnWorldY;

        if (placementSetting == SchematicPlacementSetting.Exact) {
            schematicBuildOverlay.transform.position = new Vector3(prevX, 0, -prevY);
        }
        if (placementSetting == SchematicPlacementSetting.Relative) {

            var hoverMax = hoverSelection.tile.GetMaxHeight();
            var schemLow = selectedSchematic.LowestHeight();

            var dif = hoverMax - schemLow;

            schematicBuildOverlay.transform.position = new Vector3(prevX, dif / HeightPoints.multiplyer, -prevY);

        }
        if (placementSetting == SchematicPlacementSetting.Keep) {

            foreach (var y in Enumerable.Range(hoverSelection.columnWorldY, selectedSchematic.height)) {

                foreach (var x in Enumerable.Range(hoverSelection.columnWorldX, selectedSchematic.width)) {

                    var itSection = main.GetLevelMesh(x / 16, y / 16);
                    var itColumn = itSection.section.GetTileColumn(x % 16, y % 16);

                    var itSchemColumn = schematicBuildOverlay.GetTileColumn(x - hoverSelection.columnWorldX, y - hoverSelection.columnWorldY);

                    foreach (var i in Enumerable.Range(0, itColumn.heights.Count)) {

                        itSchemColumn.heights[i] = itColumn.heights[i].Clone();

                    }

                }

            }

            schematicBuildOverlay.RefreshMesh();

            schematicBuildOverlay.transform.position = new Vector3(prevX, 0, -prevY);

        }

    }

    // FIXME: This is an exact copy of RangeAdd...
    Tile lastHoverTile = null;
    void PreviewRangeTilePlacement() {

        var firstClickColumnSectionX = startSelection.column.x + startSelection.section.arrayX * 16;
        var firstClickColumnSectionY = startSelection.column.y + startSelection.section.arrayY * 16;

        var lastClickColumnSectionX = hoverSelection.column.x + hoverSelection.section.arrayX * 16;
        var lastClickColumnSectionY = hoverSelection.column.y + hoverSelection.section.arrayY * 16;

        var startSectionX = firstClickColumnSectionX < lastClickColumnSectionX ? firstClickColumnSectionX : lastClickColumnSectionX;
        var startSectionY = firstClickColumnSectionY < lastClickColumnSectionY ? firstClickColumnSectionY : lastClickColumnSectionY;

        var endSectionX = firstClickColumnSectionX > lastClickColumnSectionX ? firstClickColumnSectionX : lastClickColumnSectionX;
        var endSectionY = firstClickColumnSectionY > lastClickColumnSectionY ? firstClickColumnSectionY : lastClickColumnSectionY;

        if (lastHoverTile != null) {

            if (lastHoverTile == hoverSelection.tile) {
                return;
            }

        }

        lastHoverTile = hoverSelection.tile;
        ClearBuildingOverlay();

        foreach (var y in Enumerable.Range(startSectionY, endSectionY - startSectionY + 1)) {

            foreach (var x in Enumerable.Range(startSectionX, endSectionX - startSectionX + 1)) {

                var sectionX = x / 16;
                var sectionY = y / 16;
                var columnX = x % 16;
                var columnY = y % 16;

                if (MeshType.topWallMeshes.Contains(selectedTilePreset.MeshID())) {

                    if (firstClickColumnSectionY / 16 != sectionY || firstClickColumnSectionY % 16 != columnY) {
                        continue;
                    }

                }
                if (MeshType.leftWallMeshes.Contains(selectedTilePreset.MeshID())) {

                    if (firstClickColumnSectionX / 16 != sectionX || firstClickColumnSectionX % 16 != columnX) {
                        continue;
                    }

                }
                if (MeshType.diagonalTLeftBRightWallMeshes.Contains(selectedTilePreset.MeshID())) {

                    if (x - firstClickColumnSectionX != y - firstClickColumnSectionY) {
                        continue;
                    }

                }
                if (MeshType.diagonalBLeftTRightWallMeshes.Contains(selectedTilePreset.MeshID())) {

                    if (x - firstClickColumnSectionX < 0 && y - firstClickColumnSectionY < 0) {
                        continue;
                    }

                    if (x - firstClickColumnSectionX > 0 && y - firstClickColumnSectionY > 0) {
                        continue;
                    }

                    if (Mathf.Abs(x - firstClickColumnSectionX) != Mathf.Abs(y - firstClickColumnSectionY)) {
                        continue;
                    }

                }

                var itSection = main.GetLevelMesh(sectionX, sectionY);
                var itColumn = itSection.section.GetTileColumn(columnX, columnY);

                var overlay = Object.Instantiate(main.SelectedTileOverlay);
                var script = overlay.GetComponent<SelectedTileOverlay>();
                script.controller = main;
                script.tile = selectedTilePreset.Create(itColumn);
                rangeBuildOverlay.Add(script);
                overlay.transform.SetParent(itSection.transform);
                overlay.transform.localPosition = Vector3.zero;

            }

        }

    }

    void ClearBuildingOverlay() {

        if (buildTileOverlay != null) {
            Object.Destroy(buildTileOverlay.gameObject);
        }

        buildTileOverlay = null;

        if (schematicBuildOverlay != null) {
            Object.Destroy(schematicBuildOverlay.gameObject);
        }

        schematicBuildOverlay = null;

        foreach (var obj in rangeBuildOverlay) {
            Object.Destroy(obj.gameObject);
        }

        rangeBuildOverlay.Clear();


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

    #endregion

    #region Model Mutating

    void AddTile(TilePreset preset) {

        if (selectedTilePreset == null) { return; }

        if (hoverSelection == null) { return; }

        foreach (var t in hoverSelection.column.tiles) {

            // Checks to make sure the tile doesn't already exist
            if (MeshType.IDFromVerticies(t.verticies) == preset.MeshID()) {
                QuickLogHandler.Log("Tile already exists!", LogSeverity.Error);
                return;
            }

        }

        var tile = preset.Create(hoverSelection.column);

        Main.AddCounterAction(new AddTileCounterAction(tile, hoverSelection.column, hoverSelection.section));

        hoverSelection.column.tiles.Add(tile);

        hoverSelection.section.RefreshMesh();

    }

    void RangeBuild(TileSelection firstSelection, TileSelection endSelection, TilePreset preset) {

        var firstClickColumnSectionX = firstSelection.column.x + firstSelection.section.arrayX * 16;
        var firstClickColumnSectionY = firstSelection.column.y + firstSelection.section.arrayY * 16;

        var lastClickColumnSectionX = endSelection.column.x + endSelection.section.arrayX * 16;
        var lastClickColumnSectionY = endSelection.column.y + endSelection.section.arrayY * 16;

        var startSectionX = firstClickColumnSectionX < lastClickColumnSectionX ? firstClickColumnSectionX : lastClickColumnSectionX;
        var startSectionY = firstClickColumnSectionY < lastClickColumnSectionY ? firstClickColumnSectionY : lastClickColumnSectionY;

        var endSectionX = firstClickColumnSectionX > lastClickColumnSectionX ? firstClickColumnSectionX : lastClickColumnSectionX;
        var endSectionY = firstClickColumnSectionY > lastClickColumnSectionY ? firstClickColumnSectionY : lastClickColumnSectionY;

        var affectedSections = new HashSet<LevelMesh>();
        var addedTiles = new List<Tile>();

        foreach (var y in Enumerable.Range(startSectionY, endSectionY - startSectionY + 1)) {

            foreach (var x in Enumerable.Range(startSectionX, endSectionX - startSectionX + 1)) {

                var sectionX = x / 16;
                var sectionY = y / 16;
                var columnX = x % 16;
                var columnY = y % 16;

                if (MeshType.topWallMeshes.Contains(preset.MeshID())) {

                    if (firstClickColumnSectionY / 16 != sectionY || firstClickColumnSectionY % 16 != columnY) {
                        continue;
                    }

                }
                if (MeshType.leftWallMeshes.Contains(selectedTilePreset.MeshID())) {

                    if (firstClickColumnSectionX / 16 != sectionX || firstClickColumnSectionX % 16 != columnX) {
                        continue;
                    }

                }
                if (MeshType.diagonalTLeftBRightWallMeshes.Contains(selectedTilePreset.MeshID())) {

                    if (x - firstClickColumnSectionX != y - firstClickColumnSectionY) {
                        continue;
                    }

                }
                if (MeshType.diagonalBLeftTRightWallMeshes.Contains(selectedTilePreset.MeshID())) {

                    if (x - firstClickColumnSectionX < 0 && y - firstClickColumnSectionY < 0) {
                        continue;
                    }

                    if (x - firstClickColumnSectionX > 0 && y - firstClickColumnSectionY > 0) {
                        continue;
                    }

                    if (Mathf.Abs(x - firstClickColumnSectionX) != Mathf.Abs(y - firstClickColumnSectionY)) {
                        continue;
                    }

                }

                var itSection = main.GetLevelMesh(sectionX, sectionY);
                var itColumn = itSection.section.GetTileColumn(columnX, columnY);

                affectedSections.Add(itSection);

                var alreadyExists = false;
                foreach (var itTile in itColumn.tiles) {

                    if (MeshType.IDFromVerticies(itTile.verticies) == preset.MeshID()) {
                        alreadyExists = true;
                    }

                }

                if (!alreadyExists) {

                    // Mutation
                    var tile = preset.Create(itColumn);

                    itColumn.tiles.Add(tile);

                    // Added for counter-action
                    addedTiles.Add(tile);

                }

            }

        }

        foreach (var affectSection in affectedSections) {
            affectSection.RefreshMesh();
        }

        AddMultiAddTileCounterAction(addedTiles, affectedSections);

    }

    void PlaceSchematic() {

        if (selectedSchematic == null) { return; }

        if (hoverSelection == null) { return; }

        var schematicToPlace = selectedSchematic.transformedSchematic == null ? selectedSchematic : selectedSchematic.transformedSchematic;

        var affectedSections = new HashSet<LevelMesh>();

        void FinishUp() {
            foreach (var affectSection in affectedSections) {
                affectSection.RefreshMesh();
            }
        }

        // Firse it needs to know the amount of sections that will be mutated for undo
        foreach (var y in Enumerable.Range(hoverSelection.columnWorldY, schematicToPlace.height)) {

            foreach (var x in Enumerable.Range(hoverSelection.columnWorldX, schematicToPlace.width)) {

                var itSection = main.GetLevelMesh(x / 16, y / 16);

                affectedSections.Add(itSection);

            }

        }

        AddSchematicAddCounterAction(affectedSections.ToList());

        // Next it adds all the tiles
        foreach (var y in Enumerable.Range(hoverSelection.columnWorldY, schematicToPlace.height)) {

            foreach (var x in Enumerable.Range(hoverSelection.columnWorldX, schematicToPlace.width)) {

                var itSection = main.GetLevelMesh(x / 16, y / 16);
                var itColumn = itSection.section.GetTileColumn(x % 16, y % 16);

                var itSchemColumn = schematicToPlace.GetTileColumn(x - hoverSelection.columnWorldX, y - hoverSelection.columnWorldY);

                if (itSchemColumn.tiles.Count == 0) {
                    continue;
                }

                if (removeAllTilesOnSchematicPlacement) {
                    // Mutation
                    itColumn.tiles.Clear();
                }

                foreach (var schemTile in itSchemColumn.tiles) {

                    var addTile = true;
                    if (!removeAllTilesOnSchematicPlacement) {

                        var schemTileMeshID = MeshType.IDFromVerticies(schemTile.verticies);

                        foreach (var tile in itColumn.tiles) {

                            if (MeshType.IDFromVerticies(tile.verticies) == schemTileMeshID) {

                                // Mutation
                                tile.ReceiveData(schemTile, false);

                                addTile = false;
                                break;
                            }

                        }

                    }

                    if (addTile) {
                        // Mutation
                        itColumn.tiles.Add(new Tile(schemTile, itColumn, itSection.section));
                    }

                }


            }

        }

        // Finally the heights
        if (placementSetting == SchematicPlacementSetting.Keep) {
            FinishUp();
            return;
        }

        var hoverMax = hoverSelection.tile.GetMaxHeight();
        var schemLow = schematicToPlace.LowestHeight();

        var dif = hoverMax - schemLow;

        foreach (var y in Enumerable.Range(hoverSelection.columnWorldY, schematicToPlace.height)) {

            foreach (var x in Enumerable.Range(hoverSelection.columnWorldX, schematicToPlace.width)) {

                var itSection = main.GetLevelMesh(x / 16, y / 16);
                var itColumn = itSection.section.GetTileColumn(x % 16, y % 16);

                var itSchemColumn = schematicToPlace.GetTileColumn(x - hoverSelection.columnWorldX, y - hoverSelection.columnWorldY);

                foreach (var i in Enumerable.Range(0, itColumn.heights.Count)) {

                    var schemHeights = itSchemColumn.heights[i];

                    foreach (var c in Enumerable.Range(1, 3)) {

                        if (schemHeights.GetTruePoint(c) != -128) {

                            // Mutation
                            // Also this works because heights hold classes not structs
                            // Assuming these heights are correct...
                            if (placementSetting == SchematicPlacementSetting.Exact) {
                                itColumn.heights[i].SetPoint(schemHeights.GetTruePoint(c), c);
                            }
                            else if (placementSetting == SchematicPlacementSetting.Relative) {

                                itColumn.heights[i].SetPoint(schemHeights.GetTruePoint(c) + dif, c);

                            }

                        }

                    }


                }

            }

        }

        FinishUp();

    }

    #endregion

    #region Counter-Action

    public class AddTileCounterAction : CounterAction {

        Tile addedTile;
        TileColumn columnAddedTo;
        LevelMesh sectionAddedTo;

        public AddTileCounterAction(Tile addedTile, TileColumn columnAddedTo, LevelMesh section) {
            this.addedTile = addedTile;
            this.columnAddedTo = columnAddedTo;
            this.sectionAddedTo = section;
        }

        public void Action() {

            columnAddedTo.tiles.Remove(addedTile);

            sectionAddedTo.RefreshMesh();

        }

    }

    public class MultiAddTileCounterAction : CounterAction {

        List<Tile> tilesAdded;
        HashSet<LevelMesh> affectedSections;

        public MultiAddTileCounterAction(List<Tile> tilesAdded, HashSet<LevelMesh> affectedSections) {
            this.tilesAdded = tilesAdded;
            this.affectedSections = affectedSections;
        }

        public void Action() {

            foreach (var tile in tilesAdded) {

                tile.column.tiles.Remove(tile);

            }

            foreach (var affectSection in affectedSections) {
                affectSection.RefreshMesh();
            }

        }



    }

    public class SchematicAddCounterAction : CounterAction {

        List<SectionSaveStateCounterAction> savedSections = new();

        public SchematicAddCounterAction(List<LevelMesh> sectionMeshes) {

            foreach (var section in sectionMeshes) {
                savedSections.Add(new SectionSaveStateCounterAction(section));
            }

        }

        public void Action() {

            foreach (var state in savedSections) {
                state.Action();
            }

        }

    }

    void AddSchematicAddCounterAction(List<LevelMesh> levelMeshes) {
        Main.AddCounterAction(new SchematicAddCounterAction(levelMeshes));
    }

    void AddMultiAddTileCounterAction(List<Tile> tilesAdded, HashSet<LevelMesh> affectedSections) {
        Main.AddCounterAction(new MultiAddTileCounterAction(tilesAdded, affectedSections));
    }

    #endregion

}