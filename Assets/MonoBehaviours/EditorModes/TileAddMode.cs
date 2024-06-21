

using FCopParser;
using System.Collections.Generic;
using System.Linq;
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


    public static TilePreset? selectedTilePreset = null;
    public static Schematic selectedSchematic = null;

    public TileSelection hoverSelection;

    public List<TileHeightMapChannelPoint> heightPointObjects = new();
    public SelectedTileOverlay buildTileOverlay = null;
    public SchematicMesh schematicBuildOverlay = null;


    public TileAddMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {
        
        if (selectedSchematic != null) {
            InitSchematicMeshOverlay();
        }

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

        hoverSelection = hover;

        if (hoverSelection != null && schematicBuildOverlay != null) {

            PreviewSchematicPlacement();

        } 
        if (hover != null && selectedTilePreset != null) {
            
            PreviewTilePlacement();

        }
        else if (heightPointObjects.Count != 0) {

            ClearBuildingOverlay();

            foreach (var point in heightPointObjects) {
                Object.Destroy(point.gameObject);
            }

            heightPointObjects.Clear();

            hoverSelection = null;

        }

        if (Controls.OnDown("Select")) {

            if (selectedTilePreset != null) {
                AddTile((TilePreset)selectedTilePreset);
            }

            if (selectedSchematic != null) {
                PlaceSchematic();
            }

        }

    }

    public void SelectSchematic(Schematic schematic) {
        ClearBuildingOverlay();

        selectedTilePreset = null;

        selectedSchematic = schematic;
        InitSchematicMeshOverlay();
    }

    public void SelectTilePreset(TilePreset preset) {
        ClearBuildingOverlay();

        selectedSchematic = null;

        selectedTilePreset = preset;
    }

    #region GameObject Managment

    public void RefreshTilePlacementOverlay() {

        if (hoverSelection == null) { return; }

        ClearBuildingOverlay();

        if (selectedTilePreset != null) {

            InitBuildTileOverlay(((TilePreset)selectedTilePreset).Create(false, hoverSelection.column));

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

            InitBuildTileOverlay(((TilePreset)selectedTilePreset).Create(false, hoverSelection.column));

            // Re-adds HeightMapChannelPoints (0 = top left, 1 = top right, 2 = bottom left, 3 = bottom right)
            AddHeightObjects(VertexPosition.TopLeft);
            AddHeightObjects(VertexPosition.TopRight);
            AddHeightObjects(VertexPosition.BottomLeft);
            AddHeightObjects(VertexPosition.BottomRight);

        }

    }

    void PreviewSchematicPlacement() {

        if (placementSetting == SchematicPlacementSetting.Exact) {
            schematicBuildOverlay.transform.position =
                new Vector3(hoverSelection.columnWorldX,
                0,
                -hoverSelection.columnWorldY);
        }
        if (placementSetting == SchematicPlacementSetting.Relative) {

            var hoverMax = hoverSelection.tile.GetMaxHeight();
            var schemLow = selectedSchematic.LowestHeight();

            var dif = hoverMax - schemLow;

            schematicBuildOverlay.transform.position =
                new Vector3(hoverSelection.columnWorldX,
                dif / HeightPoints.multiplyer,
                -hoverSelection.columnWorldY);

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

            schematicBuildOverlay.transform.position =
                new Vector3(hoverSelection.columnWorldX,
                0,
                -hoverSelection.columnWorldY);

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
            if (MeshType.IDFromVerticies(t.verticies) == preset.meshID) {
                QuickLogHandler.Log("Tile already exists!", LogSeverity.Error);
                return;
            }

        }

        var tile = preset.Create(true, hoverSelection.column);

        Main.AddCounterAction(new AddTileCounterAction(tile, hoverSelection.column, hoverSelection.section));

        hoverSelection.column.tiles.Add(tile);

        hoverSelection.section.RefreshMesh();

    }

    void PlaceSchematic() {

        if (selectedSchematic == null) { return; }

        if (hoverSelection == null) { return; }

        var affectedSections = new HashSet<LevelMesh>();

        void FinishUp() {
            foreach (var affectSection in affectedSections) {
                affectSection.RefreshMesh();
            }
        }

        // Firse it needs to know the amount of sections that will be mutated for undo
        foreach (var y in Enumerable.Range(hoverSelection.columnWorldY, selectedSchematic.height)) {

            foreach (var x in Enumerable.Range(hoverSelection.columnWorldX, selectedSchematic.width)) {

                var itSection = main.GetLevelMesh(x / 16, y / 16);

                affectedSections.Add(itSection);

            }

        }

        AddSchematicAddCounterAction(affectedSections.ToList());

        // Next it adds all the tiles
        foreach (var y in Enumerable.Range(hoverSelection.columnWorldY, selectedSchematic.height)) {

            foreach (var x in Enumerable.Range(hoverSelection.columnWorldX, selectedSchematic.width)) {

                var itSection = main.GetLevelMesh(x / 16, y / 16);
                var itColumn = itSection.section.GetTileColumn(x % 16, y % 16);

                var itSchemColumn = selectedSchematic.GetTileColumn(x - hoverSelection.columnWorldX, y - hoverSelection.columnWorldY);

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
        var schemLow = selectedSchematic.LowestHeight();

        var dif = hoverMax - schemLow;

        foreach (var y in Enumerable.Range(hoverSelection.columnWorldY, selectedSchematic.height)) {

            foreach (var x in Enumerable.Range(hoverSelection.columnWorldX, selectedSchematic.width)) {

                var itSection = main.GetLevelMesh(x / 16, y / 16);
                var itColumn = itSection.section.GetTileColumn(x % 16, y % 16);

                var itSchemColumn = selectedSchematic.GetTileColumn(x - hoverSelection.columnWorldX, y - hoverSelection.columnWorldY);

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

    #endregion

}