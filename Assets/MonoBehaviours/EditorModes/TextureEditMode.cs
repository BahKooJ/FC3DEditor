using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class TextureEditMode : TileMutatingEditMode, EditMode {

    public static bool openUVMapperByDefault = true;

    public Main main { get; set; }

    public List<TileTexturePreview> selectedTileOverlays = new();
    public TileTexturePreview previewSelectionOverlay = null;
    public List<GameObject> selectedSectionOverlays = new();

    public TextureEditView view;

    public UVPresets currentUVPresets;

    public TextureEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {
        currentUVPresets = Presets.uvPresets;
    }

    public void OnDestroy() {
        view.CloseTextureUVMapper();
        view.CloseTexturePresetPanel();
        ClearAllSelectedItems();
    }

    public void Update() {

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

        if (Main.ignoreAllInputs) { return; }

        if (Controls.OnDown("Unselect")) {
            
            if (view.activeTextureUVMapper != null) {
                view.CloseTextureUVMapper();
            }

            ClearAllSelectedItems();
        }
    
    }

    void SelectLevelItems(TileSelection selection) {

        // If shift is held then multiple tiles can be selected
        if (!Controls.IsDown("ModifierMultiSelect")) {

            // Clears the selected tile(s).
            selectedItems.Clear();
            selectedSections.Clear();

        }

        // Since frame animated tiles are a little weird it will try to only have one animated tile selected
        if (selection.tile.GetFrameCount() > 0) {
            selectedItems.Clear();
        }

        if (HasSelection) {

            if (FirstTile.GetFrameCount() > 0) {
                selectedItems.Clear();
            }

        }

        // Selects a range of tiles if the mutli-select modifier is held.
        if (Controls.IsDown("ModifierAltSelect") && Controls.IsDown("ModifierMultiSelect")) {

            if (HasSelection) {

                SelectRangeOfTiles(selection);

            }

        }
        else {

            MakeSelection(selection);

            if (selectedItems.Count == 1) {

                if (view.activeTextureUVMapper != null) {
                    view.activeTextureUVMapper.GetComponent<TextureUVMapper>().RefreshView();
                }
                else {

                    if (openUVMapperByDefault) {
                        view.OpenUVMapper();
                    }

                }

            }

        }

        RefreshSectionOverlay();

        RefeshTileOverlay();
        RefreshMeshes();

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

                    if (view.activeTextureUVMapper != null) {
                        view.CloseTextureUVMapper();
                    }

                    ClearAllSelectedItems();

                }

            }

        }
        else {

            selectedItems.Add(selection);
            selectedSections.Add(selection.section);

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

        var last = selectedItems.Last();

        selectedItems.Clear();
        selectedSections.Clear();

        MakeSelection(last);

        RefeshTileOverlay();
        RefreshSectionOverlay();

    }

    void ClearAllSelectedItems() {

        RefreshMeshes();

        selectedItems.Clear();
        selectedSections.Clear();
        ClearTileOverlays();
        ClearSectionOverlays();
        ClearPreviewOverlay();

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

    public void RefreshUVMapper() {

        if (view.activeTextureUVMapper != null) {
            view.activeTextureUVMapper.GetComponent<TextureUVMapper>().RefreshView();
        }

    }

    public void ChangeTexturePallette(int palletteOffset) {

        foreach (var selection in selectedItems) {

            selection.tile.texturePalette = palletteOffset;

        }

    }

    public void DuplicateTileUVs(bool refresh = true) {

        if (selectedItems.Count < 2) return;

        foreach (var selection in selectedItems.Skip(1)) {

            if (!IsSameShape(selection.tile)) {

                // If orginal tile is 4 uvs and next tile is 3 uvs
                // If the origianl tile has 3 uvs it can't set them to a tile with 4
                if (selection.tile.uvs.Count == 3) {
                    selection.tile.uvs = new List<int>(FirstTile.uvs.GetRange(0,3));
                }

            }
            else {

                selection.tile.uvs = new List<int>(FirstTile.uvs);
                selection.tile.texturePalette = FirstTile.texturePalette;

            }

        }


        if (refresh) {
            RefreshMeshes();
            RefreshUVMapper();
            RefreshTileOverlayTexture();
        }

    }

    public void MakeTilesOpaque() {

        foreach (var selection in selectedItems) {
            selection.tile.isSemiTransparent = false;
        }

        RefreshMeshes();

    }

    public void MakeTilesTransparent() {

        foreach (var selection in selectedItems) {
            selection.tile.isSemiTransparent = true;
        }

        RefreshMeshes();

    }

    // UV Presets

    public bool AddPreset() {

        if (!HasSelection) { return false; }

        var firstTile = FirstTile;

        var potentialID = MeshType.IDFromVerticies(firstTile.verticies);

        if (potentialID == null) {
            DialogWindowUtil.Dialog("Cannot Save Preset", "Cannot save shader preset, tile mesh is invalid!");
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

}
