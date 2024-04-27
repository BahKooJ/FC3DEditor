


using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class ShaderEditMode : TileMutatingEditMode, EditMode {

    public static bool applyColorsOnClick = false;
    public static bool showColorPresets = false;

    public Main main { get; set; }

    public List<TileTexturePreview> selectedTileOverlays = new();
    public List<GameObject> selectedSectionOverlays = new();
    public List<VertexColorPoint> vertexColorPoints = new();

    public ShaderEditPanelView view;
    public ShaderColorPickerView colorPicker;

    public ShaderPresets currentShaderPresets;
    public ColorPresets currentColorPresets;

    public bool painting = false;
    public Tile previousPaintTile = null;

    public ShaderEditMode(Main main) {
        this.main = main;
    }

    public void StartPainting() {

        painting = !painting;

        if (!painting) {
            ClearVertexColors();
        }

    }

    public void ChangeClickToggle() {

        applyColorsOnClick = !applyColorsOnClick;

        foreach (var vertex in vertexColorPoints) {
            vertex.Deselect();
        }

    }

    public void ApplyColorsToVertexColorCorners() {

        if (applyColorsOnClick) {
            return;
        }

        if (painting) {
            return;
        }

        foreach (var colorPoint in vertexColorPoints) {

            if (colorPoint.isSelected) {
                colorPoint.ChangeValue();
            }

        }
        RefreshTileOverlayShader();

    }

    public void ApplySolidMonoToTile() {

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

    public void Update() {

        if (FreeMove.looking) {
            //main.TestRayOnLevelMesh();
        }

        if (Main.ignoreAllInputs) { return; }

        if (colorPicker != null) {

            if (colorPicker.colorType == VertexColorType.MonoChrome) {

                TestTileSelection();

            }
            else {

                TestVertexColorCornerSelection();

                if (painting) {

                    var hover = main.GetTileOnLevelMesh();

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

        if (Controls.OnDown("Save")) {
            if (view.activeShaderMapper != null) {
                view.CloseShaderMapper();
            }
        }

        if (Controls.OnDown("Unselect")) {

            ClearAllSelectedItems();

        }

    }

    public void OnCreateMode() {
        currentShaderPresets = Presets.shaderPresets;
        currentColorPresets = Presets.colorPresets;
    }

    public void OnDestroy() {
        view.CloseShaderMapper();
        view.ClosePresetPanel();
        ClearAllSelectedItems();
    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {

    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

        // If shift is held then multiple tiles can be selected
        if (!Controls.IsDown("ModifierMultiSelect")) {

            // Clears the selected tile(s).
            selectedItems.Clear();
            selectedSections.Clear();

        }

        // Selects a range of tiles if the mutli-select modifier is held.
        if (Controls.IsDown("ModifierAltSelect") && Controls.IsDown("ModifierMultiSelect")) {

            if (HasSelection) {

                SelectRangeOfTiles(new TileSelection(tile, column, section));

            }

        }

        // Checks to see if the tiles vertex count is the same as the first selected tile
        // This needs to be done because there are many differences in triangle tiles and rect tiles
        // TODO: Make it so it doesn't matter
        else if (selectedItems.Count == 0) {

            MakeSelection(tile, column, section);

        }
        else {

            MakeSelection(tile, column, section);

        }

        ClearSectionOverlays();

        foreach (var iSection in selectedSections) {
            selectedSectionOverlays.Add(Object.Instantiate(main.SectionBoarders, new Vector3(iSection.x, 0, -iSection.y), Quaternion.identity));
        }

        if (colorPicker != null && !applyColorsOnClick && !painting && HasSelection) {

            colorPicker.colorType = FirstTile.shaders.type;

            if (colorPicker.colorType == VertexColorType.MonoChrome && FirstTile.shaders.type == VertexColorType.MonoChrome) {
                colorPicker.SelectedVertexColor(0);
            }

            colorPicker.RefreshView();

        }

        if (!painting && HasSelection) {
            ClearVertexColors();
            InitVertexColorCorners(selectedItems[0]);
        }

        RefeshTileOverlay();

    }

    override public void MakeSelection(Tile tile, TileColumn column, LevelMesh section, bool deSelectDuplicate = true) {

        if (IsTileAlreadySelected(tile)) {

            if (deSelectDuplicate) {
                RemoveTile(tile);
                RefeshTileOverlay();

                if (!HasSelection) {

                    ClearAllSelectedItems();

                }

            }

        }
        else {

            selectedItems.Add(new TileSelection(tile, column, section));
            selectedSections.Add(section);

        }

    }

    void OverrideWhite() {

        if (!ShaderColorPickerView.overrideWhite) {
            return;
        }

        foreach (var point in vertexColorPoints) {

            point.OverrideWhite();

        }

    }

    void TestVertexColorCornerSelection() {

        if (FreeMove.looking) {
            return;
        }

        if (vertexColorPoints.Count == 0) {
            return;
        }

        if (!Controls.OnDown("Select") && !Controls.OnDown("Interact") && !painting) {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        var hits = Physics.RaycastAll(ray, Mathf.Infinity, 8);

        foreach (var hit in hits) {

            foreach (var vertex in vertexColorPoints) {

                if (hit.colliderInstanceID == vertex.boxCollider.GetInstanceID()) {

                    if (painting) {

                        if (Controls.IsDown("Select")) {

                            if (HasSelection) {

                                var index = selectedItems.FindIndex(item => {
                                    return item.tile == vertex.selectedItem.tile;
                                });

                                if (index != -1) {

                                    vertex.ChangeValue();

                                    OverrideWhite();

                                    RefreshTileOverlayShader();


                                }

                            } else {

                                vertex.ChangeValue();

                                OverrideWhite();

                            }

                        }

                        continue;

                    }

                    else if (Controls.OnDown("Select")) {

                        if (applyColorsOnClick) {

                            vertex.ChangeValue();

                            OverrideWhite();

                            RefreshTileOverlayShader();
                            RefreshVertexColorCorners();


                        }
                        else {

                            if (Controls.IsDown("ModifierMultiSelect")) {
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

    }

    // This is for solid monochrome
    void TestTileSelection() {

        if (FreeMove.looking) {
            return;
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
            return;
        }
        if (painting) {

            if (Controls.IsDown("Select")) {
                PaintTile();
                ClearAllSelectedItems();
                selectedSections.Add(hover.section);
                RefreshMeshes();
            }

        }
        else if (applyColorsOnClick) {

            if (Controls.OnDown("Select")) {
                PaintTile();
                SelectTile(hover.tile, hover.column, hover.section);
                RefreshMeshes();

            }

        }
        else {

            if (Controls.OnDown("Select")) {

                SelectTile(hover.tile, hover.column, hover.section);

            }

        }

    }

    void ClearAllSelectedItems() {

        RefreshMeshes();

        selectedItems.Clear();
        selectedSections.Clear();
        ClearTileOverlays();

        ClearSectionOverlays();
        ClearVertexColors();

    }

    void RefeshTileOverlay() {

        ClearTileOverlays();

        foreach (var tile in selectedItems) {

            InitTileOverlay(tile);

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

        if (!HasSelection && !painting) {
            return;
        }
        if (colorPicker != null) {

            if (colorPicker.colorType == VertexColorType.MonoChrome) {
                return;
            }

        }
        if (!painting && !applyColorsOnClick) {

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

        if (painting) return;

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

    public void DuplicateTileShader() {

        if (selectedItems.Count < 2) return;

        foreach (var selection in selectedItems.Skip(1)) {

            selection.tile.shaders = FirstTile.shaders.Clone();

        }

        RefreshMeshes();

        RefreshShaderMapper();
        RefreshTileOverlayShader();

    }

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

}