﻿using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class TextureEditMode : TileMutatingEditMode {

    public static bool openUVMapperByDefault = true;

    override public Main main { get; set; }

    public List<TileTexturePreview> selectedTileOverlays = new();
    public TileTexturePreview previewSelectionOverlay = null;
    public List<GameObject> selectedSectionOverlays = new();

    public TextureEditView view;

    public UVPresets currentUVPresets;

    public bool mapperDrawing = false;

    public TextureEditMode(Main main) {
        this.main = main;
    }

    override public void OnCreateMode() {
        currentUVPresets = Presets.uvPresets;
    }

    override public void OnDestroy() {
        view.CloseTextureUVMapper();
        view.CloseTexturePresetPanel();
        ClearAllSelectedItems();
    }

    TileSelection clickSelection = null;
    override public void Update() {

        var selection = main.GetTileOnLevelMesh(!FreeMove.looking);

        if (mapperDrawing) {
            TestPaintTileSelection();
        }
        else if (selection != null) {

            if (Controls.OnDown("Select")) {

                SelectLevelItems(selection);
                clickSelection = selection;
            }
            else if (Controls.IsDown("Select")) {

                if (clickSelection?.tile != selection.tile) {

                    PaintSelection(selection);
                }

            }
            else {

                PreviewSelection(selection);

            }

        }
        else {

            ClearPreviewOverlay();

        }

        if (Main.ignoreAllInputs) { return; }

        if (Controls.OnDown("Unselect")) {
            
            if (view.activeTextureUVMapper != null) {
                view.CloseTextureUVMapper();
            }

            ClearAllSelectedItems();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0)) {

            StartDrawing();

        }
    
    }

    public void StartDrawing() {
        mapperDrawing = true;
        ClearAllSelectedItems();
    }

    #region Selection

    void SelectLevelItems(TileSelection selection) {

        AddSelectionStateCounterAction();

        // If shift is held then multiple tiles can be selected
        if (!Controls.IsDown("MultiSelect")) {

            // Clears the selected tile(s).
            selectedItems.Clear();
            selectedSections.Clear();
            ClearTileOverlays();

        }

        // Since frame animated tiles are a little weird it will try to only have one animated tile selected
        if (selection.tile.GetFrameCount() > 0) {
            QuickLogHandler.Log("Only one animated tile can be selected (Use presets)", LogSeverity.Info);
            selectedItems.Clear();
        }

        if (HasSelection) {

            if (FirstTile.GetFrameCount() > 0) {
                selectedItems.Clear();
            }

        }

        // Selects a range of tiles if the mutli-select modifier is held.
        if (Controls.OnDown("RangeSelect")) {

            if (HasSelection) {

                if (IsTileAlreadySelected(selection.tile)) {

                    SelectRangeOfTiles(selection, FirstItem, !IsTileAlreadySelected(selection.tile));

                }
                else {

                    SelectRangeOfTiles(selection, selectedItems.Last(), !IsTileAlreadySelected(selection.tile));

                }

            }

        } else {

            MakeSelection(selection);

            if (selectedItems.Count == 1) {

                if (view.activeTextureUVMapper != null) {
                    view.activeTextureUVMapper.GetComponent<TextureUVMapper>().RefreshView();
                } else {

                    if (openUVMapperByDefault) {
                        view.OpenUVMapper();
                    }

                }

            }

        }

        RefreshSectionOverlay();

        RefreshMeshes();

    }

    override public void MakeSelection(TileSelection selection, bool deSelectDuplicate = true) {

        if (IsTileAlreadySelected(selection.tile)) {

            if (deSelectDuplicate) {
                RemoveTile(selection.tile);
                RefeshTileOverlay();

                if (!HasSelection) {

                    if (view.activeTextureUVMapper != null) {
                        view.CloseTextureUVMapper();
                    }

                    ClearAllSelectedItems();

                }

            }

        } else {

            selectedItems.Add(selection);
            selectedSections.Add(selection.section);
            InitTileOverlay(selection);

        }

    }

    void PaintSelection(TileSelection selection) {

        var existingSelectedSection = selectedSections.FirstOrDefault(s => s == selection.section);

        MakeSelection(selection, false);

        if (existingSelectedSection == null) {

            ClearSectionOverlays();

            foreach (var iSection in selectedSections) {
                selectedSectionOverlays.Add(Object.Instantiate(main.SectionBoarders, new Vector3(iSection.x, 0, -iSection.y), Quaternion.identity));
            }

        }

    }

    public void DeselectAllButFirstTile() {

        if (selectedItems.Count <= 1) { return; }

        var first = FirstItem;

        selectedItems.Clear();
        selectedSections.Clear();

        MakeSelection(first);

        RefeshTileOverlay();
        RefreshSectionOverlay();

    }

    public void DeselectAllButLastTile() {

        if (selectedItems.Count <= 1) { return; }

        AddSelectionStateCounterAction();

        var last = selectedItems.Last();

        selectedItems.Clear();
        selectedSections.Clear();

        MakeSelection(last);

        RefeshTileOverlay();
        RefreshSectionOverlay();

    }

    void ClearAllSelectedItems() {

        if (selectedItems.Count != 0) {
            AddSelectionStateCounterAction();
        }

        RefreshMeshes();

        selectedItems.Clear();
        selectedSections.Clear();
        ClearTileOverlays();
        ClearSectionOverlays();
        ClearPreviewOverlay();

    }

    #endregion

    #region GameObject Management

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
        script.section = selection.section.section;
        previewSelectionOverlay = script;
        overlay.transform.SetParent(selection.section.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    void RefeshTileOverlay() {

        ClearTileOverlays();

        foreach (var item in selectedItems) {

            InitTileOverlay(item);

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
        script.section = selection.section.section;
        selectedTileOverlays.Add(script);
        overlay.transform.SetParent(selection.section.transform);
        overlay.transform.localPosition = Vector3.zero;

    }

    public void RefreshTileOverlayTexture() {

        foreach (var overly in selectedTileOverlays) {
            overly.Refresh();
        }

    }

    public void ReInitTileOverlayTexture() {

        foreach (var overly in selectedTileOverlays) {
            overly.Refresh(true);
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
        ClearSectionOverlays();
        ClearPreviewOverlay();

    }

    void ReInitGameObjects() {

        RefeshTileOverlay();
        RefreshSectionOverlay();

    }

    #endregion

    #region Model Mutating

    void TestPaintTileSelection() {

        if (FreeMove.looking) {
            return;
        }

        var hover = main.GetTileOnLevelMesh();

        if (hover == null) {
            ClearPreviewOverlay();
            return;
        }

        var mapperObj = view.activeTextureUVMapper;

        if (mapperObj == null) {
            return;
        }

        var mapper = mapperObj.GetComponent<TextureUVMapper>();

        void PaintTile() {

            var tile = hover.tile;

            mapper.ApplyCurrentDataToTile(tile);

        }

        if (Controls.IsDown("Select")) {

            AddNonSelectiveTileStateOrTileStateCounterAction(hover);

            PaintTile();
            ClearAllSelectedItems();
            selectedSections.Add(hover.section);
            RefreshMeshes();
            return;

        }
        else {
            PreviewSelection(hover);
        }

        return;

    }

    public void ChangeTexturePallette(int palletteOffset) {

        foreach (var selection in selectedItems) {

            selection.tile.texturePalette = palletteOffset;

        }

    }

    public void DuplicateTileUVs(bool refresh = true) {

        if (selectedItems.Count < 2) {

            if (refresh) {
                QuickLogHandler.Log("At least two tiles need to be selected to duplicate textures", LogSeverity.Info);
            }


            return; 
        }

        if (refresh) {
            AddTileStateCounterAction();
        }

        foreach (var selection in selectedItems.Skip(1)) {

            if (!IsSameShape(selection.tile)) {

                // If orginal tile is 4 uvs and next tile is 3 uvs
                // If the origianl tile has 3 uvs it can't set them to a tile with 4
                if (selection.tile.uvs.Count == 3) {
                    selection.tile.uvs = new List<int>(FirstTile.uvs.GetRange(0, 3));
                }

            } else {

                selection.tile.uvs = new List<int>(FirstTile.uvs);

            }

            selection.tile.texturePalette = FirstTile.texturePalette;
            selection.tile.isVectorAnimated = FirstTile.isVectorAnimated;
            selection.section.section.animationVector.x = FirstItem.section.section.animationVector.x;
            selection.section.section.animationVector.y = FirstItem.section.section.animationVector.y;

        }


        if (refresh) {
            RefreshMeshes();
            RefreshUVMapper();
            RefreshTileOverlayTexture();
        }

    }

    public void MakeTilesOpaque() {

        if (!HasSelection) {
            QuickLogHandler.Log("No tiles are selected", LogSeverity.Info);

            return;
        }

        AddTileStateCounterAction();

        foreach (var selection in selectedItems) {
            selection.tile.isSemiTransparent = false;
        }

        RefreshMeshes();
        RefreshTileOverlayTexture();

        QuickLogHandler.Log("Selected tiles made opaque", LogSeverity.Success);


    }

    public void MakeTilesTransparent() {

        if (!HasSelection) {
            QuickLogHandler.Log("No tiles are selected", LogSeverity.Info);
            return;
        }

        AddTileStateCounterAction();

        foreach (var selection in selectedItems) {
            selection.tile.isSemiTransparent = true;
        }

        RefreshMeshes();
        RefreshTileOverlayTexture();

        QuickLogHandler.Log("Selected tiles made transparent", LogSeverity.Success);


    }

    #endregion

    #region View Management

    public void RefreshUVMapper() {

        if (view.activeTextureUVMapper != null) {
            view.activeTextureUVMapper.GetComponent<TextureUVMapper>().RefreshView();
        }

    }

    public bool AddPreset() {

        if (!HasSelection) { return false; }

        var firstTile = FirstTile;

        var potentialID = MeshType.IDFromVerticies(firstTile.verticies);

        if (potentialID == null) {
            DialogWindowUtil.Dialog("Cannot Save Preset", "Cannot save preset, tile mesh is invalid!");
            return false;
        }

        var uvPreset = new UVPreset(
            firstTile.uvs,
            firstTile.texturePalette,
            "",
            (int)potentialID,
            firstTile.isSemiTransparent,
            firstTile.isVectorAnimated,
            firstTile.animationSpeed,
            firstTile.animatedUVs
            );

        currentUVPresets.presets.Add(uvPreset);

        return true;

    }

    public void AddPresetsDirectory() {

        currentUVPresets.subFolders.Add(new UVPresets("", currentUVPresets));

    }

    #endregion

    #region Counter-Actions

    public class VectorAnimationCounterAction : CounterAction {

        public string name { get; set; }

        List<LevelMesh> modifiedSections;
        int saveX;
        int saveY;

        public VectorAnimationCounterAction(HashSet<LevelMesh> modifiedSections, int saveX, int saveY) {
            this.modifiedSections = modifiedSections.ToList();
            this.saveX = saveX;
            this.saveY = saveY;

            name = "Vector Animation Change";
        }

        public void Action() {

            if (Main.editMode is not TextureEditMode) {
                return;
            }

            foreach (var modifiedSection in modifiedSections) {

                modifiedSection.section.animationVector.x = saveX;
                modifiedSection.section.animationVector.y = saveY;

            }

            var editMode = (TextureEditMode)Main.editMode;

            editMode.RefreshMeshes();
            editMode.RefreshTileOverlayTexture();
            editMode.RefreshUVMapper();

        }

    }

    public void AddVectorAnimationCounterAction() {
        Main.AddCounterAction(new VectorAnimationCounterAction(selectedSections, FirstItem.section.section.animationVector.x, FirstItem.section.section.animationVector.y));
    }

    public static void AddTileStateCounterAction() {

        Main.AddCounterAction(new MultiTileSaveStateCounterAction(((TextureEditMode)Main.editMode).selectedItems, () => {

            if (Main.editMode is not TextureEditMode) {
                return;
            }

            var editMode = (TextureEditMode)Main.editMode;

            editMode.RefreshMeshes();
            editMode.RefreshTileOverlayTexture();
            editMode.RefreshUVMapper();

        }));

    }

    static void AddSelectionStateCounterAction() {

        Main.AddCounterAction(new SelectionSaveStateCounterAction(((TextureEditMode)Main.editMode), () => {

            if (Main.editMode is not TextureEditMode) {
                return;
            }

            var editMode = (TextureEditMode)Main.editMode;

            editMode.ClearAllGameObjects();
            editMode.ReInitGameObjects();
            editMode.RefreshUVMapper();

        }));

    }

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

                var editMode = (TextureEditMode)Main.editMode;

                editMode.RefeshTileOverlay();

            }));

        }

    }

    #endregion

}
