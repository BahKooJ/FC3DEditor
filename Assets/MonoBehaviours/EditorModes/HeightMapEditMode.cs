
using FCopParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class HeightMapEditMode : EditMode {

    public static bool keepHeightsOnTop = false;

    public Main main { get; set; }

    public TileColumn selectedColumn = null;
    public LevelMesh selectedSection = null;
    public List<HeightMapChannelPoint> heightPointObjects = new();
    public List<SelectedTileOverlay> selectedTileOverlays = new();
    public GameObject selectedSectionOverlay = null;

    public HeightMapEditPanelView view;

    public HeightMapChannelPoint lastSelectedHeightChannel = null;

    public void Update() {
        
        if (FreeMove.looking) {
            main.TestRayOnLevelMesh();
        }

        TestHeightMapChannelSelection();

        if (Controls.OnDown("Unselect")) {

            foreach (var height in heightPointObjects) {
                
                height.DeSelect();

            }

        } 


    }

    public void OnCreateMode() {

    }

    public void OnDestroy() {
        ClearAllSelectedItems();

        if (view.debugTilePanelView != null) {
            Object.Destroy(view.debugTilePanelView);
        }
    }

    public HeightMapEditMode(Main main) {
        this.main = main;
    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {
        // TODO: Add overlay when looking at tile
    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

        var isDifferentSection = section != selectedSection;

        if (isDifferentSection) {

            foreach (var obj in heightPointObjects) {
                Object.Destroy(obj.gameObject);
            }

            heightPointObjects.Clear();

            if (selectedSectionOverlay != null) {
                Object.Destroy(selectedSectionOverlay);
            }

            selectedSectionOverlay = Object.Instantiate(main.SectionBoarders, new Vector3(section.x, 0, -section.y), Quaternion.identity);

        }
        
        // Updates the remaining data
        selectedColumn = column;
        selectedSection = section;

        if (isDifferentSection) {
            AddAllHeights();
        }


        // If the number of the height channel is held down, all HeightMapChannelPoints will be selected in the column.
        if (Controls.IsDown("ModifierChannelSelect1")) {

            SelectAllInColumn(1);

        } 
        else if (Controls.IsDown("ModifierChannelSelect2")) {

            SelectAllInColumn(2);


        }
        else if (Controls.IsDown("ModifierChannelSelect3")) {

            SelectAllInColumn(3);

        }

    }

    #region Local Methods

    void TestHeightMapChannelSelection() {

        if (FreeMove.looking) {
            return;
        }

        if (!Controls.OnDown("Select") && !Controls.OnDown("Interact") && Input.GetAxis("Mouse ScrollWheel") == 0) {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

            foreach (var channel in heightPointObjects) {

                if (hit.colliderInstanceID == channel.boxCollider.GetInstanceID()) {

                    if (Controls.OnDown("Select") && Controls.IsDown("ModifierMultiSelect")) {

                        if (Controls.IsDown("ModifierAltSelect")) {
                            SelectAllHeightChannelsInSection(channel.channel);
                            return;
                        } else {
                            channel.SelectOrDeSelect();
                        }

                    } else if (Controls.OnDown("Select")) {

                        channel.Click();

                    }

                    if (Controls.OnDown("Interact")) {

                        channel.ChangeExactHeight();

                    }
                
                }

            }


        }

    }

    void SelectAllInColumn(int channel) {
        
        foreach (var height in selectedColumn.heights) {

            var heightobj = heightPointObjects.First( obj => {
                return obj.heightPoints == height && obj.channel == channel;
            });
            heightobj.Select();

        }

    }

    void SelectAllHeightChannelsInSection(int channel) {

        foreach (var heightObj in heightPointObjects) {

            if (heightObj.channel == channel) {
                heightObj.Select();
            }

        }

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

    void AddAllHeights() {

        foreach (var y in Enumerable.Range(0, 16)) {

            foreach (var x in Enumerable.Range(0, 16)) {

                var itColumn = selectedSection.section.GetTileColumn(x, y);

                AddSingleHeightChannelObject(VertexPosition.TopLeft, 1, itColumn);
                AddSingleHeightChannelObject(VertexPosition.TopRight, 1, itColumn);
                AddSingleHeightChannelObject(VertexPosition.BottomLeft, 1, itColumn);
                AddSingleHeightChannelObject(VertexPosition.BottomRight, 1, itColumn);

                AddSingleHeightChannelObject(VertexPosition.TopLeft, 2, itColumn);
                AddSingleHeightChannelObject(VertexPosition.TopRight, 2, itColumn);
                AddSingleHeightChannelObject(VertexPosition.BottomLeft, 2, itColumn);
                AddSingleHeightChannelObject(VertexPosition.BottomRight, 2, itColumn);

                AddSingleHeightChannelObject(VertexPosition.TopLeft, 3, itColumn);
                AddSingleHeightChannelObject(VertexPosition.TopRight, 3, itColumn);
                AddSingleHeightChannelObject(VertexPosition.BottomLeft, 3, itColumn);
                AddSingleHeightChannelObject(VertexPosition.BottomRight, 3, itColumn);


            }

        }

    }

    void AddHeightObjects(VertexPosition corner) {

        AddSingleHeightChannelObject(corner, 1, selectedColumn);
        AddSingleHeightChannelObject(corner, 2, selectedColumn);
        AddSingleHeightChannelObject(corner, 3, selectedColumn);

    }

    void AddSingleHeightChannelObject(VertexPosition corner, int channel, TileColumn column) {

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
        script.heightPoints = column.heights[(int)corner - 1];
        script.controller = this;
        script.channel = channel;
        script.section = selectedSection;

        heightPointObjects.Add(script);

    }

    #endregion

    #region Public and Callback Methods

    public void MoveAllHeights(float distance) {

        foreach (var heightObj in heightPointObjects) {

            if (heightObj.isSelected) {
                heightObj.MoveHeight(distance);
            }

        }

    }

    public void UnselectAndRefreshHeightPoints() {

        foreach (var obj in heightPointObjects) {

            Object.Destroy(obj.gameObject);

        }

        heightPointObjects.Clear();

        AddHeightObjects(VertexPosition.TopLeft);
        AddHeightObjects(VertexPosition.TopRight);
        AddHeightObjects(VertexPosition.BottomLeft);
        AddHeightObjects(VertexPosition.BottomRight);

    }

    public void RefreshSelectedOverlays() {

        foreach (var overlay in selectedTileOverlays) {

            overlay.Refresh();

        }

    }

    #endregion

}