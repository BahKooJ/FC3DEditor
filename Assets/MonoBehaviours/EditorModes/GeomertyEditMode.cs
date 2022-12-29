
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

class GeometryEditMode : EditMode {

    public Main main { get; set; }

    public void Update() {
        
        if (FreeMove.looking) {
            main.TestRayOnLevelMesh();
        }

    }

    public void OnCreateMode() {

    }

    public void OnDestroy() {

    }


    public GeometryEditMode(Main main) {
        this.main = main;
    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {
        // TODO: Add overlay when looking at tile
    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

        if (Input.GetKey(KeyCode.LeftShift)) {

            if (main.selectedSection != null) {
                if (main.selectedSection != section) {
                    return;
                }
            }

        } else {

            main.selectedTiles.Clear();

            main.ClearTileOverlays();

        }

        main.selectedTiles.Add(tile);
        main.selectedColumn = column;
        main.selectedSection = section;

        var oldPoints = new List<HeightMapChannelPoint>(main.heightPointObjects);

        main.heightPointObjects.Clear();

        foreach (var obj in oldPoints) {

            if (!obj.isSelected) {
                Object.Destroy(obj.gameObject);
            } else {
                main.heightPointObjects.Add(obj);
            }

        }

        main.AddHeightObjects(0);
        main.AddHeightObjects(1);
        main.AddHeightObjects(2);
        main.AddHeightObjects(3);

        if (Input.GetKey(KeyCode.Alpha1)) {

            foreach (var obj in main.heightPointObjects) {

                if (obj.channel == 1) {
                    obj.Select();
                }

            }

        } else if (Input.GetKey(KeyCode.Alpha2)) {

            foreach (var obj in main.heightPointObjects) {

                if (obj.channel == 2) {
                    obj.Select();
                }

            }

        } else if (Input.GetKey(KeyCode.Alpha3)) {

            foreach (var obj in main.heightPointObjects) {

                if (obj.channel == 3) {
                    obj.Select();
                }

            }

        }

        main.InitTileOverlay(tile);

    }
}