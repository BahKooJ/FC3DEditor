


using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;
using Object = UnityEngine.Object;

public class ShaderEditMode : TileMutatingEditMode, EditMode {

    public static bool openShaderMapperByDefault = true;

    public Main main { get; set; }

    public List<TileTexturePreview> selectedTileOverlays = new();
    public List<GameObject> selectedSectionOverlays = new();
    public List<VertexColorPoint> vertexColorPoints = new();

    public ShaderEditPanelView view;

    public ShaderPresets currentShaderPresets;

    public bool worldSpaceEditMode = true;

    public ShaderEditMode(Main main) {
        this.main = main;
    }

    public void Update() {

        if (FreeMove.looking) {
            main.TestRayOnLevelMesh();
        }

        if (Main.ignoreAllInputs) { return; }

        TestVertexColorCornerSelection();

        if (Controls.OnDown("Save")) {
            if (view.activeShaderMapper != null) {
                view.CloseShaderMapper();
            }
        }

        if (Controls.OnDown("Unselect")) {

            if (view.activeShaderMapper != null) {
                view.CloseShaderMapper();
            }

            ClearAllSelectedItems();
        }

    }

    public void OnCreateMode() {
        currentShaderPresets = Presets.shaderPresets;
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

            if (view.activeShaderMapper != null) {
                view.activeShaderMapper.GetComponent<ShaderMapperView>().RefreshView();
            }
            else {
                
                if (openShaderMapperByDefault) {
                    view.OpenShaderMapper();
                }


            }



        }
        else if (IsSameShape(tile)) {

            MakeSelection(tile, column, section);

        }
        else {
            return;
        }

        ClearSectionOverlays();

        foreach (var iSection in selectedSections) {
            selectedSectionOverlays.Add(Object.Instantiate(main.SectionBoarders, new Vector3(iSection.x, 0, -iSection.y), Quaternion.identity));
        }

        if (worldSpaceEditMode) {
            InitVertexColorCorners();
        }

        RefeshTileOverlay();

    }

    override public void MakeSelection(Tile tile, TileColumn column, LevelMesh section, bool deSelectDuplicate = true) {

        if (IsTileAlreadySelected(tile)) {

            if (deSelectDuplicate) {
                RemoveTile(tile);
                RefeshTileOverlay();

                if (!HasSelection) {

                    if (view.activeShaderMapper != null) {
                        view.CloseShaderMapper();
                    }

                    ClearAllSelectedItems();

                }

            }

        }
        else {

            selectedItems.Add(new TileSelection(tile, column, section));
            selectedSections.Add(section);

        }

    }

    void TestVertexColorCornerSelection() {

        if (FreeMove.looking) {
            return;
        }

        if (!Controls.OnDown("Select") && !Controls.OnDown("Interact")) {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

            foreach (var vertex in vertexColorPoints) {

                if (hit.colliderInstanceID == vertex.boxCollider.GetInstanceID()) {

                    if (Controls.OnDown("Select") && Controls.IsDown("ModifierMultiSelect")) {

                        vertex.SelectOrDeselect();

                    }
                    else if (Controls.OnDown("Select")) {

                        foreach (var vert in vertexColorPoints) {
                            vert.Deselect();
                        }

                        vertex.Select();

                    }

                }

            }

        }

    }

    void TestPaint() {

        if (FreeMove.looking) {
            return;
        }

        if (!Controls.IsDown("Select")) {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.SphereCast(ray, 1.4f, Mathf.Infinity, 8)) {

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

    void InitVertexColorCorners() {

        ClearVertexColors();

        if (!HasSelection) {
            return;
        }

        foreach (var i in Enumerable.Range(0, FirstTile.verticies.Count)) {
            InitSingleVertexColorCorner(i);
        }

    }

    void InitSingleVertexColorCorner(int index) {

        var firstSelectedItem = selectedItems[0];

        var worldX = firstSelectedItem.section.x + firstSelectedItem.column.x;
        var worldY = -(firstSelectedItem.section.y + firstSelectedItem.column.y);

        var tileVert = firstSelectedItem.tile.verticies[index];

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

        var pos = new Vector3(worldX, firstSelectedItem.column.heights[(int)tileVert.vertexPosition - 1].GetPoint(tileVert.heightChannel), worldY);


        var obj = Object.Instantiate(main.vertexColorPoint, pos, Quaternion.identity);
        var vertPoint = obj.GetComponent<VertexColorPoint>();
        vertPoint.controller = this;

        if (view.activeShaderMapper != null) {
            vertPoint.mapper = view.activeShaderMapper.GetComponent<ShaderMapperView>();
        }

        vertPoint.selectedItem = selectedItems[0];
        vertPoint.index = index;

        vertexColorPoints.Add(vertPoint);

    }

    void ClearVertexColors() {

        foreach (var vertexColorPoint in vertexColorPoints) {
            Object.Destroy(vertexColorPoint.gameObject);
        }

        vertexColorPoints.Clear();

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

    // This is called by the mapper
    public void ApplyColorsToVertexColorCorners() {

        foreach (var colorPoint in vertexColorPoints) {

            if (colorPoint.isSelected) {
                colorPoint.ChangeValue();
            }

        }

    }

    public void RefreshShaderMapper() {

        if (view.activeShaderMapper != null) {
            view.activeShaderMapper.GetComponent<ShaderMapperView>().RefreshView();
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

    public void AddPresetsDirectory() {

        currentShaderPresets.subFolders.Add(new ShaderPresets("", currentShaderPresets));

    }

}