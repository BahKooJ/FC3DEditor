

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
            section.section.parser.tileEffects[channel] = (byte)value;
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

    }

    #endregion


    #region Counter-Actions

    // Counter-Action methods must be static
    // This is to avoid memory leaks.

    public class TileEffectChangeCounterAction : CounterAction {

        FCopLevelSection modifiedSection;
        byte value;
        int index;

        public TileEffectChangeCounterAction(FCopLevelSection modifiedSection, byte value, int index) {
            this.modifiedSection = modifiedSection;
            this.value = value;
            this.index = index;
        }

        public void Action() {

            modifiedSection.tileEffects[index] = value;

        }

    }

    public class MultiTileEffectChangeCounterAction : CounterAction {

        List<TileEffectChangeCounterAction> counterActions = new();

        public MultiTileEffectChangeCounterAction(HashSet<LevelMesh> changedSections, int index) {

            foreach (var sectionMesh in changedSections) {

                counterActions.Add(new TileEffectChangeCounterAction(
                    sectionMesh.section,
                    sectionMesh.section.tileEffects[index],
                    index
                    ));

            }

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

        Tile removedTile;
        TileColumn column;

        public RemoveTileCounterAction(Tile removedTile, TileColumn column) {
            this.removedTile = removedTile;
            this.column = column;
        }

        public void Action() {

            column.tiles.Add(removedTile);

        }


    }

    public class MultiRemoveTileCounterAction : CounterAction {

        List<RemoveTileCounterAction> removedTileCounterActions = new();

        public MultiRemoveTileCounterAction(List<TileSelection> items) {

            foreach (var item in items) {

                removedTileCounterActions.Add(new RemoveTileCounterAction(item.tile, item.column));

            }

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

        Tile addedTile;
        TileColumn columnAddedTo;

        public PassiveAddTileCounterAction(Tile addedTile, TileColumn columnAddedTo) {
            this.addedTile = addedTile;
            this.columnAddedTo = columnAddedTo;
        }

        public void Action() {

            columnAddedTo.tiles.Remove(addedTile);

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

    #endregion

}