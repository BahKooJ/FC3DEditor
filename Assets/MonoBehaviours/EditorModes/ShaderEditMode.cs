


using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Presets;
using UnityEngine;
using Object = UnityEngine.Object;

public class ShaderEditMode : TileMutatingEditMode {

    public static readonly List<EditingTool> toolList = new() { 
        new EditingTool("default"), 
        new EditingTool("clickColorApply"), 
        new EditingTool("paint", true), 
        new EditingTool("presetPaint", true) };
    public static bool showColorPresets = false;

    override public Main main { get; set; }

    public List<TileTexturePreview> selectedTileOverlays = new();
    public TileTexturePreview previewSelectionOverlay = null;
    public List<GameObject> selectedSectionOverlays = new();
    public List<VertexColorPoint> vertexColorPoints = new();

    public ShaderEditPanelView view;
    public ShaderColorPickerView colorPicker;

    public ShaderPresets currentShaderPresets;
    public ColorPresets currentColorPresets;

    public Tile previousPaintTile = null;
    public ShaderPreset selectedPreset = null;

    public EditingTool selectedTool = toolList[0];
    public bool Default {
        get { return selectedTool.name == "default"; }
    }
    public bool Painting {
        get { return selectedTool.name == "paint"; }
    }
    public bool ColorClick {
        get { return selectedTool.name == "clickColorApply";}
    }
    public bool PresetPainting {
        get { return selectedTool.name == "presetPaint"; }
    }


    public ShaderEditMode(Main main) {
        this.main = main;
    }

    override public void Update() {

        var testForSelection = true;

        var hover = main.GetTileOnLevelMesh(!FreeMove.looking);
        
        if (PresetPainting) {

            var didMonoHit = TestPaintTileSelection();

            testForSelection = !didMonoHit;

        }
        else if (colorPicker != null) {

            if (colorPicker.colorType == VertexColorType.MonoChrome) {

                var didMonoHit = TestSolidMonoTileSelection();

                testForSelection = !didMonoHit;

            }
            else {

                var didHitCorner = TestVertexColorCornerSelection();

                testForSelection = didHitCorner ? false : testForSelection;

                if (Painting) {

                    if (hover == null) {
                        ClearVertexColors();
                    }
                    else {

                        if (vertexColorPoints.Count == 0) {
                            ClearVertexColors();
                            InitPaintVertexColorCorners(hover);
                            previousPaintTile = hover.tile;
                        }
                        else if (previousPaintTile != hover.tile) {
                            ClearVertexColors();
                            InitPaintVertexColorCorners(hover);
                            previousPaintTile = hover.tile;
                        }

                    }

                }

            }

        }

        // This is here after it's tested all other raycasts
        // That way there isn't unintentinal tile selection
        if (testForSelection && hover != null) {

            if (Painting) {
                if (FreeMove.looking && Controls.OnDown("Select")) {
                    SelectLevelItems(hover);
                }
                else if (Controls.OnDown("SelectWhilePainting")) {
                    SelectLevelItems(hover);
                }
            }
            else {

                if (Controls.OnDown("Select")) {
                    SelectLevelItems(hover);
                }
                else {

                    PreviewSelection(hover);

                }

            }

        }
        else if (previewSelectionOverlay != null) {

            ClearPreviewOverlay();

        }


        if (Main.ignoreAllInputs) { return; }

        if (Controls.OnDown("Save")) {
            if (view.activeShaderMapper != null) {
                view.CloseShaderMapper();
            }
        }

        if (Controls.OnDown("Unselect")) {

            ClearAllSelectedItems();

        }

    }

    override public void OnCreateMode() {
        currentShaderPresets = Presets.shaderPresets;
        currentColorPresets = Presets.colorPresets;
    }

    override public void OnDestroy() {
        view.CloseShaderMapper();
        view.ClosePresetPanel();
        ClearAllSelectedItems();
    }

    #region Selection

    public void SelectLevelItems(TileSelection selection) {

        AddSelectionStateCounterAction();

        // If shift is held then multiple tiles can be selected
        if (!Controls.IsDown("MultiSelect")) {

            // Clears the selected tile(s).
            selectedItems.Clear();
            selectedSections.Clear();

        }

        // Selects a range of tiles if the mutli-select modifier is held.
        if (Controls.OnDown("RangeSelect") && !selectedTool.selectionChanges) {

            if (HasSelection) {

                SelectRangeOfTiles(selection);

            }

        }
        else {

            MakeSelection(selection);

        }

        RefreshSectionOverlay();

        if (colorPicker != null && Default && HasSelection) {

            colorPicker.colorType = FirstTile.shaders.type;

            if (colorPicker.colorType == VertexColorType.MonoChrome && FirstTile.shaders.type == VertexColorType.MonoChrome) {
                colorPicker.SelectedVertexColor(0);
            }

            colorPicker.RefreshView();

        }

        if (!selectedTool.selectionChanges && HasSelection) {
            ClearVertexColors();
            InitVertexColorCorners(selectedItems[0]);
        }

        RefeshTileOverlay();
        selection.section.RefreshMesh();

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

    void PreviewSelection(TileSelection selection) {

        if (previewSelectionOverlay != null) {

            if (previewSelectionOverlay.tile == selection.tile) {
                return;
            }

            ClearPreviewOverlay();

        }

        var overlay = Object.Instantiate(main.TileTexturePreview);
        var script = overlay.GetComponent<TileTexturePreview>();
        script.controller = main;
        script.tile = selection.tile;
        script.showShaders = true;
        previewSelectionOverlay = script;
        overlay.transform.SetParent(selection.section.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    void ClearAllSelectedItems() {

        AddSelectionStateCounterAction();

        RefreshMeshes();

        selectedItems.Clear();
        selectedSections.Clear();
        ClearTileOverlays();
        ClearPreviewOverlay();

        ClearSectionOverlays();
        ClearVertexColors();

    }

    #endregion

    #region Tool Management

    public void SelectTool(int index) {

        selectedPreset = null;
        view.shaderPresetsView.RefreshSelection();

        if (selectedTool.name == toolList[index].name) {
            selectedTool = toolList[0];
            ClearAllSelectedItems();
            return;
        }

        selectedTool = toolList[index];

        if (Painting) {
            StartPainting();
        }
        else if (ColorClick) {
            ChangeClickToggle();
        }
        else if (PresetPainting) {
            selectedPreset = null;
            
        }

    }

    public void StartPainting() {
        
        ClearVertexColors();
        

    }

    public void ChangeClickToggle() {

        foreach (var vertex in vertexColorPoints) {
            vertex.Deselect();
        }

    }

    #endregion

    #region GameObject Management

    void RefeshTileOverlay() {

        ClearTileOverlays();

        foreach (var tile in selectedItems) {

            InitTileOverlay(tile);

        }

    }

    void RefreshSectionOverlay() {

        ClearSectionOverlays();

        foreach (var iSection in selectedSections) {
            selectedSectionOverlays.Add(Object.Instantiate(main.SectionBoarders, new Vector3(iSection.x, 0, -iSection.y), Quaternion.identity));
        }

    }

    void InitTileOverlay(TileSelection selection) {

        var overlay = Object.Instantiate(main.TileTexturePreview);
        var script = overlay.GetComponent<TileTexturePreview>();
        script.controller = main;
        script.tile = selection.tile;
        script.showShaders = true;
        selectedTileOverlays.Add(script);
        overlay.transform.SetParent(selection.section.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    void InitPaintVertexColorCorners(TileSelection hover) {

        var startX = hover.column.x - 1;
        var Xcount = 3;

        if (startX < 0) {
            startX = 0;
            Xcount = 2;
        }
        if (startX == 14) {
            startX = 14;
            Xcount = 2;
        }

        var startY = hover.column.y - 1;
        var Ycount = 3;

        if (startY < 0) {
            startY = 0;
            Ycount = 2;
        }
        if (startY == 14) {
            startY = 14;
            Ycount = 2;
        }

        foreach (var y in Enumerable.Range(startY, Ycount)) {

            foreach (var x in Enumerable.Range(startX, Xcount)) {

                var itColumn = hover.section.section.GetTileColumn(x, y);

                foreach (var tile in itColumn.tiles) {

                    InitVertexColorCorners(new TileSelection(tile, itColumn, hover.section));

                }

            }

        }

    }

    void InitVertexColorCorners(TileSelection tile) {

        if (!HasSelection && !Painting) {
            return;
        }
        if (colorPicker != null) {

            if (colorPicker.colorType == VertexColorType.MonoChrome) {
                return;
            }

        }
        if (Default) {

            if (FirstTile.shaders.type == VertexColorType.MonoChrome) {
                return;
            }

        }


        foreach (var i in Enumerable.Range(0, tile.tile.verticies.Count)) {
            InitSingleVertexColorCorner(i, tile);
        }

    }

    void InitSingleVertexColorCorner(int index, TileSelection tile) {

        var worldX = tile.section.x + tile.column.x;
        var worldY = -(tile.section.y + tile.column.y);

        var tileVert = tile.tile.verticies[index];

        switch (tileVert.vertexPosition) {
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

        var pos = new Vector3(worldX, tile.column.heights[(int)tileVert.vertexPosition - 1].GetPoint(tileVert.heightChannel), worldY);


        var obj = Object.Instantiate(main.vertexColorPoint, pos, Quaternion.identity);
        var vertPoint = obj.GetComponent<VertexColorPoint>();
        vertPoint.controller = this;
        vertPoint.selectedItem = tile;
        vertPoint.index = index;

        vertexColorPoints.Add(vertPoint);

    }

    void ClearVertexColors() {

        foreach (var vertexColorPoint in vertexColorPoints) {
            Object.Destroy(vertexColorPoint.gameObject);
        }

        vertexColorPoints.Clear();

    }

    void RefreshVertexColorCorners() {

        foreach (var vertexColorPoint in vertexColorPoints) {
            vertexColorPoint.RefreshColors();
        }

    }

    public void RefreshVertexColors() {

        if (Painting) return;

        if (colorPicker == null) return;

        if (!HasSelection) return;

        ClearVertexColors();

        if (colorPicker.colorType != VertexColorType.MonoChrome) {
            InitVertexColorCorners(selectedItems[0]);
        }

    }

    public void RefreshTileOverlayShader() {

        foreach (var overly in selectedTileOverlays) {
            overly.Refresh();
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

    public void RefreshShaderMapper() {

        if (view.activeShaderMapper != null) {
            view.activeShaderMapper.GetComponent<ShaderColorPickerView>().RefreshView();
        }

    }

    #endregion

    #region Model Mutating

    bool TestVertexColorCornerSelection() {

        if (FreeMove.looking) {
            return false;
        }

        if (vertexColorPoints.Count == 0) {
            return false;
        }

        if (!Controls.OnDown("Select") && !Controls.OnDown("Interact") && !Painting) {
            return false;
        }

        var didHit = false;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        var hits = Physics.RaycastAll(ray, Mathf.Infinity, 8);

        foreach (var hit in hits) {

            foreach (var vertex in vertexColorPoints) {

                if (hit.colliderInstanceID == vertex.boxCollider.GetInstanceID()) {

                    didHit = true;

                    if (Painting) {

                        if (Controls.IsDown("Select")) {

                            if (HasSelection) {

                                var index = selectedItems.FindIndex(item => {
                                    return item.tile == vertex.selectedItem.tile;
                                });

                                if (index != -1) {

                                    AddNonSelectiveTileStateOrTileStateCounterAction(vertex.selectedItem);

                                    vertex.ChangeValue();

                                    OverrideWhite(vertex.selectedItem);

                                    RefreshTileOverlayShader();


                                }

                            }
                            else {

                                AddNonSelectiveTileStateOrTileStateCounterAction(vertex.selectedItem);

                                vertex.ChangeValue();

                                OverrideWhite(vertex.selectedItem);

                            }

                        }

                        continue;

                    }
                    else if (Controls.OnDown("Select")) {

                        if (ColorClick) {

                            AddTileStateCounterAction();

                            vertex.ChangeValue();

                            OverrideWhite(vertex.selectedItem);

                            RefreshTileOverlayShader();
                            RefreshVertexColorCorners();


                        }
                        else {

                            if (Controls.IsDown("MultiSelect")) {
                                vertex.SelectOrDeselect();
                            }
                            else {

                                foreach (var vert in vertexColorPoints) {
                                    vert.Deselect();
                                }

                                vertex.Select();

                            }

                        }

                    }

                }

            }

        }

        return didHit;

    }

    // This is for solid monochrome
    bool TestSolidMonoTileSelection() {

        if (FreeMove.looking) {
            return false;
        }

        var hover = main.GetTileOnLevelMesh();

        void PaintTile() {

            var tile = hover.tile;

            if (tile.shaders.type != colorPicker.colorType) {
                tile.ChangeShader(colorPicker.colorType);
            }

            var shaderSolid = (MonoChromeShader)tile.shaders;

            shaderSolid.value = colorPicker.solidMonoByteValue;

            shaderSolid.Apply();
        }


        if (hover == null) {
            ClearPreviewOverlay();
            return false;
        }
        if (Painting) {

            if (Controls.IsDown("Select")) {

                AddNonSelectiveTileStateOrTileStateCounterAction(hover);

                PaintTile();
                ClearAllSelectedItems();
                selectedSections.Add(hover.section);
                RefreshMeshes();
                return true;
            }
            else {
                PreviewSelection(hover);
            }

        }
        else if (ColorClick) {

            if (Controls.OnDown("Select")) {

                SelectLevelItems(hover);

                AddTileStateCounterAction(true);

                PaintTile();
                RefreshMeshes();
                return true;
            }

        }
        else {

            if (Controls.OnDown("Select")) {

                SelectLevelItems(hover);
                return true;

            }

        }

        return false;

    }

    bool TestPaintTileSelection() {

        if (FreeMove.looking) {
            return false;
        }

        var hover = main.GetTileOnLevelMesh();

        if (hover == null) {
            ClearPreviewOverlay();
            return false;
        }

        if (selectedPreset == null) {
            return false;
        }

        void PaintTile() {

            var tile = hover.tile;

            if (tile.shaders.isQuad == selectedPreset.shader.isQuad) {

                tile.shaders = selectedPreset.shader.Clone();

            }

        }

        if (Controls.IsDown("Select")) {

            AddNonSelectiveTileStateOrTileStateCounterAction(hover);

            PaintTile();
            ClearAllSelectedItems();
            selectedSections.Add(hover.section);
            RefreshMeshes();
            return true;

        }
        else {
            PreviewSelection(hover);
        }

        return false;

    }

    public void ApplyColorsToVertexColorCorners() {

        if (!Default) {
            return;
        }

        // Remember that this method checks for mouse inputs so nothing is mutli-added.
        AddTileStateCounterAction();

        foreach (var colorPoint in vertexColorPoints) {

            if (colorPoint.isSelected) {
                colorPoint.ChangeValue();
            }

        }
        RefreshTileOverlayShader();

    }

    public void ApplySolidMonoToTile() {

        if (selectedItems.Count > 0) {

            AddTileStateCounterAction();

        }

        foreach (var item in selectedItems) {

            var tile = item.tile;

            if (tile.shaders.type != colorPicker.colorType) {
                tile.ChangeShader(colorPicker.colorType);
            }

            var shaderSolid = (MonoChromeShader)tile.shaders;

            shaderSolid.value = colorPicker.solidMonoByteValue;

            shaderSolid.Apply();

        }

        RefreshTileOverlayShader();

    }

    void OverrideWhite(TileSelection selection) {

        if (!ShaderColorPickerView.overrideWhite) {
            return;
        }

        foreach (var point in vertexColorPoints) {

            if (point.selectedItem.tile == selection.tile) {
                point.OverrideWhite();
            }

        }

    }

    public void DuplicateTileShader() {

        if (selectedItems.Count < 2) {

            QuickLogHandler.Log("At least two tiles need to be selected to duplicate textures", LogSeverity.Info);

            return;
        }

        AddTileStateCounterAction();

        foreach (var selection in selectedItems.Skip(1)) {

            selection.tile.shaders = FirstTile.shaders.Clone();

        }

        RefreshMeshes();

        RefreshShaderMapper();
        RefreshTileOverlayShader();

    }

    #endregion

    #region Presets

    public bool AddPreset() {

        if (selectedItems.Count == 0) { return false; }

        if (FirstTile.shaders is AnimatedShader) { return false; }

        var potentialID = MeshType.IDFromVerticies(FirstTile.verticies);

        if (potentialID == null) {
            DialogWindowUtil.Dialog("Cannot Save Preset", "Cannot save shader preset, tile mesh is invalid!");
            return false;
        }

        var shaderPreset = new ShaderPreset(FirstTile.shaders, "", (int)potentialID);

        currentShaderPresets.presets.Add(shaderPreset);

        return true;

    }

    public bool AddColorPreset() {

        if (colorPicker == null) return false;

        if (colorPicker.colorType == VertexColorType.ColorAnimated) return false;

        ColorPreset preset = null;

        switch (colorPicker.colorType) {
            case VertexColorType.MonoChrome:
                preset = new ColorPreset("", VertexColorType.MonoChrome, colorPicker.solidMonoByteValue);
                break;
            case VertexColorType.DynamicMonoChrome:
                preset = new ColorPreset("", VertexColorType.DynamicMonoChrome, colorPicker.dynamicMonoValue);
                break;
            case VertexColorType.Color:
                preset = new ColorPreset("", VertexColorType.Color, colorPicker.colorValue.Clone());
                break;
        }

        currentColorPresets.presets.Add(preset);

        return true;

    }

    public void AddPresetsDirectory() {

        currentShaderPresets.subFolders.Add(new ShaderPresets("", currentShaderPresets));

    }

    public void AddColorPresetsDirectory() {

        currentColorPresets.subFolders.Add(new ColorPresets("", currentColorPresets));

    }

    #endregion

    #region Counter-Actions

    public static void AddNonSelectiveTileStateOrTileStateCounterAction(TileSelection item) {

        if (Main.counterActionAddedOnCurrentSelectHold) {

            var last = Main.counterActions.Last();

            var tileCounterAction = (NonSelectiveMultiTileSaveStateCounterAction)last;

            tileCounterAction.AddTileSaveState(item);

        }
        else {

            // No need to refresh meshes as the class already does that.
            Main.AddCounterAction(new NonSelectiveMultiTileSaveStateCounterAction(item, () => {

                if (Main.editMode is not ShaderEditMode) {
                    return;
                }

                var editMode = (ShaderEditMode)Main.editMode;

                editMode.RefeshTileOverlay();
                editMode.RefreshVertexColors();

            }));

        }

    }

    public static void AddTileStateCounterAction(bool ignoreSelectionCheck = false) {

        Main.AddCounterAction(new MultiTileSaveStateCounterAction(((ShaderEditMode)Main.editMode).selectedItems, () => {

            if (Main.editMode is not ShaderEditMode) {
                return;
            }

            var editMode = (ShaderEditMode)Main.editMode;

            editMode.RefreshMeshes();
            editMode.RefeshTileOverlay();
            editMode.RefreshVertexColors();

        }), ignoreSelectionCheck);

    }

    static void AddSelectionStateCounterAction() {

        Main.AddCounterAction(new SelectionSaveStateCounterAction(((ShaderEditMode)Main.editMode), () => {

            if (Main.editMode is not ShaderEditMode) {
                return;
            }

            var editMode = (ShaderEditMode)Main.editMode;

            editMode.RefeshTileOverlay();
            editMode.RefreshSectionOverlay();
            editMode.ClearVertexColors();
            editMode.RefreshVertexColors();

        }));

    }

    #endregion

}