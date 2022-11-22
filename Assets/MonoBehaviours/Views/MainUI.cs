
using FCopParser;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

class MainUI: MonoBehaviour {

    public Main controller;

    public GameObject rotateLeftButton;
    public GameObject rotateRightButton;
    public GameObject shiftHeightUpButton;
    public GameObject shiftHeightDownButton;
    public GameObject addTilePresetButton;
    public GameObject tilePresets;
    public GameObject graphicsPropertiesButton;
    public GameObject showTilesButton;

    public GameObject graphicsPropertiesView;

    public GameObject activeGraphicsPropertiesView = null;

    void Start() {

        foreach (Object obj in transform) {

            switch (obj.GameObject().name) {

                case "Rotate Left Button":
                    rotateLeftButton = obj.GameObject();
                    break;
                case "Rotate Right Button":
                    rotateRightButton = obj.GameObject();
                    break;
                case "Shift Height Up Button":
                    shiftHeightUpButton = obj.GameObject();
                    break;
                case "Shift Height Down Button":
                    shiftHeightDownButton = obj.GameObject();
                    break;
                case "Add Tile Preset Button":
                    addTilePresetButton = obj.GameObject();
                    break;
                case "Tile Presets":
                    tilePresets = obj.GameObject();
                    break;
                case "Graphics Properties Button":
                    graphicsPropertiesButton = obj.GameObject();
                    break;
                case "Show Tiles Button":
                    showTilesButton = obj.GameObject();
                    break;

            }

        }

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.M)) {

            if (activeGraphicsPropertiesView != null) {
                Destroy(activeGraphicsPropertiesView);
                controller.selectedSection.RefreshMesh();
            }

        }

    }

    public void OnClickGraphicsProperitesButton() {

        if (controller.selectedTile == null) { return; }

        if (activeGraphicsPropertiesView != null) {
            Destroy(activeGraphicsPropertiesView);
            controller.selectedSection.RefreshMesh();
        } else {

            activeGraphicsPropertiesView = Instantiate(graphicsPropertiesView);

            activeGraphicsPropertiesView.GetComponent<GraphicsPropertiesView>().controller = controller;

            activeGraphicsPropertiesView.transform.SetParent(transform.parent, false);

        }

    }

    public void OnClickRotateLeftButton() {

        if (controller.selectedTile.verticies.Count == 3) {

            var verticies = controller.selectedTile.verticies;

            bool isBottomRight =
                (verticies[0].vertexPosition == VertexPosition.TopRight) &&
                (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
                (verticies[2].vertexPosition == VertexPosition.BottomRight);

            bool isBottomLeft =
                (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
                (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
                (verticies[2].vertexPosition == VertexPosition.BottomRight);

            bool isTopLeft =
                (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
                (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
                (verticies[2].vertexPosition == VertexPosition.TopRight);

            bool isTopRight =
                (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
                (verticies[1].vertexPosition == VertexPosition.BottomRight) &&
                (verticies[2].vertexPosition == VertexPosition.TopRight);

            if (isBottomRight) {
                verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
                verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
                verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.BottomRight);
            } else if (isBottomLeft) {
                verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
                verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
                verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.TopRight);
            } else if (isTopLeft) {
                verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
                verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomRight);
                verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.TopRight);
            } else if (isTopRight) {
                verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopRight);
                verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
                verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.BottomRight);
            }

        } 
        
        //else {

        //    var verticies = controller.selectedTile.verticies;

        //    var isWall = false;

        //    var vertexToTest = verticies[0];
        //    var verticiesToTest = verticies.GetRange(1,3);

        //    foreach (var vertex in verticiesToTest) {

        //        if (vertexToTest.vertexPosition == vertex.vertexPosition) {
        //            isWall = true;
        //            break;
        //        }

        //    }

        //    if (!isWall) {
        //        return;
        //    }

        //}


        controller.selectedSection.RefreshMesh();

    }

    public void OnClickRotateRightButton() {

        if (controller.selectedTile.verticies.Count != 3) {
            return;
        }

        var verticies = controller.selectedTile.verticies;

        bool isBottomRight =
            (verticies[0].vertexPosition == VertexPosition.TopRight) &&
            (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
            (verticies[2].vertexPosition == VertexPosition.BottomRight);

        bool isBottomLeft =
            (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
            (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
            (verticies[2].vertexPosition == VertexPosition.BottomRight);

        bool isTopLeft =
            (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
            (verticies[1].vertexPosition == VertexPosition.BottomLeft) &&
            (verticies[2].vertexPosition == VertexPosition.TopRight);

        bool isTopRight =
            (verticies[0].vertexPosition == VertexPosition.TopLeft) &&
            (verticies[1].vertexPosition == VertexPosition.BottomRight) &&
            (verticies[2].vertexPosition == VertexPosition.TopRight);

        if (isBottomRight) {
            verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
            verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
            verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.TopRight);
        } else if (isBottomLeft) {
            verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
            verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
            verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.TopRight);
        } else if (isTopLeft) {
            verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopLeft);
            verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
            verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.BottomRight);
        } else if (isTopRight) {
            verticies[0] = new TileVertex(verticies[0].heightChannel, VertexPosition.TopRight);
            verticies[1] = new TileVertex(verticies[1].heightChannel, VertexPosition.BottomLeft);
            verticies[2] = new TileVertex(verticies[2].heightChannel, VertexPosition.BottomRight);
        }

        controller.selectedSection.RefreshMesh();

    }

    public void OnClickShiftHeightUpButton() {

        foreach (var index in Enumerable.Range(0,controller.selectedTile.verticies.Count)) {

            var vertex = controller.selectedTile.verticies[index];

            if (vertex.heightChannel < 3) {
                controller.selectedTile.verticies[index] = new TileVertex(vertex.heightChannel + 1, vertex.vertexPosition);
            }

        }

        controller.selectedSection.RefreshMesh();

    }

    public void OnClickShiftHeightDownButton() {

        foreach (var index in Enumerable.Range(0, controller.selectedTile.verticies.Count)) {

            var vertex = controller.selectedTile.verticies[index];

            if (vertex.heightChannel > 1) {
                controller.selectedTile.verticies[index] = new TileVertex(vertex.heightChannel - 1, vertex.vertexPosition);
            }

        }

        controller.selectedSection.RefreshMesh();

    }

}

