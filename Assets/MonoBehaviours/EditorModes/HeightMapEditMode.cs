﻿
using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HeightMapEditMode.HeightSelectionSaveStateCounterAction;
using Object = UnityEngine.Object;

public class HeightMapEditMode : EditMode {

    public static bool keepHeightsOnTop = false;

    public Main main { get; set; }

    static public TileColumn selectedColumn = null;
    static public LevelMesh selectedSection = null;
    public List<HeightMapChannelPoint> heightPointObjects = new();
    public GameObject selectedSectionOverlay = null;

    public HeightMapEditPanelView view;

    HeightMapChannelPoint lastSelectedHeightChannel = null;
    TileColumn startGridRange = null;
    TileColumn lastHoveredColumn = null;

    public void Update() {
        
        if (FreeMove.looking) {

            if (Controls.IsDown("Select")) {
                
                var selection = main.GetTileOnLevelMesh(false);

                if (selection != null) {
                    SelectLevel(selection.column, selection.section);
                    lastHoveredColumn = selection.column;
                }

            }
            if (Controls.OnUp("Select")) {
                startGridRange = null;
                lastHoveredColumn = null;
            }

        }

        TestHeightMapChannelSelection();

        if (Controls.OnDown("Unselect")) {

            DeselectAllHeights();

        }

    }

    public void OnCreateMode() {
        ReinitExistingSelectedItems();
    }

    public void OnDestroy() {
        ClearAllGameObjects();
    }

    public HeightMapEditMode(Main main) {
        this.main = main;
    }

    #region Selection

    public void SelectLevel(TileColumn column, LevelMesh section) {

        var isDifferentSection = section != selectedSection;

        if (isDifferentSection && Controls.OnDown("Select")) {

            AddSectionSelectionSaveStateCounterAction();

            foreach (var obj in heightPointObjects) {
                Object.Destroy(obj.gameObject);
            }

            heightPointObjects.Clear();

            if (selectedSectionOverlay != null) {
                Object.Destroy(selectedSectionOverlay);
            }

            if (section == null) {
                selectedSection = null;
                return;
            }

            selectedSectionOverlay = Object.Instantiate(main.SectionBoarders, new Vector3(section.x, 0, -section.y), Quaternion.identity);

            selectedSection = section;

            AddAllHeights();

        }

        if (selectedSection == section) {

            selectedColumn = column;

        }

        // If the number of the height channel is held down, all HeightMapChannelPoints will be selected in the column.
        if (Controls.IsDown("ModifierChannelSelect1")) {

            GridRangeSelect(1);

        } else if (Controls.IsDown("ModifierChannelSelect2")) {

            GridRangeSelect(2);


        } else if (Controls.IsDown("ModifierChannelSelect3")) {

            GridRangeSelect(3);

        }

    }

    void SelectAllInColumn(int channel) {

        AddHeightSelectionSaveStateCounterAction();

        startGridRange = selectedColumn;

        foreach (var height in selectedColumn.heights) {

            var heightobj = heightPointObjects.First(obj => {
                return obj.heightPoints == height && obj.channel == channel;
            });
            heightobj.Select();

        }

    }

    void RangeSelect(HeightMapChannelPoint heightVertex) {

        if (lastSelectedHeightChannel == null) {
            SelectAllHeightChannelsInSection(heightVertex.channel);
            return;
        }

        AddHeightSelectionSaveStateCounterAction();

        var firstClickX = lastSelectedHeightChannel.x;
        var firstClickY = lastSelectedHeightChannel.y;

        var firstClickChannel = lastSelectedHeightChannel.channel;

        var lastClickX = heightVertex.x;
        var lastClickY = heightVertex.y;

        var lastClickChannel = heightVertex.channel;

        var startX = firstClickX < lastClickX ? firstClickX : lastClickX;
        var startY = firstClickY < lastClickY ? firstClickY : lastClickY;

        var endX = firstClickX > lastClickX ? firstClickX : lastClickX;
        var endY = firstClickY > lastClickY ? firstClickY : lastClickY;

        var startChannel = firstClickChannel < lastClickChannel ? firstClickChannel : lastClickChannel;
        var endChannel = firstClickChannel > lastClickChannel ? firstClickChannel : lastClickChannel;

        foreach (var y in Enumerable.Range(startY, endY - startY + 1)) {

            foreach (var x in Enumerable.Range(startX, endX - startX + 1)) {

                var heights = heightPointObjects.Where(point => { return point.x == x && point.y == y; });

                foreach (var height in heights) {

                    if (height.channel >= startChannel && height.channel <= endChannel) {
                        height.Select();
                    }

                }

            }

        }

    }

    void GridRangeSelect(int channel) {

        if (startGridRange == null) {
            SelectAllInColumn(channel);
            return;
        }

        if (lastHoveredColumn == selectedColumn) {
            return;
        }

        AddHeightSelectionSaveStateCounterAction();

        var firstClickX = startGridRange.x;
        var firstClickY = startGridRange.y;

        var lastClickX = selectedColumn.x;
        var lastClickY = selectedColumn.y;

        var startX = firstClickX < lastClickX ? firstClickX : lastClickX;
        var startY = firstClickY < lastClickY ? firstClickY : lastClickY;

        var endX = firstClickX > lastClickX ? firstClickX : lastClickX;
        var endY = firstClickY > lastClickY ? firstClickY : lastClickY;

        foreach (var y in Enumerable.Range(startY, endY - startY + 1)) {

            foreach (var x in Enumerable.Range(startX, endX - startX + 1)) {

                var column = selectedSection.section.GetTileColumn(x, y);

                foreach (var height in column.heights) {

                    var heightobj = heightPointObjects.First(obj => {
                        return obj.heightPoints == height && obj.channel == channel;
                    });
                    heightobj.Select();

                }

            }

        }

    }


    void SelectAllHeightChannelsInSection(int channel) {

        AddHeightSelectionSaveStateCounterAction();

        foreach (var heightObj in heightPointObjects) {

            if (heightObj.channel == channel) {
                heightObj.Select();
            }

        }
    }

    void TestHeightMapChannelSelection() {

        if (FreeMove.looking) {
            return;
        }

        if (!Controls.OnDown("Select") && !Controls.OnDown("Interact")) {
            return;
        }

        if (Main.IsMouseOverUI()) {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 8)) {

            foreach (var channel in heightPointObjects) {

                if (hit.colliderInstanceID == channel.boxCollider.GetInstanceID()) {

                    if (Controls.IsDown("ModifierChannelSelect1") && channel.channel != 1) {
                        return;
                    }
                    if (Controls.IsDown("ModifierChannelSelect2") && channel.channel != 2) {
                        return;
                    }
                    if (Controls.IsDown("ModifierChannelSelect3") && channel.channel != 3) {
                        return;
                    }

                    if (Controls.OnDown("RangeSelect")) {
                        RangeSelect(channel);
                    }
                    else if (Controls.OnDown("MultiSelect")) {

                        AddHeightSelectionSaveStateCounterAction();

                        channel.SelectOrDeSelect();

                        if (channel.isSelected) {
                            lastSelectedHeightChannel = channel;
                        }
                        else {
                            lastSelectedHeightChannel = null;
                        }

                    }
                    else if (Controls.OnDown("Select")) {

                        channel.Click();

                    }

                    if (Controls.OnDown("Interact")) {

                        AddHeightSelectionSaveStateCounterAction();

                        channel.ChangeExactHeight();

                    }

                }

            }

        }

    }

    #endregion

    #region GameObject Managment

    public void RefreshHeights() {

        foreach (var obj in heightPointObjects) {
            obj.RefreshHeight();
        }

    }

    void ReinitExistingSelectedItems() {

        if (selectedColumn == null) {
            return;
        }

        if (selectedSection == null) {
            return;
        }

        AddAllHeights();

        selectedSectionOverlay = Object.Instantiate(main.SectionBoarders, new Vector3(selectedSection.x, 0, -selectedSection.y), Quaternion.identity);

    }

    void AddAllHeights() {

        foreach (var y in Enumerable.Range(0, 16)) {

            foreach (var x in Enumerable.Range(0, 16)) {

                var itColumn = selectedSection.section.GetTileColumn(x, y);

                AddSingleHeightChannelObject(VertexPosition.TopLeft, 1, itColumn, x, y);
                AddSingleHeightChannelObject(VertexPosition.TopRight, 1, itColumn, x, y);
                AddSingleHeightChannelObject(VertexPosition.BottomLeft, 1, itColumn, x, y);
                AddSingleHeightChannelObject(VertexPosition.BottomRight, 1, itColumn, x, y);

                AddSingleHeightChannelObject(VertexPosition.TopLeft, 2, itColumn, x, y);
                AddSingleHeightChannelObject(VertexPosition.TopRight, 2, itColumn, x, y);
                AddSingleHeightChannelObject(VertexPosition.BottomLeft, 2, itColumn, x, y);
                AddSingleHeightChannelObject(VertexPosition.BottomRight, 2, itColumn, x, y);

                AddSingleHeightChannelObject(VertexPosition.TopLeft, 3, itColumn, x, y);
                AddSingleHeightChannelObject(VertexPosition.TopRight, 3, itColumn, x, y);
                AddSingleHeightChannelObject(VertexPosition.BottomLeft, 3, itColumn, x, y);
                AddSingleHeightChannelObject(VertexPosition.BottomRight, 3, itColumn, x, y);


            }

        }

    }

    void AddSingleHeightChannelObject(VertexPosition corner, int channel, TileColumn column, int x, int y) {

        var existingHeightChannel = heightPointObjects.Find(obj => {
            return obj.heightPoints == column.heights[(int)corner - 1] && obj.channel == channel;
        });

        if (existingHeightChannel != null) {
            return;
        }

        var worldX = selectedSection.x + column.x;
        var worldY = -(selectedSection.y + column.y);

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

        var point = Object.Instantiate(main.heightMapChannelPoint, pos, Quaternion.identity);
        var script = point.GetComponent<HeightMapChannelPoint>();
        script.x = x;
        script.y = y;
        script.heightPoints = column.heights[(int)corner - 1];
        script.controller = this;
        script.channel = channel;
        script.section = selectedSection;

        heightPointObjects.Add(script);

    }

    void DeselectAllHeights() {

        AddHeightSelectionSaveStateCounterAction();

        foreach (var height in heightPointObjects) {

            height.DeSelect();

        }

        lastSelectedHeightChannel = null;
        startGridRange = null;
        lastHoveredColumn = null;

    }

    void ClearAllSelectedItems() {

        foreach (var obj in heightPointObjects) {

            Object.Destroy(obj.gameObject);

        }

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
            selectedSectionOverlay = null;
        }


        heightPointObjects.Clear();

        selectedColumn = null;
        selectedSection = null;

    }

    void ClearAllGameObjects() {

        foreach (var obj in heightPointObjects) {
            Object.Destroy(obj.gameObject);
        }

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
        }

    }

    #endregion

    #region Model Mutating

    public void SetHeightsWithValue(int value) {

        AddHeightMapSaveStateCounterAction();

        foreach (var point in heightPointObjects) {

            if (point.isSelected) {

                point.heightPoints.SetPoint(value, point.channel);

                point.RefreshHeight();

                if (keepHeightsOnTop) {
                    point.KeepHigherChannelsOnTop();
                }

            }

        }

    }

    public void DiscardUnusedChannels() {

        if (selectedSection == null) {
            QuickLogHandler.Log("No sections are selected", LogSeverity.Info);
            return;
        }

        AddAllHeightMapSaveStateCounterAction();
        selectedSection.section.DiscardUnusedHeights();
        RefreshHeights();
        selectedSection.RefreshMesh();

        QuickLogHandler.Log("Unused channels (vertices) sent to -128", LogSeverity.Success);

    }

    #endregion

    #region Callbacks

    public void MoveAllHeights(float distance) {

        foreach (var heightObj in heightPointObjects) {

            if (heightObj.isSelected) {
                heightObj.MoveHeight(distance);
            }

        }

    }

    public void UnselectHeights() {

        foreach (var height in heightPointObjects) {

            height.DeSelect();

        }

    }

    #endregion

    #region Counter-Actions

    public class HeightSelectionSaveStateCounterAction : CounterAction {

        public string name { get; set; }

        public class HeightPointSaveState {

            public HeightPoints heightPoints;
            public int channel;

            public HeightPointSaveState(HeightPoints heightPoints, int channel) {
                this.heightPoints = heightPoints;
                this.channel = channel;

            }

        }

        List<HeightPointSaveState> selectedPoints;

        public HeightSelectionSaveStateCounterAction(List<HeightPointSaveState> selectedPoints) {

            this.selectedPoints = new(selectedPoints);

            name = "Height Map Selection";

        }

        public void Action() {

            if (Main.editMode is not HeightMapEditMode) {
                return;
            }

            var editMode = (HeightMapEditMode)Main.editMode;

            foreach (var heightObj in editMode.heightPointObjects) {

                var found = false;
                foreach (var saveState in selectedPoints) {

                    if (saveState.heightPoints == heightObj.heightPoints && saveState.channel == heightObj.channel) {

                        heightObj.Select();
                        found = true;

                        break;

                    }

                }

                if (!found) {
                    heightObj.DeSelect();
                }

            }

        }

    }

    public class SectionSelectionSaveStateCounterAction : CounterAction {

        public string name { get; set; }

        LevelMesh selectedLevelMesh;

        public SectionSelectionSaveStateCounterAction(LevelMesh selectedLevelMesh) {
            this.selectedLevelMesh = selectedLevelMesh;

            name = "Section Selection";
        }

        public void Action() {

            if (Main.editMode is not HeightMapEditMode) {
                return;
            }

            var editMode = (HeightMapEditMode)Main.editMode;

            editMode.SelectLevel(null, selectedLevelMesh);

            // This is here because the methods adds the same counter action
            Main.PopCounterAction();

        }

    }

    public void AddHeightSelectionSaveStateCounterAction() {

        var total = new List<HeightPointSaveState>();

        foreach (var heightObj in heightPointObjects) {

            if (heightObj.isSelected) {
                total.Add(new HeightPointSaveState(heightObj.heightPoints, heightObj.channel));
            }

        }

        Main.AddCounterAction(new HeightSelectionSaveStateCounterAction(new(total)));

    }

    public void AddHeightMapSaveStateCounterActionFromClick(HeightMapChannelPoint point) {

        if (point.isSelected) {

            AddHeightMapSaveStateCounterAction();

        } else {

            Main.AddCounterAction(new HeightMapSaveStateCounterAction(point.heightPoints, () => {

                if (Main.editMode is not HeightMapEditMode) {
                    return;
                }

                var editMode = (HeightMapEditMode)Main.editMode;

                editMode.RefreshHeights();

                HeightMapEditMode.selectedSection.RefreshMesh();


            }));

        }

    }

    public void AddHeightMapSaveStateCounterAction() {

        var affectedHeights = new List<HeightPoints>();

        foreach (var heightObj in heightPointObjects) {

            if (heightObj.isSelected) {
                affectedHeights.Add(heightObj.heightPoints);
            }

        }

        Main.AddCounterAction(new MultiHeightMapSaveStateCounterAction(new(affectedHeights), () => {

            if (Main.editMode is not HeightMapEditMode) {
                return;
            }

            var editMode = (HeightMapEditMode)Main.editMode;

            editMode.RefreshHeights();

            HeightMapEditMode.selectedSection.RefreshMesh();


        }));

    }

    public void AddAllHeightMapSaveStateCounterAction() {

        var affectedHeights = new List<HeightPoints>();

        foreach (var heightObj in heightPointObjects) {
            
            affectedHeights.Add(heightObj.heightPoints);

        }

        Main.AddCounterAction(new MultiHeightMapSaveStateCounterAction(new(affectedHeights), () => {

            if (Main.editMode is not HeightMapEditMode) {
                return;
            }

            var editMode = (HeightMapEditMode)Main.editMode;

            editMode.RefreshHeights();

            HeightMapEditMode.selectedSection.RefreshMesh();


        }));

    }

    public void AddSectionSelectionSaveStateCounterAction() {

        Main.AddCounterAction(new SectionSelectionSaveStateCounterAction(selectedSection));

    }

    #endregion

}